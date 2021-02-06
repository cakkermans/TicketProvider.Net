using System;
using System.Collections.Generic;
using System.Text;
using TicketProvider.Net.Mail.Smtp;
using TicketProvider.Net.Dns;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The EmailValidatorSession class represents an (asynchonous) email validation session executed by an instance of the
    /// EmailValidator class.
    /// </summary>
    public class EmailValidatorSession
    {

        #region Variables

        private AsyncResult<EmailValidatorResult> _asyncResult;

        private EmailAddress _emailAddress;
        private EmailValidatorResult _result;

        private List<EmailValidatorMxServer> _mxServers;
        private IEnumerator<EmailValidatorMxServer> _mxServerEnumerator;
        private ResourceRecordType _mxServerQueryType;

        private SmtpClient _smtpClient;
        private EmailValidatorSessionSmtpState _smtpState;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the email address being validated.
        /// </summary>
        public EmailAddress EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        /// <summary>
        /// Gets / sets the async result for the email validation operation.
        /// </summary>
        public AsyncResult<EmailValidatorResult> AsyncResult
        {
            get { return _asyncResult; }
            set { _asyncResult = value; }
        }

        /// <summary>
        /// Gets the mx server enumerator.
        /// </summary>
        public IEnumerator<EmailValidatorMxServer> MxServerEnumerator
        {
            get { return _mxServerEnumerator; }
        }

        /// <summary>
        /// Gets / sets the DNS resource record type to query for.
        /// </summary>
        public ResourceRecordType MxServerQueryType
        {
            get { return _mxServerQueryType; }
            set {
                if (value != ResourceRecordType.MX && value != ResourceRecordType.A && value != ResourceRecordType.AAAA)
                    throw new ArgumentException("The specified ResourceRecordType is not allowed.");
                _mxServerQueryType = value; }
        }

        /// <summary>
        /// Gets / sets the result of this email validation session.
        /// </summary>
        public EmailValidatorResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        /// <summary>
        /// Gets the current mx server being validated.
        /// </summary>
        public EmailValidatorMxServer CurrentMxServer
        {
            get { return _mxServerEnumerator.Current; }
        }

        /// <summary>
        /// Gets / sets the SMTP client for this email validation session.
        /// </summary>
        public SmtpClient SmtpClient
        {
            get { return _smtpClient; }
            set { _smtpClient = value; }
        }

        public EmailValidatorSessionSmtpState SmtpState
        {
            get { return _smtpState; }
            set { _smtpState = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the EmailValdiatorSession class with the specified EmailAddress and async result.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="asyncResult"></param>
        public EmailValidatorSession(EmailAddress emailAddress, AsyncResult<EmailValidatorResult> asyncResult)
        {

            _emailAddress = emailAddress;
            _asyncResult = asyncResult;
            _result = new EmailValidatorResult(emailAddress);
        }

        /// <summary>
        /// Sets the defined mx servers for the email address being validated to the passed mx servers and
        /// sets up the mx server enumerator.
        /// </summary>
        /// <param name="mxServers"></param>
        public void SetMxServers(IEnumerable<EmailValidatorMxServer> mxServers)
        {

            _mxServers = new List<EmailValidatorMxServer>(mxServers);
            _mxServerEnumerator = _mxServers.GetEnumerator();
        }

        /// <summary>
        /// Sets the next MX server to be the current MX server. Returns false if no more MX servers are available.
        /// </summary>
        /// <returns></returns>
        public bool NextMxServer()
        {

            return _mxServerEnumerator.MoveNext();
        }

        /// <summary>
        /// Completes the asynchonous result for this email validation session.
        /// </summary>
        /// <param name="completedSynchonously"></param>
        public void Complete(bool completedSynchonously)
        {

            _asyncResult.Complete(_result, completedSynchonously);
        }

        /// <summary>
        /// Completes the asynchonous result for this email validation session with the specified exception
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="completedSynchonously"></param>
        public void Complete(Exception ex, bool completedSynchonously)
        {

            _asyncResult.Complete(ex, completedSynchonously);
        }

        #endregion
    }

    /// <summary>
    /// The EmailValidatorSessionSmtpState enum indicates the current state of an smtp session of an email validation session.
    /// </summary>
    public enum EmailValidatorSessionSmtpState
    {

        Init,
        Helo,
        Mail,
        Rcpt,
    }
}
