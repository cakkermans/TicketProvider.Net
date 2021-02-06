using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{

    /// <summary>
    /// The InvalidUnknownCommand represents a SMTP command which is unknown or invalid.
    /// </summary>
    public class InvalidUnknownCommand : SmtpCommand
    {

        #region Variables

        private string _data;
        private SmtpCommandType _type;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets / sets the raw command data.
        /// </summary>
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        #region Functions

        public override void Deserialize(Stream stream)
        {

            using(TextReader textReader = new StreamReader(stream))
                _data = textReader.ReadToEnd();
        }

        public override void Serialize(Stream stream)
        {

            using(TextWriter textWriter = new StreamWriter(stream))
                textWriter.Write(_data);
        }

        public void SetType(SmtpCommandType type)
        {
            _type = type;
        }

        #endregion
    }
}
