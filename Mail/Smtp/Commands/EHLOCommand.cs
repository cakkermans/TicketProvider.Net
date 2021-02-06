using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp.Commands
{

    #region RFC 5321 Specification

    /*
4.1.1.1. Extended HELLO (EHLO) or HELLO (HELO)


   These commands are used to identify the SMTP client to the SMTP
   server.  The argument clause contains the fully-qualified domain name
   of the SMTP client, if one is available.  In situations in which the
   SMTP client system does not have a meaningful domain name (e.g., when
   its address is dynamically allocated and no reverse mapping record is
   available), the client SHOULD send an address literal (see
   Section 4.1.3).

   RFC 2821, and some earlier informal practices, encouraged following
   the literal by information that would help to identify the client
   system.  That convention was not widely supported, and many SMTP
   servers considered it an error.  In the interest of interoperability,
   it is probably wise for servers to be prepared for this string to
   occur, but SMTP clients SHOULD NOT send it.

   The SMTP server identifies itself to the SMTP client in the
   connection greeting reply and in the response to this command.

   A client SMTP SHOULD start an SMTP session by issuing the EHLO
   command.  If the SMTP server supports the SMTP service extensions, it
   will give a successful response, a failure response, or an error
   response.  If the SMTP server, in violation of this specification,
   does not support any SMTP service extensions, it will generate an
   error response.  Older client SMTP systems MAY, as discussed above,
   use HELO (as specified in RFC 821) instead of EHLO, and servers MUST
   support the HELO command and reply properly to it.  In any event, a
   client MUST issue HELO or EHLO before starting a mail transaction.

   These commands, and a "250 OK" reply to one of them, confirm that
   both the SMTP client and the SMTP server are in the initial state,
   that is, there is no transaction in progress and all state tables and
   buffers are cleared.

   Syntax:

   ehlo           = "EHLO" SP ( Domain / address-literal ) CRLF

   helo           = "HELO" SP Domain CRLF

   Normally, the response to EHLO will be a multiline reply.  Each line
   of the response contains a keyword and, optionally, one or more
   parameters.  Following the normal syntax for multiline replies, these
   keywords follow the code (250) and a hyphen for all but the last
   line, and the code and a space for the last line.  The syntax for a
   positive response, using the ABNF notation and terminal symbols of
   RFC 5234 [7], is:

   ehlo-ok-rsp    = ( "250" SP Domain [ SP ehlo-greet ] CRLF )
                    / ( "250-" Domain [ SP ehlo-greet ] CRLF
                    *( "250-" ehlo-line CRLF )
                    "250" SP ehlo-line CRLF )
   
   ehlo-greet     = 1*(%d0-9 / %d11-12 / %d14-127)
                    ; string of any characters other than CR or LF

   ehlo-line      = ehlo-keyword *( SP ehlo-param )

   ehlo-keyword   = (ALPHA / DIGIT) *(ALPHA / DIGIT / "-")
                    ; additional syntax of ehlo-params depends on
                    ; ehlo-keyword

   ehlo-param     = 1*(%d33-126)
                    ; any CHAR excluding <SP> and all
                    ; control characters (US-ASCII 0-31 and 127
                    ; inclusive)

   Although EHLO keywords may be specified in upper, lower, or mixed
   case, they MUST always be recognized and processed in a case-
   insensitive manner.  This is simply an extension of practices
   specified in RFC 821 and Section 2.4.

   The EHLO response MUST contain keywords (and associated parameters if
   required) for all commands not listed as "required" in Section 4.5.1
   excepting only private-use commands as described in Section 4.1.5.
   Private-use commands MAY be listed.
     */


    /*
4.1.3. Address Literals


   Sometimes a host is not known to the domain name system and
   communication (and, in particular, communication to report and repair
   the error) is blocked.  To bypass this barrier, a special literal
   form of the address is allowed as an alternative to a domain name.
   For IPv4 addresses, this form uses four small decimal integers
   separated by dots and enclosed by brackets such as [123.255.37.2],
   which indicates an (IPv4) Internet Address in sequence-of-octets
   form.  For IPv6 and other forms of addressing that might eventually
   be standardized, the form consists of a standardized "tag" that
   identifies the address syntax, a colon, and the address itself, in a
   format specified as part of the relevant standards (i.e., RFC 4291
   [8] for IPv6).

   Specifically:

   IPv4-address-literal  = Snum 3("."  Snum)

   IPv6-address-literal  = "IPv6:" IPv6-addr

   General-address-literal  = Standardized-tag ":" 1*dcontent

   Standardized-tag  = Ldh-str
                     ; Standardized-tag MUST be specified in a
                     ; Standards-Track RFC and registered with IANA

   dcontent       = %d33-90 / ; Printable US-ASCII
                  %d94-126 ; excl. "[", "\", "]"

   Snum           = 1*3DIGIT
                  ; representing a decimal integer
                  ; value in the range 0 through 255

   IPv6-addr      = IPv6-full / IPv6-comp / IPv6v4-full / IPv6v4-comp

   IPv6-hex       = 1*4HEXDIG

   IPv6-full      = IPv6-hex 7(":" IPv6-hex)

   IPv6-comp      = [IPv6-hex *5(":" IPv6-hex)] "::"
                  [IPv6-hex *5(":" IPv6-hex)]
                  ; The "::" represents at least 2 16-bit groups of
                  ; zeros.  No more than 6 groups in addition to the
                  ; "::" may be present.

   IPv6v4-full    = IPv6-hex 5(":" IPv6-hex) ":" IPv4-address-literal

   IPv6v4-comp    = [IPv6-hex *3(":" IPv6-hex)] "::"
                  [IPv6-hex *3(":" IPv6-hex) ":"]
                  IPv4-address-literal
                  ; The "::" represents at least 2 16-bit groups of
                  ; zeros.  No more than 4 groups in addition to the
                  ; "::" and IPv4-address-literal may be present.
*/

    #endregion

    /// <summary>
    /// The EHLO command class represents a SMTP extended HELLO command.
    /// </summary>
    public class EHLOCommand : HELOCommand
    {

        #region Variables

        private IPAddress _ipAddress;

        #endregion

        #region Properties

        public override SmtpCommandType Type
        {
            get { return SmtpCommandType.EHLO; }
        }

        /// <summary>
        /// Gets / sets the IP address of the SMTP client used to identify itself. If a host name
        /// is available, that will be used to identify the SMTP client.
        /// </summary>
        public IPAddress IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the EHLOCommand class.
        /// </summary>
        public EHLOCommand()
        {

            Command = "EHLO";
        }

        /// <summary>
        /// Initializes a new instance of the EHLOCommand class identifying the client using the specified
        /// host name.
        /// </summary>
        /// <param name="hostName"></param>
        public EHLOCommand(string hostName)
            : this()
        {

            HostName = hostName;
        }

        /// <summary>
        /// Initializes a new instance of the EHLOCommand class identifying the client using the specified
        /// host name.
        /// </summary>
        /// <param name="hostName"></param>
        public EHLOCommand(IPAddress ipAddress)
            : this()
        {

            _ipAddress = ipAddress;
        }

        public override void Deserialize(Stream stream)
        {

            // Declare variables
            StringBuilder sb;
            char firstChar;
            string tag = null;

            // Read the first character of the address literal.
            firstChar = (char)stream.ReadByte();

            // Read the remainder of the address literal. Append first character if it is not the first character
            // of an IPv4 address literal.
            sb = new StringBuilder();
            if (firstChar != '[')
                sb.Append(firstChar);
            while (true)
            {

                int c;

                // Check if the end of the command has been reached.
                c = stream.ReadByte();
                if (c == -1)
                    break;

                // Check if the previous part of the string was a tag.
                if (c == ':' && tag != null)
                {
                    tag = sb.ToString();
                    sb = new StringBuilder();
                }

                // If the address literal is an IPv4 address, do not append the closing ] bracket.
                if (firstChar == '[' && c == (int)']')
                    continue;

                // If the character is a SP character, the end of the address literal has been reached.
                if (c == (int)' ')
                    break;

                // Append character.
                sb.Append((char)c);
            }

            if (firstChar == '[')
            {
                _ipAddress = IPAddress.Parse(sb.ToString());
            }
            else if (tag != null)
            {
                switch (tag)
                {
                    case "IPv6":
                        _ipAddress = IPAddress.Parse(sb.ToString());
                        break;
                    default:
                        throw new SmtpException(SmtpExceptionType.ParameterSyntaxError, string.Format("Unknown address-literal tag '{0}' on HELO command encoutered", tag));
                }
            }
            else if (sb.Length != 0)
            {
                HostName = sb.ToString();
            }
            else
                throw new SmtpException(SmtpExceptionType.ParameterSyntaxError, "Invalid EHLO address literal.");
        }

        public override void Serialize(Stream stream)
        {

            using (TextWriter textWriter = new StreamWriter(stream))
            {

                textWriter.Write(Command);
                textWriter.Write(" ");

                // Identify the client, use the host name if available.
                if (!string.IsNullOrEmpty(HostName))
                    textWriter.Write(HostName);
                else
                {
                    switch (_ipAddress.AddressFamily)
                    {
                        case AddressFamily.InterNetwork:
                            textWriter.Write("[");
                            textWriter.Write(_ipAddress);
                            textWriter.Write("]");
                            break;
                        case AddressFamily.InterNetworkV6:
                            textWriter.Write("IPv6:");
                            textWriter.Write(_ipAddress);
                            break;
                    }
                }

                textWriter.Write("\r\n");
            }
        }

        #endregion
    }
}
