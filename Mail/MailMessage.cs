using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The MailMessage class represents an email message and exposes a high level email message manipulation interface.
    /// </summary>
    public class MailMessage
    {

        #region Variables

        private NameValueCollection _headers;
        private string _subject;
        private string _body;
        private MessageId _messageId;

        private EmailAddress _senderAddress;
        private EmailAddress _returnPath;
        private EmailAddressCollection _fromAddresses;
        private EmailAddressCollection _toAddresses;
        private EmailAddressCollection _ccAddresses;
        private EmailAddressCollection _bccAddresses;
        private EmailAddressCollection _replyToAddresses;

        private MessageIdCollection _inReplyTo;
        private MessageIdCollection _references;

        internal const string HeaderSubject = "Subject";
        internal const string HeaderDate = "Date";

        internal const string HeaderFrom = "From";
        internal const string HeaderSender = "Sender";
        internal const string HeaderReplyTo = "Reply-To";

        internal const string HeaderTo = "To";
        internal const string HeaderCc = "Cc";
        internal const string HeaderBcc = "Bcc";

        internal const string HeaderMessageId = "Message-ID";
        internal const string HeaderInReplyTo = "In-Reply-To";
        internal const string HeaderReferences = "References";

        internal const string HeaderReturnPath = "Return-Path";

        #endregion

        #region Properties

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public NameValueCollection Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        /// <summary>
        /// Gets / sets the email address of the actual agent responsible for sending the message.
        /// </summary>
        public EmailAddress SenderAddress
        {
            get { return _senderAddress; }
            set { _senderAddress = value; }
        }

        /// <summary>
        /// Gets / sets the email address to return this mail message to if it is undeliverable.
        /// </summary>
        public EmailAddress ReturnPath
        {
            get { return _returnPath; }
            set { _returnPath = value; }
        }

        /// <summary>
        /// Gets / sets the email addresses of the persons and/or systems responsible for writing the message.
        /// </summary>
        public EmailAddressCollection FromAddresses
        {
            get { return _fromAddresses; }
            set { _fromAddresses = value; }
        }

        public EmailAddressCollection ToAddresses
        {
            get { return _toAddresses; }
            set { _toAddresses = value; }
        }

        public EmailAddressCollection CcAddresses
        {
            get { return _ccAddresses; }
            set { _ccAddresses = value; }
        }

        public EmailAddressCollection BccAddresses
        {
            get { return _bccAddresses; }
            set { _bccAddresses = value; }
        }

        public EmailAddressCollection ReplyToAddresses
        {
            get { return _replyToAddresses; }
            set { _replyToAddresses = value; }
        }

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public MessageId MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        public MessageIdCollection InReplyTo
        {
            get { return _inReplyTo; }
            set { _inReplyTo = value; }
        }

        public MessageIdCollection References
        {
            get { return _references; }
            set { _references = value; }
        }

        #endregion

        #region Functions

        public MailMessage()
        {

            _headers = new NameValueCollection();
            _body = string.Empty;
            _subject = string.Empty;

            _fromAddresses = new EmailAddressCollection();
            _replyToAddresses = new EmailAddressCollection();
            _toAddresses = new EmailAddressCollection();
            _ccAddresses = new EmailAddressCollection();
            _bccAddresses = new EmailAddressCollection();
            
            _inReplyTo = new MessageIdCollection();
            _references = new MessageIdCollection();

            _headers[HeaderDate] = new Rfc5422DateTime(DateTime.Now, TimeZone.CurrentTimeZone).ToString();
        }

        /// <summary>
        /// Sets all the headers constructed from properties.
        /// </summary>
        private void PackHeaders()
        {

            _headers[HeaderSubject] = _subject;
            if (_senderAddress != null)
                _headers[HeaderSender] = _senderAddress.ToString();
            if (_fromAddresses.Count > 0)
                _headers[HeaderFrom] = _fromAddresses.ToString();
            if (_replyToAddresses.Count > 0)
                _headers[HeaderReplyTo] = _replyToAddresses.ToString();
            if (_toAddresses.Count > 0)
                _headers[HeaderTo] = _toAddresses.ToString();
            if (_ccAddresses.Count > 0)
                _headers[HeaderCc] = _ccAddresses.ToString();
            if (_bccAddresses.Count > 0)
                _headers[HeaderBcc] = _bccAddresses.ToString();
            if (_messageId != null)
                _headers[HeaderMessageId] = _messageId.ToString();
            if (_inReplyTo.Count > 0)
                _headers[HeaderInReplyTo] = _inReplyTo.ToString();
            if (_references.Count > 0)
                _headers[HeaderReferences] = _references.ToString();

            if (_returnPath != null)
                _headers[HeaderReturnPath] = _returnPath.ToString();
        }

        private void UnpackHeaders()
        {
        }

        public virtual void Serialize(ImfWriter writer)
        {
            PackHeaders();

            foreach (string key in _headers.Keys)
                writer.WriteHeader(key, _headers[key]);

            writer.WriteBody(_body);
        }

        /// <summary>
        /// Returns a list containing the email addresses for all the recipients for this mail message.
        /// </summary>
        /// <returns></returns>
        public List<EmailAddress> GetAllRecipients()
        {

            List<EmailAddress> result;

            result = new List<EmailAddress>();
            result.AddRange(_toAddresses);
            result.AddRange(_ccAddresses);
            result.AddRange(_bccAddresses);

            return result;
        }

        #endregion
    }
}
