using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TicketProvider.Net.Mail.Smtp.Commands;

namespace TicketProvider.Net.Mail.Smtp
{

    /// <summary>
    /// The SmtpCommandDeserializer deserializes SMTP command into concrete SmtpCommand class instances.
    /// </summary>
    public class SmtpCommandDeserializer : IDisposable
    {

        #region Variables

        private Stream _stream;
        private SmtpSessionMode _mode;

        #endregion

        #region Properties

        public SmtpSessionMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the SmtpCommandDeserializer class deserializing SMTP command from the passed byte array.
        /// </summary>
        /// <param name="data"></param>
        public SmtpCommandDeserializer(byte[] data)
        {

            _stream = new MemoryStream(data);
        }

        /// <summary>
        /// Initializes a new instance of the SmtpCommandDeserializer class deserializing SMTP command from the passed stream.
        /// </summary>
        /// <param name="stream"></param>
        public SmtpCommandDeserializer(Stream stream)
        {

            _stream = stream;
        }

        public void Dispose()
        {
            if (_stream != null)
                _stream.Dispose();
        }

        /// <summary>
        /// Deserializes and returns the SmtpCommand.
        /// </summary>
        /// <returns></returns>
        public SmtpCommand Deserialize()
        {
            return null;
            //return Deserialize(_stream, _mode);
        }

        /// <summary>
        /// Deserializes and returns the SmtpCommand contained in the passed data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SmtpCommand Deserialize(byte[] data, SmtpSessionMode mode)
        {
            using (MemoryStream memStream = new MemoryStream(data))
                return Deserialize(memStream, mode);
        }

        /// <summary>
        /// Deserializes a SMTP command from the passed Stream and returns the SmtpCommand.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SmtpCommand Deserialize(MemoryStream stream, SmtpSessionMode mode)
        {

            // Declare variables
            StringBuilder commandBuilder;
            SmtpCommand command;

            // If the SmtpServerSession is in Data mode, the whole of the read message is
            // data content. Return a DATAContentCommand accordingly.
            if (mode == SmtpSessionMode.Data)
                return new DATAContentCommand(stream.ToArray());

            // Read characters until a SP character is encountered, or the end of the stream
            // is reached.
            commandBuilder = new StringBuilder();
            while (true)
            {

                int c;

                c = stream.ReadByte();
                if (c == (int)' ' || c == -1 || c == (int)'\r' || c == (int)'n' || c == 0)
                    break;
                commandBuilder.Append((char)c);
            }

            // Determine which command to instantiate and deserialize futher command details.
            switch (commandBuilder.ToString())
            {
                case "DATA":
                    command = new DATACommand();
                    break;
                case "EHLO":
                    command = new EHLOCommand();
                    break;
                case "EXPN":
                    command = new EXPNCommand();
                    break;
                case "HELO":
                    command = new HELOCommand();
                    break;
                case "HELP":
                    command = new HELPCommand();
                    break;
                case "MAIL":
                    command = new MAILCommand();
                    break;
                case "NOOP":
                    command = new NOOPCommand();
                    break;
                case "QUIT":
                    command = new QUITCommand();
                    break;
                case "RCPT":
                    command = new RCPTCommand();
                    break;
                case "RSET":
                    command = new RSETCommand();
                    break;
                case "VRFY":
                    command = new VRFYCommand();
                    break;
                default:
                    command = new InvalidUnknownCommand();
                    break;
            }

            command.Deserialize(stream);
            return command;
        }

        #endregion
    }
}
