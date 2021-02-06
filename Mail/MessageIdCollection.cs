using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail
{
    public class MessageIdCollection : Collection<MessageId>
    {

        #region RFC 5322 3.6.4

        /*
3.6.4.  Identification Fields

   Though listed as optional in the table in section 3.6, every message
   SHOULD have a "Message-ID:" field.  Furthermore, reply messages
   SHOULD have "In-Reply-To:" and "References:" fields as appropriate
   and as described below.

   The "Message-ID:" field contains a single unique message identifier.
   The "References:" and "In-Reply-To:" fields each contain one or more
   unique message identifiers, optionally separated by CFWS.

   The message identifier (msg-id) syntax is a limited version of the
   addr-spec construct enclosed in the angle bracket characters, "<" and
   ">".  Unlike addr-spec, this syntax only permits the dot-atom-text
   form on the left-hand side of the "@" and does not have internal CFWS
   anywhere in the message identifier.

      Note: As with addr-spec, a liberal syntax is given for the right-
      hand side of the "@" in a msg-id.  However, later in this section,
      the use of a domain for the right-hand side of the "@" is
      RECOMMENDED.  Again, the syntax of domain constructs is specified
      by and used in other protocols (e.g., [RFC1034], [RFC1035],
      [RFC1123], [RFC5321]).  It is therefore incumbent upon
      implementations to conform to the syntax of addresses for the
      context in which they are used.

   message-id      =   "Message-ID:" msg-id CRLF

   in-reply-to     =   "In-Reply-To:" 1*msg-id CRLF

   references      =   "References:" 1*msg-id CRLF

   msg-id          =   [CFWS] "<" id-left "@" id-right ">" [CFWS]

   id-left         =   dot-atom-text / obs-id-left

   id-right        =   dot-atom-text / no-fold-literal / obs-id-right

   no-fold-literal =   "[" *dtext "]"

   The "Message-ID:" field provides a unique message identifier that
   refers to a particular version of a particular message.  The
   uniqueness of the message identifier is guaranteed by the host that
   generates it (see below).  This message identifier is intended to be
   machine readable and not necessarily meaningful to humans.  A message
   identifier pertains to exactly one version of a particular message;
   subsequent revisions to the message each receive new message
   identifiers.

      Note: There are many instances when messages are "changed", but
      those changes do not constitute a new instantiation of that
      message, and therefore the message would not get a new message
      identifier.  For example, when messages are introduced into the
      transport system, they are often prepended with additional header
      fields such as trace fields (described in section 3.6.7) and
      resent fields (described in section 3.6.6).  The addition of such
      header fields does not change the identity of the message and
      therefore the original "Message-ID:" field is retained.  In all
      cases, it is the meaning that the sender of the message wishes to
      convey (i.e., whether this is the same message or a different
      message) that determines whether or not the "Message-ID:" field
      changes, not any particular syntactic difference that appears (or
      does not appear) in the message.

   The "In-Reply-To:" and "References:" fields are used when creating a
   reply to a message.  They hold the message identifier of the original
   message and the message identifiers of other messages (for example,
   in the case of a reply to a message that was itself a reply).  The
   "In-Reply-To:" field may be used to identify the message (or
   messages) to which the new message is a reply, while the
   "References:" field may be used to identify a "thread" of
   conversation.

   When creating a reply to a message, the "In-Reply-To:" and
   "References:" fields of the resultant message are constructed as
   follows:

   The "In-Reply-To:" field will contain the contents of the
   "Message-ID:" field of the message to which this one is a reply (the
   "parent message").  If there is more than one parent message, then
   the "In-Reply-To:" field will contain the contents of all of the
   parents' "Message-ID:" fields.  If there is no "Message-ID:" field in
   any of the parent messages, then the new message will have no "In-
   Reply-To:" field.

   The "References:" field will contain the contents of the parent's
   "References:" field (if any) followed by the contents of the parent's
   "Message-ID:" field (if any).  If the parent message does not contain
   a "References:" field but does have an "In-Reply-To:" field
   containing a single message identifier, then the "References:" field
   will contain the contents of the parent's "In-Reply-To:" field
   followed by the contents of the parent's "Message-ID:" field (if
   any).  If the parent has none of the "References:", "In-Reply-To:",
   or "Message-ID:" fields, then the new message will have no
   "References:" field.

      Note: Some implementations parse the "References:" field to
      display the "thread of the discussion".  These implementations
      assume that each new message is a reply to a single parent and
      hence that they can walk backwards through the "References:" field
      to find the parent of each message listed there.  Therefore,
      trying to form a "References:" field for a reply that has multiple
      parents is discouraged; how to do so is not defined in this
      document.

   The message identifier (msg-id) itself MUST be a globally unique
   identifier for a message.  The generator of the message identifier
   MUST guarantee that the msg-id is unique.  There are several
   algorithms that can be used to accomplish this.  Since the msg-id has
   a similar syntax to addr-spec (identical except that quoted strings,
   comments, and folding white space are not allowed), a good method is
   to put the domain name (or a domain literal IP address) of the host
   on which the message identifier was created on the right-hand side of
   the "@" (since domain names and IP addresses are normally unique),
   and put a combination of the current absolute date and time along
   with some other currently unique (perhaps sequential) identifier
   available on the system (for example, a process id number) on the
   left-hand side.  Though other algorithms will work, it is RECOMMENDED
   that the right-hand side contain some domain identifier (either of
   the host itself or otherwise) such that the generator of the message
   identifier can guarantee the uniqueness of the left-hand side within
   the scope of that domain.

   Semantically, the angle bracket characters are not part of the
   msg-id; the msg-id is what is contained between the two angle bracket
   characters.
         */

        #endregion

        public override string ToString()
        {
            using (TextWriter writer = new StringWriter())
            {
                for (int i = 0; i < Count; i++)
                {
                    if (i > 0)
                        writer.Write("\r\n");
                    writer.Write(this[i]);
                }

                return writer.ToString();
            }
        }
    }
}
