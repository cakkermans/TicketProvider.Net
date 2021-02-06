using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{

    #region RFC 5321 Specification

    /*
4.1.1.8. HELP (HELP)


   This command causes the server to send helpful information to the
   client.  The command MAY take an argument (e.g., any command name)
   and return more specific information as a response.

   This command has no effect on the reverse-path buffer, the forward-
   path buffer, or the mail data buffer, and it may be issued at any
   time.

   SMTP servers SHOULD support HELP without arguments and MAY support it
   with arguments.

   Syntax:

      help = "HELP" [ SP String ] CRLF


     */

    #endregion

    /// <summary>
    /// The HELPCommand class represents a SMTP HELP command.
    /// </summary>
    public class HELPCommand : SmtpCommand
    {

        #region Variables

        private string _argument;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.HELP; }
        }

        /// <summary>
        /// Gets / sets the optional help argument.
        /// </summary>
        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the HELPCommand class.
        /// </summary>
        public HELPCommand()
        {

            Command = "HELP";
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

                // Write the mailing list name to verify.
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
