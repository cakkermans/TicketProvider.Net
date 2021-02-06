using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{

    #region RFC 5321 Specification

    /*
4.1.1.9. NOOP (NOOP)


   This command does not affect any parameters or previously entered
   commands.  It specifies no action other than that the receiver send a
   "250 OK" reply.

   This command has no effect on the reverse-path buffer, the forward-
   path buffer, or the mail data buffer, and it may be issued at any
   time.  If a parameter string is specified, servers SHOULD ignore it.

   Syntax:

      noop = "NOOP" [ SP String ] CRLF

     */

    #endregion

    /// <summary>
    /// The NOOPCommand class represents a SMTP NOOP command.
    /// </summary>
    public class NOOPCommand : SmtpCommand
    {

        #region Variables

        private string _argument;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.NOOP; }
        }

        /// <summary>
        /// Gets / sets the optional argument.
        /// </summary>
        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the NOOPCommand class.
        /// </summary>
        public NOOPCommand()
        {

            Command = "NOOP";
            _argument = string.Empty;
        }

        public override void Deserialize(Stream stream)
        {

            using(TextReader textReader = new StreamReader(stream))
                _argument = textReader.ReadToEnd();
        }

        public override void Serialize(Stream stream)
        {

            using (TextWriter textWriter = new StreamWriter(stream))
            {

                textWriter.Write(Command);

                // Write optional argument
                if (string.IsNullOrEmpty(_argument))
                {
                    textWriter.Write(" ");
                    textWriter.Write(_argument);
                }

                textWriter.Write("\r\n");
            }
        }

        #endregion
    }
}
