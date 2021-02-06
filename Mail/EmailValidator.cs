using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TicketProvider.Net.Dns;
using TicketProvider.Net.Dns.Records;
using TicketProvider.Net.Mail.Smtp;
using TicketProvider.Net.Mail.Smtp.Commands;

namespace TicketProvider.Net.Mail
{
    public static class EmailValidator
    {

        private static string _reversePath;

        static EmailValidator()
        {

            _reversePath = "no-reply@ticketprovider.com";
        }

        /// <summary>
        /// Validates the syntax of the passed email address.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>Returns true if the passed email address is syntactically valid, false if not.</returns>
        public static bool ValidateSyntax(EmailAddress emailAddress)
        {
            return ValidateSyntax(emailAddress.Email);
        }

        /// <summary>
        /// Validates the syntax of the passed email address.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>Returns true if the passed email address is syntactically valid, false if not.</returns>
        public static bool ValidateSyntax(string emailAddress)
        {

            // Declare variables
            Regex emailRegex;

            // Validate email address against regex
            emailRegex = new Regex("\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");
            return emailRegex.IsMatch(emailAddress);
        }

        /// <summary>
        /// Begins asynchonous deep email validation for the specified email address.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static IAsyncResult BeginDeepEmailValidation(EmailAddress emailAddress, AsyncCallback asyncCallback, object state)
        {

            // Declare variables
            EmailValidatorSession session;

            // Initialize new session
            session = new EmailValidatorSession(emailAddress, new AsyncResult<EmailValidatorResult>(asyncCallback, state));

            try
            {

                // Start by validating the email address syntactically.
                if (!ValidateSyntax(emailAddress))
                {
                    session.Result.SynaxValid = EmailValidatorResultState.Failed;
                    session.Complete(true);
                }
                else
                {

                    // Update validation result and begin resolving MX records for the email address to be validated.
                    session.Result.SynaxValid = EmailValidatorResultState.Success;
                    session.MxServerQueryType = ResourceRecordType.MX;
                    DnsResolver.BeginQuery(emailAddress.Domain, session.MxServerQueryType, QueryCallback, session);
                }
            }
            catch (Exception ex)
            {
                session.Complete(ex, true);
            }

            return session.AsyncResult;
        }

        private static void QueryCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            EmailValidatorSession session;
            ResourceRecord[] records;
            List<EmailValidatorMxServer> mxServers;
            List<IPAddress> mxAddresses;

            session = (EmailValidatorSession)asyncResult.AsyncState;
            try
            {

                // End call to BeginQuery() and inspect the result based on what kind of record is being queried for.
                records = DnsResolver.EndQuery(asyncResult);

                switch (session.MxServerQueryType)
                {
                    case ResourceRecordType.MX:

                        // Store obtained mx servers.
                        mxServers = new List<EmailValidatorMxServer>();
                        foreach (ResourceRecord record in records)
                        {
                            if (record.Type == ResourceRecordType.MX)
                            {
                                mxServers.Add(new EmailValidatorMxServer(((MXRecord)record).Exchange));
                            }
                        }

                        // If no MX records are found, revert to 'fallback to A' mode as specified in RFC 5321 section 5.
                        // Query for A or AAAA records.
                        if (mxServers.Count == 0)
                        {
                            session.MxServerQueryType = ResourceRecordType.A;
                            DnsResolver.BeginQuery(session.EmailAddress.Domain, session.MxServerQueryType, QueryCallback, session);
                            return;
                        }

                        break;
                    case ResourceRecordType.A:
                    case ResourceRecordType.AAAA:

                        // Since we are already in 'fallback to A' mode, the host clearly doesn't exist if no A records are
                        // found for the domain name. End the validation.
                        if (records.Length == 0)
                        {
                            session.Result.MxDefined = EmailValidatorResultState.Failed;
                            session.Complete(false);
                            return;
                        }

                        // Create surrogate MX server from ip addresses of domain name.
                        mxAddresses = new List<IPAddress>();
                        foreach (ResourceRecord record in records)
                        {
                            switch (record.Type)
                            {
                                case ResourceRecordType.A:
                                    mxAddresses.Add(((ARecord)record).IPAddress);
                                    break;
                                case ResourceRecordType.AAAA:
                                    mxAddresses.Add(((AAAARecord)record).IPAddress);
                                    break;
                            }
                        }
                        mxServers = new List<EmailValidatorMxServer>();
                        mxServers.Add(new EmailValidatorMxServer(session.EmailAddress.Domain, mxAddresses.ToArray()));

                        break;
                    default:
                        throw new MailException("Unexpected DNS resource record being queried.");
                }
                
                // Store the MX servers, update validation result and start connecting to the first MX server.
                session.Result.MxDefined = EmailValidatorResultState.Success;
                session.SetMxServers(mxServers);
                NextMxServer(session);
            }
            catch (Exception ex)
            {
                session.Complete(ex, false);
            }
        }

        private static void NextMxServer(EmailValidatorSession session)
        {
            try
            {

                // Check if the current MX server state is set to 'Success' or 'Rejected'. If so, do not move to the next MX server but
                // complete the validation according to the state of the last MX server.
                if (session.CurrentMxServer != null)
                {
                    if (session.CurrentMxServer.State == EmailValidatorMxServerState.Accepted)
                    {
                        session.Result.MxReachable = EmailValidatorResultState.Success;
                        session.Result.Deliverable = EmailValidatorResultState.Success;
                        session.Complete(false);
                        return;
                    }
                    if (session.CurrentMxServer.State == EmailValidatorMxServerState.Rejected)
                    {
                        session.Result.MxReachable = EmailValidatorResultState.Success;
                        session.Result.Deliverable = EmailValidatorResultState.Failed;
                        session.Complete(false);
                        return;
                    }
                }

                // Attempt to move the enumerator foward. If no next MX server is available, complete the validation.
                if (!session.NextMxServer())
                {
                    session.Result.MxReachable = EmailValidatorResultState.Unknown;
                    session.Result.Deliverable = EmailValidatorResultState.Unknown;
                    session.Complete(false);
                    return;
                }

                // Start resolving the addresses of the next mx server if no addresses are known for this mx server yet, else
                // start connecting straight away.
                if (session.CurrentMxServer.IpAddresses.Length == 0)
                    DnsResolver.BeginGetHostAddresses(session.CurrentMxServer.DomainName, NextMxServerResolveCallback, session);
                else
                    ConnectMxServer(session);
            }
            catch (Exception ex)
            {
                session.Complete(ex, false);
            }
        }

        private static void NextMxServerResolveCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            EmailValidatorSession session;
            IPAddress[] ipAddresses;

            session = (EmailValidatorSession)asyncResult.AsyncState;
            try
            {

                // End call to BeginGetHostAddresses() and check if any addresses were found.
                // If no addresses were found, move on to the next mx server.
                ipAddresses = DnsResolver.EndGetHostAddresses(asyncResult);
                if (ipAddresses.Length == 0)
                {
                    NextMxServer(session);
                    return;
                }

                // Store obtained ip addresses and attempt to connect.
                session.CurrentMxServer.IpAddresses = ipAddresses;
                ConnectMxServer(session);
            }
            catch (Exception ex)
            {
                
                // When an exception is caught, initially try to move to the next MX server. If that fails as well
                // complete the validation using the initally caught exception.
                try
                {
                    session.CurrentMxServer.State = EmailValidatorMxServerState.Failed;
                    NextMxServer(session);
                }
                catch (Exception)
                {
                    session.Complete(ex, false);
                }
            }
        }

        private static void ConnectMxServer(EmailValidatorSession session)
        {
            try
            {

                // Attempt to connect only to the first IP address of the MX server.
                session.SmtpClient = new SmtpClient(session.CurrentMxServer.IpAddresses[0]);
                session.SmtpState = EmailValidatorSessionSmtpState.Init;
                session.SmtpClient.BeginConnect(SmtpReplyCallback, session);
            }
            catch (Exception ex)
            {

                // When an exception is caught, initially try to move to the next MX server. If that fails as well
                // complete the validation using the initally caught exception.
                try
                {
                    session.CurrentMxServer.State = EmailValidatorMxServerState.Failed;
                    NextMxServer(session);
                }
                catch (Exception)
                {
                    session.Complete(ex, false);
                }
            }
        }

        private static void SmtpReplyCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            EmailValidatorSession session;
            SmtpReply reply;
            SmtpCommand command;

            session = (EmailValidatorSession)asyncResult.AsyncState;
            try
            {

                // Inspect the current SmtpState in order to determine what to do.
                switch (session.SmtpState)
                {
                    case EmailValidatorSessionSmtpState.Init:

                        // End call to BeginConnect() and inspect the reply.
                        reply = session.SmtpClient.EndConnect(asyncResult);
                        if (reply.Code != (int)SmtpReplyCode.ServiceReady)
                        {
                            session.CurrentMxServer.State = EmailValidatorMxServerState.Failed;
                            session.SmtpClient.BeginSendCommand(new QUITCommand(), SendQuitCallback, session);
                            return;
                        }

                        // Advance the state to Helo and send a SMTP HELO command
                        session.SmtpState = EmailValidatorSessionSmtpState.Helo;
                        command = new EHLOCommand(DnsResolver.GetHostAddresses("")[0]);
                        session.SmtpClient.BeginSendCommand(command, SmtpReplyCallback, session);

                        return;
                    case EmailValidatorSessionSmtpState.Helo:

                        // End call to BeginSendCommand() and inspect the reply.
                        reply = session.SmtpClient.EndSendCommand(asyncResult);
                        if (reply.Code != (int)SmtpReplyCode.Completed)
                        {
                            session.CurrentMxServer.State = EmailValidatorMxServerState.Failed;
                            session.SmtpClient.BeginSendCommand(new QUITCommand(), SendQuitCallback, session);
                            return;
                        }

                        // Advance the state to Mail and send a SMTP MAIL command
                        session.SmtpState = EmailValidatorSessionSmtpState.Mail;
                        command = new MAILCommand(new EmailAddress(_reversePath));
                        session.SmtpClient.BeginSendCommand(command, SmtpReplyCallback, session);

                        return;
                    case EmailValidatorSessionSmtpState.Mail:

                        // End call to BeginSendCommand() and inspect the reply.
                        reply = session.SmtpClient.EndSendCommand(asyncResult);
                        if (reply.Code != (int)SmtpReplyCode.Completed)
                        {
                            throw new SmtpException("Remote SMTP server did not accept reverse-path email address.", reply);
                        }

                        // Advance the state to Mail and send a SMTP RCPT command
                        session.SmtpState = EmailValidatorSessionSmtpState.Rcpt;
                        command = new RCPTCommand(session.EmailAddress);
                        session.SmtpClient.BeginSendCommand(command, SmtpReplyCallback, session);

                        return;
                    case EmailValidatorSessionSmtpState.Rcpt:

                        // End call to BeginSendCommand() and inspect the reply. If the server refused the RCPT command, do
                        // not move to the next MX server (backup servers may function as catch-all, thus invalidating the result),
                        // instead complete the validation indicating the mail address was refused.
                        reply = session.SmtpClient.EndSendCommand(asyncResult);
                        if (reply.Code != (int)SmtpReplyCode.Completed)
                        {
                            session.CurrentMxServer.State = EmailValidatorMxServerState.Rejected;
                            session.SmtpClient.BeginSendCommand(new QUITCommand(), SendQuitCallback, session);
                            return;
                        }

                        // Update MX server status and being closing the SMTP session.
                        session.CurrentMxServer.State = EmailValidatorMxServerState.Accepted;
                        session.SmtpClient.BeginSendCommand(new QUITCommand(), SendQuitCallback, session);

                        return;
                }
            }
            catch (Exception ex)
            {

                // When an exception is caught, initially try to move to the next MX server. If that fails as well
                // complete the validation using the initally caught exception.
                try
                {
                    session.CurrentMxServer.State = EmailValidatorMxServerState.Failed;
                    NextMxServer(session);
                }
                catch (Exception)
                {
                    session.Complete(ex, false);
                }
            }
        }

        /// <summary>
        /// RFC 5321 specifies the client should never just disconnect, but always send an SMTP QUIT command before disconnecting.
        /// This method is invoked after the SMTP QUIT command is sent and actually disconnects the SMTP client.
        /// </summary>
        /// <param name="asyncResult"></param>
        private static void SendQuitCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            EmailValidatorSession session;

            session = (EmailValidatorSession)asyncResult.AsyncState;

            try
            {

                // End call to EndSendCommand() and start disconnecting the client.
                session.SmtpClient.EndSendCommand(asyncResult);
                session.SmtpClient.BeginDisconnect(DisconnectCallback, session);
            }
            catch (Exception ex)
            {

                // When an exception is caught, initially try to move to the next MX server. If that fails as well
                // complete the validation using the initally caught exception.
                try
                {
                    session.CurrentMxServer.State = EmailValidatorMxServerState.Failed;
                    NextMxServer(session);
                }
                catch (Exception)
                {
                    session.Complete(ex, false);
                }
            }
        }

        private static void DisconnectCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            EmailValidatorSession session;

            session = (EmailValidatorSession)asyncResult.AsyncState;

            try
            {

                // End call to Disconnect() and move to the next MX server.
                session.SmtpClient.EndDisconnect(asyncResult);
            }
            finally
            {
                NextMxServer(session);
            }
        }

        public static EmailValidatorResult EndDeepEmailValidation(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<EmailValidatorResult> asyncValidationResult;

            asyncValidationResult = (AsyncResult<EmailValidatorResult>)asyncResult;
            return asyncValidationResult.EndInvoke();
        }
    }

    
}
