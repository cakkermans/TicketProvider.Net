using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace TicketProvider.Net.Mail.Mime
{
    public class MimeContainer
    {

        #region Variables

        private string _boundary;
        private List<MimeContainer> _children;
        private NameValueCollection _headers;

        internal const string HeaderContentType = "Content-Type";

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the MIME boundary used in this MIME message.
        /// </summary>
        public string Boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        #endregion

        #region Functions

        #endregion
    }
}
