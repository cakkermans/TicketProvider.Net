using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{

    /// <summary>
    /// The DATAContentCommand represents the actual data sent to the SMTP server after the DATA SMTP command.
    /// </summary>
    public class DATAContentCommand : SmtpCommand
    {

        #region Variables

        private byte[] _data;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.DATA_CONTENT; }
        }

        /// <summary>
        /// Gets / sets the data transmitted in encoded format.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the DATAContentCommand class.
        /// </summary>
        public DATAContentCommand()
        {

            Command = string.Empty;
            _data = new byte[0];
        }

        /// <summary>
        /// Initializes a new instance of the DATAContentCommand class with the specified data.
        /// </summary>
        /// <param name="data"></param>
        public DATAContentCommand(byte[] data)
            : this()
        {

            _data = data;
        }

        public override void Deserialize(Stream stream)
        {

            _data = new byte[stream.Length];
            stream.Read(_data, 0, (int)stream.Length);
        }

        public override void Serialize(Stream stream)
        {

            byte[] buffer;

            stream.Write(_data, 0, _data.Length);
            buffer = Encoding.ASCII.GetBytes("\r\n.\r\n");
            stream.Write(buffer, 0, buffer.Length);
        }

        public override string ToString()
        {
            return "{MAIL DATA}";
        }

        #endregion
    }
}
