using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{
    #region RFC 5321 Specification

    /*
4.1.1.7. EXPAND (EXPN)


   This command asks the receiver to confirm that the argument
   identifies a mailing list, and if so, to return the membership of
   that list.  If the command is successful, a reply is returned
   containing information as described in Section 3.5.  This reply will
   have multiple lines except in the trivial case of a one-member list.

   This command has no effect on the reverse-path buffer, the forward-
   path buffer, or the mail data buffer, and it may be issued at any
   time.

   Syntax:

      expn = "EXPN" SP String CRLF

     */

    #endregion

    /// <summary>
    /// The EXPNCommand class represents a SMTP EXPAND command, asking the receiver to confirm that the argument
    /// identifies a mailing list, and if so, to return the membership of that list.
    /// </summary>
    public class EXPNCommand : SmtpCommand
    {

        #region Variables

        private string _query;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.EXPN; }
        }

        /// <summary>
        /// Gets / sets the mailing list name to verify.
        /// </summary>
        public string Query
        {
            get { return _query; }
            set { _query = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the VRFYCommand class.
        /// </summary>
        public EXPNCommand()
        {

            Command = "EXPN";
            _query = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the VRFYCommand class asking to verify the supplied mailing list name.
        /// </summary>
        /// <param name="hostName"></param>
        public EXPNCommand(string query)
            : this()
        {

            _query = query;
        }

        public override void Deserialize(Stream stream)
        {

            using(TextReader textReader = new StreamReader(stream))
                _query = textReader.ReadToEnd();
        }

        public override void Serialize(Stream stream)
        {

            using (TextWriter textWriter = new StreamWriter(stream))
            {

                textWriter.Write(Encoding.UTF8.GetBytes(Command));
                textWriter.Write(Encoding.UTF8.GetBytes(" "));

                // Write the mailing list name to verify.
                textWriter.Write(Encoding.UTF8.GetBytes(_query));
                textWriter.Write(Encoding.UTF8.GetBytes("\r\n"));
            }
        }

        #endregion
    }
}
