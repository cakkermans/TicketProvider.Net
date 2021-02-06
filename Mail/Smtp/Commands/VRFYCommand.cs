using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{
    #region RFC 5321 Specification

    /*
4.1.1.6. VERIFY (VRFY)


   This command asks the receiver to confirm that the argument
   identifies a user or mailbox.  If it is a user name, information is
   returned as specified in Section 3.5.

   This command has no effect on the reverse-path buffer, the forward-
   path buffer, or the mail data buffer.

   Syntax:

      vrfy = "VRFY" SP String CRLF
     */

    #endregion

    /// <summary>
    /// The VRFYCommand class represents a SMTP VERIFY command, asking the receiver to confirm that the argument
    /// identifies a user or mailbox.
    /// </summary>
    public class VRFYCommand : SmtpCommand
    {

        #region Variables

        private string _query;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.VRFY; }
        }

        /// <summary>
        /// Gets / sets the user or mailbox to verify.
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
        public VRFYCommand()
        {

            Command = "VRFY";
            _query = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the VRFYCommand class asking to verify the supplied email address.
        /// </summary>
        /// <param name="hostName"></param>
        public VRFYCommand(string query)
            : this()
        {

            _query = query;
        }

        public override void Deserialize(Stream stream)
        {

            using(TextReader textReader = new StreamReader(stream))
                _query = textReader.ReadLine();
        }

        public override void Serialize(Stream stream)
        {

            using (TextWriter textWriter = new StreamWriter(stream))
            {

                textWriter.Write(Command);
                textWriter.Write(" ");

                // Write the user of mailbox to verify
                if (!string.IsNullOrEmpty(_query))
                    textWriter.Write(_query);

                textWriter.Write("\r\n");
            }
        }

        #endregion
    }
}
