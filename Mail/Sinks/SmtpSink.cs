using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TicketProvider.Net.Mail.Smtp;
using TicketProvider.Net.Mail.Smtp.Commands;


namespace TicketProvider.Net.Mail.Sinks
{

    /// <summary>
    /// The SmtpSink class provides a basic message sink for sending email messages to a an actual SMTP server.
    /// </summary>
    public class SmtpSink : IMessageSink
    {

        #region Variables

        private SmtpClient _client;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the IP address of the SMTP server to connect to.
        /// </summary>
        public IPAddress Server
        {
            get { return _client.Server.Address; }
            set { _client.Server.Address = value; }
        }

        /// <summary>
        /// Gets / sets the port of the SMTP server to connect to.
        /// </summary>
        public int Port
        {
            get { return _client.Server.Port; }
            set { _client.Server.Port = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the SmtpSink class.
        /// </summary>
        public SmtpSink()
        {

            _client = new SmtpClient();
        }

        public SmtpSink(string host, int port)
        {

            IPAddress[] address;
            IPEndPoint server;

            address = Dns.DnsResolver.GetHostAddresses(host);
            if (address.Length == 0)
                throw new MailException("Unable to resolve hostname of specified SMTP server.");
            server = new IPEndPoint(address[0], port);
            _client = new SmtpClient(server);
        }

        #endregion

        #region IMessageSink Members

        public void Send(MailMessage message)
        {

            SmtpReply reply;
            MAILCommand mailCmd;
            RCPTCommand rcptCmd;
            DATACommand dataCmd;
            DATAContentCommand dataContentCmd;

            // Ensure connection is open
            if (!IsOpen)
                Open();

            // Send MAIL command
            mailCmd = new MAILCommand(message.ReturnPath);
            reply = _client.SendCommand(mailCmd);
            if (reply.Code != (int)SmtpReplyCode.Completed)
                throw new SmtpException("Unable to deliver message.", mailCmd.ToString(), reply);

            // Set recipients
            foreach (EmailAddress recipient in message.GetAllRecipients())
            {
                rcptCmd = new RCPTCommand(recipient);
                reply = _client.SendCommand(rcptCmd);
                switch (reply.Code)
                {
                    case (int)SmtpReplyCode.Completed:
                        break;
                    case (int)SmtpReplyCode.MailboxUnavailable:
                    case (int)SmtpReplyCode.UserNotLocal:
                    case (int)SmtpReplyCode.MailboxNameNotAllowed:
                        break;
                    default:
                        throw new SmtpException("Unable to deliver message. Unexpected response to RCPT command.", rcptCmd.ToString(), reply);
                }
            }

            // Send DATA command
            dataCmd = new DATACommand();
            reply = _client.SendCommand(dataCmd);
            if (reply.Code != (int)SmtpReplyCode.StartMailInput)
                throw new SmtpException("Unable to deliver message.", dataCmd.ToString(), reply);

            // Send actual message data
            dataContentCmd = new DATAContentCommand(SerializeMessage(message));
            reply = _client.SendCommand(dataContentCmd);
            if (reply.Code != (int)SmtpReplyCode.Completed)
                throw new SmtpException("Unable to deliver message.", reply);
        }

        private byte[] SerializeMessage(MailMessage message)
        {

            MemoryStream memStream;
            ImfWriter writer;

            using (memStream = new MemoryStream())
            {
                using (writer = new ImfWriter(memStream, ImfWriterEncoding.SevenBit))
                {
                    message.Serialize(writer);
                }

                return memStream.GetBuffer();
            }
        }

        public void Open()
        {

            SmtpReply reply;
            EHLOCommand ehloCmd;

            // Open connection
            reply = _client.Connect();
            if (reply.Code != (int)SmtpReplyCode.ServiceReady)
                throw new SmtpException("Unable to open connection to SMTP server. Unexpected welcome message.", reply);

            // Send EHLO command
            ehloCmd = new EHLOCommand(Dns.DnsResolver.GetHostAddresses("localhost")[0]);
            reply = _client.SendCommand(ehloCmd);
            if(reply.Code != (int)SmtpReplyCode.Completed)
                throw new SmtpException("Unable to open connection to SMTP server. Unexpected response to EHLO command.", ehloCmd.ToString(), reply);
        }

        public void Close()
        {
            
            // Send QUIT command
            SmtpReply reply;
            QUITCommand quitCmd;

            // Open connection
            quitCmd = new QUITCommand();
            reply = _client.SendCommand(quitCmd);
            _client.Disconnect();
        }

        public void Dispose()
        {
            if (IsOpen)
                Close();
            _client.Dispose();
        }

        public bool IsOpen
        {
            get { return _client.Connected; }
        }

        #endregion
    }
}
