using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp
{
    public abstract class SmtpCommand
    {

        #region Variables

        private string _command;

        #endregion

        #region Properties

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        /// <summary>
        /// Returns true if the command is part of a local, unregistered SMTP extension.
        /// </summary>
        public bool IsLocalExtension
        {
            get { return _command[0] == 'X'; }
        }

        public abstract SmtpCommandType Type { get; }

        #endregion

        #region Functions

        public virtual void Serialize(Stream stream)
        {

            byte[] buffer;

            buffer = Encoding.ASCII.GetBytes(_command + "\r\n");
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Deserializes the command from the passed TextReader.
        /// </summary>
        /// <remarks>
        /// Implementors should note that the TextReader will be positioned before the first character after the command
        /// string and SP character. The first character read will thus be the first character of the command paramter string.
        /// </remarks>
        /// <param name="stream"></param>
        public abstract void Deserialize(Stream stream);

        #endregion
    }
}
