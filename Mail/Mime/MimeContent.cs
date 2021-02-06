using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace TicketProvider.Net.Mail.Mime
{
    public class MimeContent : IMimeObject
    {

        #region Variables

        private NameValueCollection _headers;
        private string _content;

        internal const string HeaderContentType = "Content-Type";
        internal const string HeaderContentTransferEncoding = "Content-Transfer-Encoding";
        internal const string HeaderContentDisposition = "Content-Disposition";
        internal const string HeaderContentId = "Content-ID";
        internal const string HeaderContentDescription = "Content-Description";
        internal const string HeaderCharset = "Charset";

        #endregion

        #region Properties

        public string ContentType
        {
            get { return _headers[HeaderContentType]; }
            set { _headers[HeaderContentType] = value; }
        }

        public string ContentTransferEncoding
        {
            get { return _headers[HeaderContentTransferEncoding]; }
            set { _headers[HeaderContentTransferEncoding] = value; }
        }

        public string ContentDisposition
        {
            get { return _headers[HeaderContentDisposition]; }
            set { _headers[HeaderContentDisposition] = value; }
        }

        public string ContentId
        {
            get { return _headers[HeaderContentId]; }
            set { _headers[HeaderContentId] = value; }
        }

        public string ContentDescription
        {
            get { return _headers[HeaderContentDescription]; }
            set { _headers[HeaderContentDescription] = value; }
        }

        public string Charset
        {
            get { return _headers[HeaderCharset]; }
            set { _headers[HeaderCharset] = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }

        #endregion

        #region Functions

        public MimeContent()
        {

            _headers = new NameValueCollection();
        }

        #endregion

        #region IMimeObject Members

        public virtual void Serialize(ImfWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
