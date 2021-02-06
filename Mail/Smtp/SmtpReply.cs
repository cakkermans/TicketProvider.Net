using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp
{

    #region RFC 5321 Specification

    /*
4.2. SMTP Replies


   Replies to SMTP commands serve to ensure the synchronization of
   requests and actions in the process of mail transfer and to guarantee
   that the SMTP client always knows the state of the SMTP server.
   Every command MUST generate exactly one reply.

   The details of the command-reply sequence are described in
   Section 4.3.

   An SMTP reply consists of a three digit number (transmitted as three
   numeric characters) followed by some text unless specified otherwise
   in this document.  The number is for use by automata to determine
   what state to enter next; the text is for the human user.  The three
   digits contain enough encoded information that the SMTP client need
   not examine the text and may either discard it or pass it on to the
   user, as appropriate.  Exceptions are as noted elsewhere in this
   document.  In particular, the 220, 221, 251, 421, and 551 reply codes
   are associated with message text that must be parsed and interpreted
   by machines.  In the general case, the text may be receiver dependent
   and context dependent, so there are likely to be varying texts for
   each reply code.  A discussion of the theory of reply codes is given
   in Section 4.2.1.  Formally, a reply is defined to be the sequence: a
   three-digit code, <SP>, one line of text, and <CRLF>, or a multiline
   reply (as defined in the same section).  Since, in violation of this
   specification, the text is sometimes not sent, clients that do not
   receive it SHOULD be prepared to process the code alone (with or
   without a trailing space character).  Only the EHLO, EXPN, and HELP
   commands are expected to result in multiline replies in normal
   circumstances; however, multiline replies are allowed for any
   command.

   In ABNF, server responses are:

   Greeting       = ( "220 " (Domain / address-literal)
                  [ SP textstring ] CRLF ) /
                  ( "220-" (Domain / address-literal)
                  [ SP textstring ] CRLF
                  *( "220-" [ textstring ] CRLF )
                  "220" [ SP textstring ] CRLF )

   textstring     = 1*(%d09 / %d32-126) ; HT, SP, Printable US-ASCII

   Reply-line     = *( Reply-code "-" [ textstring ] CRLF )
                  Reply-code [ SP textstring ] CRLF

   Reply-code     = %x32-35 %x30-35 %x30-39

   where "Greeting" appears only in the 220 response that announces that
   the server is opening its part of the connection.  (Other possible
   server responses upon connection follow the syntax of Reply-line.)

   An SMTP server SHOULD send only the reply codes listed in this
   document.  An SMTP server SHOULD use the text shown in the examples
   whenever appropriate.

   An SMTP client MUST determine its actions only by the reply code, not
   by the text (except for the "change of address" 251 and 551 and, if
   necessary, 220, 221, and 421 replies); in the general case, any text,
   including no text at all (although senders SHOULD NOT send bare
   codes), MUST be acceptable.  The space (blank) following the reply
   code is considered part of the text.  Whenever possible, a receiver-
   SMTP SHOULD test the first digit (severity indication) of the reply
   code.

   The list of codes that appears below MUST NOT be construed as
   permanent.  While the addition of new codes should be a rare and
   significant activity, with supplemental information in the textual
   part of the response being preferred, new codes may be added as the
   result of new Standards or Standards-Track specifications.
   Consequently, a sender-SMTP MUST be prepared to handle codes not
   specified in this document and MUST do so by interpreting the first
   digit only.

   In the absence of extensions negotiated with the client, SMTP servers
   MUST NOT send reply codes whose first digits are other than 2, 3, 4,
   or 5.  Clients that receive such out-of-range codes SHOULD normally
   treat them as fatal errors and terminate the mail transaction.


[...]


   The reply text may be longer than a single line; in these cases the
   complete text must be marked so the SMTP client knows when it can
   stop reading the reply.  This requires a special format to indicate a
   multiple line reply.

   The format for multiline replies requires that every line, except the
   last, begin with the reply code, followed immediately by a hyphen,
   "-" (also known as minus), followed by text.  The last line will
   begin with the reply code, followed immediately by <SP>, optionally
   some text, and <CRLF>.  As noted above, servers SHOULD send the <SP>
   if subsequent text is not sent, but clients MUST be prepared for it
   to be omitted.

   For example:

      250-First line
      250-Second line
      250-234 Text beginning with numbers
      250 The last line

   In a multiline reply, the reply code on each of the lines MUST be the
   same.  It is reasonable for the client to rely on this, so it can
   make processing decisions based on the code in any line, assuming
   that all others will be the same.  In a few cases, there is important
   data for the client in the reply "text".  The client will be able to
   identify these cases from the current context.


     */

    #endregion

    [Serializable]
    public class SmtpReply
    {

        #region Variables

        private int _code;
        private string _message;
        private bool _isMultiLine;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the code associated with this SmtpReply.
        /// </summary>
        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        /// <summary>
        /// Gets / sets the message associated with this SmtpReply.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Gets / sets if this SmtpReply is part of a multi line response and not the last line of the multi line response.
        /// </summary>
        internal bool IsMultiLine
        {
            get { return _isMultiLine; }
            set { _isMultiLine = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the SmtpReply class.
        /// </summary>
        public SmtpReply()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SmtpReply 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public SmtpReply(int code, string message)
        {

            _code = code;
            _message = message;
        }

        /// <summary>
        /// Serializes the SmtpReply and appends a message boundary.
        /// </summary>
        /// <param name="textWriter"></param>
        public void Serialize(TextWriter textWriter)
        {

            // Write code
            textWriter.Write("{0:000}", _code);

            // Write seperator
            if (_isMultiLine)
                textWriter.Write('-');
            else
                textWriter.Write(' ');

            // Write message
            textWriter.Write(_message);

            // Append message boundary
            textWriter.Write("\r\n");
        }

        public void Deserialize(TextReader textReader)
        {

            // Declare variables
            char[] code;
            char seperator;

            // Read code
            code = new char[3];
            textReader.Read(code, 0, 3);
            _code = int.Parse(new string(code));

            // Read seperator
            seperator = (char)textReader.Read();
            if (seperator == '-')
                _isMultiLine = true;
            else
                _isMultiLine = false;

            // Read message
            _message = textReader.ReadLine();
        }

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                Serialize(writer);
                return writer.ToString();
            }
        }

        #endregion
    }
}
