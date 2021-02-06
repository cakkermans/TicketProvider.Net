using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail
{

    #region RFC 5322
    /*
3.4.1.  Addr-Spec Specification

   An addr-spec is a specific Internet identifier that contains a
   locally interpreted string followed by the at-sign character ("@",
   ASCII value 64) followed by an Internet domain.  The locally
   interpreted string is either a quoted-string or a dot-atom.  If the
   string can be represented as a dot-atom (that is, it contains no
   characters other than atext characters or "." surrounded by atext
   characters), then the dot-atom form SHOULD be used and the quoted-
   string form SHOULD NOT be used.  Comments and folding white space
   SHOULD NOT be used around the "@" in the addr-spec.

      Note: A liberal syntax for the domain portion of addr-spec is
      given here.  However, the domain portion contains addressing
      information specified by and used in other protocols (e.g.,
      [RFC1034], [RFC1035], [RFC1123], [RFC5321]).  It is therefore
      incumbent upon implementations to conform to the syntax of
      addresses for the context in which they are used.






Resnick                     Standards Track                    [Page 17]
 
RFC 5322                Internet Message Format             October 2008


   addr-spec       =   local-part "@" domain

   local-part      =   dot-atom / quoted-string / obs-local-part

   domain          =   dot-atom / domain-literal / obs-domain

   domain-literal  =   [CFWS] "[" *([FWS] dtext) [FWS] "]" [CFWS]

   dtext           =   %d33-90 /          ; Printable US-ASCII
                       %d94-126 /         ;  characters not including
                       obs-dtext          ;  "[", "]", or "\"

   The domain portion identifies the point to which the mail is
   delivered.  In the dot-atom form, this is interpreted as an Internet
   domain name (either a host name or a mail exchanger name) as
   described in [RFC1034], [RFC1035], and [RFC1123].  In the domain-
   literal form, the domain is interpreted as the literal Internet
   address of the particular host.  In both cases, how addressing is
   used and how messages are transported to a particular host is covered
   in separate documents, such as [RFC5321].  These mechanisms are
   outside of the scope of this document.

   The local-part portion is a domain-dependent string.  In addresses,
   it is simply interpreted on the particular host as a name of a
   particular mailbox.*/

    #endregion

    public class EmailAddressCollection : Collection<EmailAddress>
    {


        public override string ToString()
        {

            using (TextWriter writer = new StringWriter())
            {
                for (int i = 0; i < Count; i++)
                {
                    if (i > 0)
                        writer.Write(',');
                    writer.Write(this[i]);
                }

                return writer.ToString();
            }
        }

        /*
        public static bool TryParse(string value, EmailAddressCollection collection)
        {
            return false;
        }*/
    }
}
