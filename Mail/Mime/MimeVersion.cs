using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail.Mime
{
    public struct MimeVersion
    {

        private int _major;
        private int _minor;

        public int Major
        {
            get { return _major; }
            set { _major = value; }
        }

        public int Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }
    }
}
