using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TicketProvider.Net.Mail.Smtp
{
    [Serializable]
    public class SmtpException : Exception, ISerializable
    {

        #region Variables

        private SmtpExceptionType _type;
        private SmtpReply _smtpReply;
        private string _smtpRequest;

        #endregion

        #region Properties

        public SmtpExceptionType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Gets / sets the SMTP reply leading to this exception being thrown, if any.
        /// </summary>
        public SmtpReply SmtpReply
        {
            get { return _smtpReply; }
            set { _smtpReply = value; }
        }

        /// <summary>
        /// Gets / sets the string representation of the SMTP reply request to this exception being thrown, if any.
        /// </summary>
        public string SmtpRequest
        {
            get { return _smtpRequest; }
            set { _smtpRequest = value; }
        }

        #endregion

        #region Functions

        public SmtpException()
            : base()
        {
        }

        public SmtpException(SmtpExceptionType type)
            : base()
        {
            _type = type;
        }

        public SmtpException(string message)
            : base(message)
        {
        }

        public SmtpException(SmtpExceptionType type, string message)
            : base(message)
        {
            _type = type;
        }

        public SmtpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public SmtpException(SmtpExceptionType type, string message, Exception innerException)
            : base(message, innerException)
        {
            _type = type;
        }

        public SmtpException(string smtpRequest, SmtpReply smtpReply)
        {

            _smtpReply = smtpReply;
            _smtpRequest = smtpRequest;
        }

        public SmtpException(string message, string smtpRequest, SmtpReply smtpReply)
            : base(message)
        {

            _smtpReply = smtpReply;
            _smtpRequest = smtpRequest;
        }

        public SmtpException(string message, string smtpRequest, SmtpReply smtpReply, Exception innerException)
            : base(message, innerException)
        {

            _smtpReply = smtpReply;
            _smtpRequest = smtpRequest;
        }

        public SmtpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            _type = (SmtpExceptionType)info.GetValue("SmtpExceptionType", typeof(SmtpExceptionType));
            _smtpReply = (SmtpReply)info.GetValue("SmtpReply", typeof(SmtpReply));
            _smtpRequest = info.GetString("SmtpRequest");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("SmtpExceptionType", _type);
            info.AddValue("SmtpReply", _smtpReply);
            info.AddValue("SmtpRequest", _smtpRequest);
        }

        #endregion
    }

    public enum SmtpExceptionType
    {
        GenericError,
        ParameterSyntaxError,
        CommandSyntaxError,
    }
}
