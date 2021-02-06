using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{

    #region RFC 5321 Specification

    /*
4.1.1.2. MAIL (MAIL)


   This command is used to initiate a mail transaction in which the mail
   data is delivered to an SMTP server that may, in turn, deliver it to
   one or more mailboxes or pass it on to another system (possibly using
   SMTP).  The argument clause contains a reverse-path and may contain
   optional parameters.  In general, the MAIL command may be sent only
   when no mail transaction is in progress, see Section 4.1.4.

   The reverse-path consists of the sender mailbox.  Historically, that
   mailbox might optionally have been preceded by a list of hosts, but
   that behavior is now deprecated (see Appendix C).  In some types of
   reporting messages for which a reply is likely to cause a mail loop
   (for example, mail delivery and non-delivery notifications), the
   reverse-path may be null (see Section 3.6).

   This command clears the reverse-path buffer, the forward-path buffer,
   and the mail data buffer, and it inserts the reverse-path information
   from its argument clause into the reverse-path buffer.

   If service extensions were negotiated, the MAIL command may also
   carry parameters associated with a particular service extension.

   Syntax:

   mail = "MAIL FROM:" Reverse-path
                                       [SP Mail-parameters] CRLF
*/
    #endregion

    public class MAILCommand : SmtpCommand
    {

        #region Variables

        private EmailAddress _reversePath;
        private string _parameters;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.MAIL; }
        }

        /// <summary>
        /// Gets / sets the reverse-path for the mail to be sent, which can be used to report errors.
        /// </summary>
        public EmailAddress ReversePath
        {
            get { return _reversePath; }
            set { _reversePath = value; }
        }

        /// <summary>
        /// Gets / sets optional parameters associated with a particular service extension.
        /// </summary>
        public string Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the MAILCommand class.
        /// </summary>
        public MAILCommand()
        {

            Command = "MAIL";
        }

        /// <summary>
        /// Initializes a new instance of the MAILCommand class with the specified reverse-path.
        /// </summary>
        /// <param name="reversePath"></param>
        public MAILCommand(EmailAddress reversePath)
            : this()
        {

            _reversePath = reversePath;
        }

        public override void Deserialize(Stream stream)
        {
            
            // Declare variables
            char[] buffer;
            char firstChar;
            StringBuilder sb;

            using (TextReader textReader = new StreamReader(stream))
            {

                // Read "FROM:" and check for syntax errors.
                buffer = new char[5];
                textReader.Read(buffer, 0, 5);
                if (new string(buffer) != "FROM:")
                    throw new SmtpException(SmtpExceptionType.ParameterSyntaxError, "Invalid parameter syntax. Expected 'FROM:'.");


                // Read the first character of the reverse path.
                firstChar = (char)textReader.Read();

                // The remainder of the command string should contain the reverse path.
                sb = new StringBuilder();
                if (firstChar != '<')
                    sb.Append(firstChar);
                while (true)
                {

                    int c;

                    // Check if the end of the command has been reached.
                    c = textReader.Read();
                    if (c == -1 || c == (int)' ' || c == 0)
                        break;

                    // If the address literal is an IPv4 address, do not append the closing ] bracket.
                    if (firstChar == '<' && c == (int)'>')
                        continue;

                    // Append character.
                    sb.Append((char)c);
                }

                _reversePath = new EmailAddress(sb.ToString());
            }
        }

        public override void Serialize(Stream stream)
        {

            using (TextWriter textWriter = new StreamWriter(stream))
            {

                textWriter.Write(Command);
                textWriter.Write(" FROM:<");

                // Write the mail reverse path.
                textWriter.Write(_reversePath.Email);
                textWriter.Write(">");

                // Optionally write the parameters.
                if (!string.IsNullOrEmpty(_parameters))
                {
                    textWriter.Write(" ");
                    textWriter.Write(_parameters);
                }

                textWriter.Write("\r\n");
            }
        }

        #endregion
    }
}
