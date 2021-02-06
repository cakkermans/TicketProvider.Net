using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Mail
{
    public class ImfReader
    {

        private ImfTokenType _tokenType;
        private string _tokenContent;

        public bool Next()
        {
            return false;
        }

        public ImfTokenType TokenType
        {
            get { return _tokenType; }
            protected set { _tokenType = value; }
        }
        public string TokenContent
        {
            get { return _tokenContent; }
            protected set { _tokenContent = value; }
        }
    }

    public enum ImfTokenType
    {
        Comment,
        HeaderName,
        HeaderValue
    }
}
