using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The EmailAddress class represents an email address and optionally an associated name.
    /// </summary>
    public class EmailAddress
    {

        #region Variables

        private string _email;
        private string _name;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the actual mailbox address of this EmailAddress class instance.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// Gets / sets the name associated with this EmailAddress class instance.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the mailbox portion of the email address.
        /// </summary>
        public string MailBox
        {
            get
            {

                string[] pieces;

                pieces = _email.Split('@');
                if (pieces.Length != 2)
                    return string.Empty;
                else 
                    return pieces[0];
            }
        }

        /// <summary>
        /// Gets the domain portion of the email address.
        /// </summary>
        public string Domain
        {
            get
            {

                string[] pieces;

                pieces = _email.Split('@');
                if (pieces.Length != 2)
                    return string.Empty;
                else
                    return pieces[1];
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the EmailAddress class.
        /// </summary>
        public EmailAddress()
        {
        }

        public EmailAddress(string email)
        {

            _email = email;
        }

        public static bool TryParse(string text, out EmailAddress emailAddress)
        {

            int semiColonIndex;

            emailAddress = new EmailAddress();
            semiColonIndex = text.IndexOf(':');
            if (semiColonIndex > -1)
                emailAddress = new EmailAddress(text.Substring(semiColonIndex));
            else
                emailAddress = new EmailAddress(text);

            return true;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_name))
                return _email;
            else
                return string.Format("{0} <{1}>", _name, _email);
        }

        #endregion
    }
}
