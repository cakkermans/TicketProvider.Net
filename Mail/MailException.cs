using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TicketProvider.Net.Mail
{
    [Serializable]
    public class MailException : Exception, ISerializable
    {

        #region Variables

        #endregion

        #region Properties

        #endregion

        #region Functions

        public MailException()
        {
        }

        public MailException(string message)
            : base(message)
        {
        }

        public MailException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public MailException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion
    }
}
