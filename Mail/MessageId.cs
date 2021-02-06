using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TicketProvider.Net.Mail
{
    public class MessageId
    {

        private string _id;
        private string _domain;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public MessageId()
        {
        }

        public MessageId(string id, string domain)
        {

            _id = id;
            _domain = domain;
        }

        public override string ToString()
        {
            return _id + "@" + _domain;
        }

        public static bool TryParse(string value, out MessageId messageId)
        {

            string[] pieces;

            messageId = null;
            pieces = value.Split('@');
            if (pieces.Length != 2)
                return false;
            messageId = new MessageId(pieces[0], pieces[1]);
            return true;
        }

        /// <summary>
        /// Generates a new MessageId class instance with a GUID as local part and the hostname of the current computer.
        /// </summary>
        /// <returns></returns>
        public static MessageId GenerateNew()
        {
            return new MessageId(Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + ".TicketProviderSystem", 
                System.Net.Dns.GetHostName());
        }
    }
}
