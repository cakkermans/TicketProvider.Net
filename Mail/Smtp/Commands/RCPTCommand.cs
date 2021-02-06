using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{
    #region RFC 5321 Specification

    /*
4.1.1.3. RECIPIENT (RCPT)


   This command is used to identify an individual recipient of the mail
   data; multiple recipients are specified by multiple uses of this
   command.  The argument clause contains a forward-path and may contain
   optional parameters.

   The forward-path normally consists of the required destination
   mailbox.  Sending systems SHOULD NOT generate the optional list of
   hosts known as a source route.  Receiving systems MUST recognize
   source route syntax but SHOULD strip off the source route
   specification and utilize the domain name associated with the mailbox
   as if the source route had not been provided.

   Similarly, relay hosts SHOULD strip or ignore source routes, and
   names MUST NOT be copied into the reverse-path.  When mail reaches
   its ultimate destination (the forward-path contains only a
   destination mailbox), the SMTP server inserts it into the destination
   mailbox in accordance with its host mail conventions.

   This command appends its forward-path argument to the forward-path
   buffer; it does not change the reverse-path buffer nor the mail data
   buffer.

   For example, mail received at relay host xyz.com with envelope
   commands

      MAIL FROM:<userx@y.foo.org>
      RCPT TO:<@hosta.int,@jkl.org:userc@d.bar.org>

   will normally be sent directly on to host d.bar.org with envelope
   commands

      MAIL FROM:<userx@y.foo.org>
      RCPT TO:<userc@d.bar.org>

   As provided in Appendix C, xyz.com MAY also choose to relay the
   message to hosta.int, using the envelope commands

      MAIL FROM:<userx@y.foo.org>
      RCPT TO:<@hosta.int,@jkl.org:userc@d.bar.org>

   or to jkl.org, using the envelope commands

      MAIL FROM:<userx@y.foo.org>
      RCPT TO:<@jkl.org:userc@d.bar.org>

   Attempting to use relaying this way is now strongly discouraged.
   Since hosts are not required to relay mail at all, xyz.com MAY also
   reject the message entirely when the RCPT command is received, using
   a 550 code (since this is a "policy reason").

   If service extensions were negotiated, the RCPT command may also
   carry parameters associated with a particular service extension
   offered by the server.  The client MUST NOT transmit parameters other
   than those associated with a service extension offered by the server
   in its EHLO response.

   Syntax:

      rcpt = "RCPT TO:" ( "<Postmaster@" Domain ">" / "<Postmaster>" /
                  Forward-path ) [SP Rcpt-parameters] CRLF

                  Note that, in a departure from the usual rules for
                  local-parts, the "Postmaster" string shown above is
                  treated as case-insensitive.

     */

    #endregion

    /// <summary>
    /// The RCPTCommand class represents a RCPT SMTP command which is used to identify an individual recipient 
    /// of the mail data; multiple recipients are specified by multiple uses of this command.
    /// </summary>
    public class RCPTCommand : SmtpCommand
    {

        #region Variables

        private EmailAddress _forwardPath;
        private string _parameters;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.RCPT; }
        }

        /// <summary>
        /// Gets / sets the forward-path for the mail to be sent.
        /// </summary>
        public EmailAddress ForwardPath
        {
            get { return _forwardPath; }
            set { _forwardPath = value; }
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
        /// Initializes a new instance of the RCPTCommand class.
        /// </summary>
        public RCPTCommand()
        {

            Command = "RCPT";
        }

        /// <summary>
        /// Initializes a new instance of the RCPTCommand class with the specified forward-path.
        /// </summary>
        /// <param name="reversePath"></param>
        public RCPTCommand(EmailAddress forwardPath)
            : this()
        {

            _forwardPath = forwardPath;
        }

        public override void Deserialize(Stream stream)
        {

            char[] buffer;
            StringBuilder sb;

            using (TextReader textReader = new StreamReader(stream))
            {

                // Read "FROM:" and check for syntax errors.
                buffer = new char[4];
                textReader.Read(buffer, 0, 4);
                if (new string(buffer) != "TO:<")
                    throw new SmtpException(SmtpExceptionType.ParameterSyntaxError, "Invalid parameter syntax. Expected 'TO:<'.");

                // Read and parse email address.
                sb = new StringBuilder();
                while (true)
                {

                    int c;

                    c = textReader.Read();
                    if (c < 0)
                        throw new SmtpException(SmtpExceptionType.ParameterSyntaxError, "Forward-path parameter not complete. Expected to encounter '<' character.");
                    if (c == (int)'>')
                        break;

                    sb.Append((char)c);
                }
                EmailAddress.TryParse(sb.ToString(), out _forwardPath);

                // Read command parameters.
                if (textReader.Read() > -1)
                    _parameters = textReader.ReadToEnd();
            }
        }

        public override void Serialize(Stream stream)
        {

            using (TextWriter textWriter = new StreamWriter(stream))
            {

                textWriter.Write(Command);
                textWriter.Write(" TO:<");

                // Write the mail reverse path.
                textWriter.Write(_forwardPath.Email);
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
