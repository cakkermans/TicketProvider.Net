using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail
{
    public class MailMessageHeader
    {

        #region Variables

        private string _name;
        private string _value;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region Functions

        public MailMessageHeader()
        {
        }

        public MailMessageHeader(string name, string value)
        {
            _name = name;
            _value = value;
        }

        #endregion
    }
}
