using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The SerializerEmailMessage class represents an email message in it's serialized form, containing the raw content, sender address 
    /// and all the receipient addresses. The SerializedEmailMessage basically is how the email message is received from the SMTP server.
    /// </summary>
    public class SerializedEmailMessage
    {

        #region Variables

        private string _content;
        private List<EmailAddress> _receipients;
        private EmailAddress _sender;

        #endregion

        #region Propeties

        /// <summary>
        /// Gets / sets the raw content of the this email message.
        /// </summary>
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// Gets / sets the list containing all the receipients of this email message.
        /// </summary>
        public List<EmailAddress> Receipients
        {
            get { return _receipients; }
            set { _receipients = value; }
        }

        /// <summary>
        /// Gets / sets the send of this email message.
        /// </summary>
        public EmailAddress Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the SerializerEmailMessage class.
        /// </summary>
        public SerializedEmailMessage()
        {

            _receipients = new List<EmailAddress>();
        }

        #endregion
    }
}
