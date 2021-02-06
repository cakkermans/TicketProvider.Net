using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/*
http://tools.ietf.org/html/rfc5322

2. Lexical Analysis of Messages


2.1. General Description


   At the most basic level, a message is a series of characters.  A
   message that is conformant with this specification is composed of
   characters with values in the range of 1 through 127 and interpreted
   as US-ASCII [ANSI.X3-4.1986] characters.  For brevity, this document
   sometimes refers to this range of characters as simply "US-ASCII
   characters".

      Note: This document specifies that messages are made up of
      characters in the US-ASCII range of 1 through 127.  There are
      other documents, specifically the MIME document series ([RFC2045],
      [RFC2046], [RFC2047], [RFC2049], [RFC4288], [RFC4289]), that
      extend this specification to allow for values outside of that
      range.  Discussion of those mechanisms is not within the scope of
      this specification.

   Messages are divided into lines of characters.  A line is a series of
   characters that is delimited with the two characters carriage-return
   and line-feed; that is, the carriage return (CR) character (ASCII
   value 13) followed immediately by the line feed (LF) character (ASCII
   value 10).  (The carriage return/line feed pair is usually written in
   this document as "CRLF".)



Resnick                     Standards Track                     [Page 6]

 
RFC 5322                Internet Message Format             October 2008


   A message consists of header fields (collectively called "the header
   section of the message") followed, optionally, by a body.  The header
   section is a sequence of lines of characters with special syntax as
   defined in this specification.  The body is simply a sequence of
   characters that follows the header section and is separated from the
   header section by an empty line (i.e., a line with nothing preceding
   the CRLF).

      Note: Common parlance and earlier versions of this specification
      use the term "header" to either refer to the entire header section
      or to refer to an individual header field.  To avoid ambiguity,
      this document does not use the terms "header" or "headers" in
      isolation, but instead always uses "header field" to refer to the
      individual field and "header section" to refer to the entire
      collection.

2.1.1. Line Length Limits


   There are two limits that this specification places on the number of
   characters in a line.  Each line of characters MUST be no more than
   998 characters, and SHOULD be no more than 78 characters, excluding
   the CRLF.

   The 998 character limit is due to limitations in many implementations
   that send, receive, or store IMF messages which simply cannot handle
   more than 998 characters on a line.  Receiving implementations would
   do well to handle an arbitrarily large number of characters in a line
   for robustness sake.  However, there are so many implementations that
   (in compliance with the transport requirements of [RFC5321]) do not
   accept messages containing more than 1000 characters including the CR
   and LF per line, it is important for implementations not to create
   such messages.

   The more conservative 78 character recommendation is to accommodate
   the many implementations of user interfaces that display these
   messages which may truncate, or disastrously wrap, the display of
   more than 78 characters per line, in spite of the fact that such
   implementations are non-conformant to the intent of this
   specification (and that of [RFC5321] if they actually cause
   information to be lost).  Again, even though this limitation is put
   on messages, it is incumbent upon implementations that display
   messages to handle an arbitrarily large number of characters in a
   line (certainly at least up to the 998 character limit) for the sake
   of robustness.







Resnick                     Standards Track                     [Page 7]

 
RFC 5322                Internet Message Format             October 2008


2.2. Header Fields


   Header fields are lines beginning with a field name, followed by a
   colon (":"), followed by a field body, and terminated by CRLF.  A
   field name MUST be composed of printable US-ASCII characters (i.e.,
   characters that have values between 33 and 126, inclusive), except
   colon.  A field body may be composed of printable US-ASCII characters
   as well as the space (SP, ASCII value 32) and horizontal tab (HTAB,
   ASCII value 9) characters (together known as the white space
   characters, WSP).  A field body MUST NOT include CR and LF except
   when used in "folding" and "unfolding", as described in section
   2.2.3.  All field bodies MUST conform to the syntax described in
   sections 3 and 4 of this specification.

2.2.1. Unstructured Header Field Bodies


   Some field bodies in this specification are defined simply as
   "unstructured" (which is specified in section 3.2.5 as any printable
   US-ASCII characters plus white space characters) with no further
   restrictions.  These are referred to as unstructured field bodies.
   Semantically, unstructured field bodies are simply to be treated as a
   single line of characters with no further processing (except for
   "folding" and "unfolding" as described in section 2.2.3).

2.2.2. Structured Header Field Bodies


   Some field bodies in this specification have a syntax that is more
   restrictive than the unstructured field bodies described above.
   These are referred to as "structured" field bodies.  Structured field
   bodies are sequences of specific lexical tokens as described in
   sections 3 and 4 of this specification.  Many of these tokens are
   allowed (according to their syntax) to be introduced or end with
   comments (as described in section 3.2.2) as well as the white space
   characters, and those white space characters are subject to "folding"
   and "unfolding" as described in section 2.2.3.  Semantic analysis of
   structured field bodies is given along with their syntax.

2.2.3. Long Header Fields


   Each header field is logically a single line of characters comprising
   the field name, the colon, and the field body.  For convenience
   however, and to deal with the 998/78 character limitations per line,
   the field body portion of a header field can be split into a
   multiple-line representation; this is called "folding".  The general
   rule is that wherever this specification allows for folding white
   space (not simply WSP characters), a CRLF may be inserted before any
   WSP.




Resnick                     Standards Track                     [Page 8]

 
RFC 5322                Internet Message Format             October 2008


   For example, the header field:

   Subject: This is a test

   can be represented as:

   Subject: This
    is a test

      Note: Though structured field bodies are defined in such a way
      that folding can take place between many of the lexical tokens
      (and even within some of the lexical tokens), folding SHOULD be
      limited to placing the CRLF at higher-level syntactic breaks.  For
      instance, if a field body is defined as comma-separated values, it
      is recommended that folding occur after the comma separating the
      structured items in preference to other places where the field
      could be folded, even if it is allowed elsewhere.

   The process of moving from this folded multiple-line representation
   of a header field to its single line representation is called
   "unfolding".  Unfolding is accomplished by simply removing any CRLF
   that is immediately followed by WSP.  Each header field should be
   treated in its unfolded form for further syntactic and semantic
   evaluation.  An unfolded header field has no length restriction and
   therefore may be indeterminately long.

2.3. Body


   The body of a message is simply lines of US-ASCII characters.  The
   only two limitations on the body are as follows:

   o  CR and LF MUST only occur together as CRLF; they MUST NOT appear
      independently in the body.
   o  Lines of characters in the body MUST be limited to 998 characters,
      and SHOULD be limited to 78 characters, excluding the CRLF.

      Note: As was stated earlier, there are other documents,
      specifically the MIME documents ([RFC2045], [RFC2046], [RFC2049],
      [RFC4288], [RFC4289]), that extend (and limit) this specification
      to allow for different sorts of message bodies.  Again, these
      mechanisms are beyond the scope of this document.
*/

namespace TicketProvider.Net.Mail
{

    /// <summary>
    /// The ImfWriter impelements a TextWriter and transparantly ensures any data written to it is formatted to comply with
    /// section 2 from the RFC5322 internet message format specification.
    /// </summary>
    public class ImfWriter : IDisposable
    {

        #region Variables

        private int _maxLineLength;
        private ImfWriterCompliancyLevel _compliancy;
        private bool _bodyStarted;
        private bool _isMultiPart;

        private TextWriter _output;
        private StringBuilder _currentLine;
        private int _lastFoldingPos;

        private ImfWriterEncoding _encoding;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the RFC5322 compliancy level this ImfWriter should operate on.
        /// </summary>
        public ImfWriterCompliancyLevel CompliancyLevel
        {
            get { return _compliancy; }
            set
            {
                _compliancy = value;
                switch (_compliancy)
                {
                    case ImfWriterCompliancyLevel.Strict:
                        _maxLineLength = 77;
                        break;
                    case ImfWriterCompliancyLevel.Loose:
                        _maxLineLength = 997;
                        break;
                    case ImfWriterCompliancyLevel.None:
                        _maxLineLength = int.MaxValue;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the encoding used to write IMF data.
        /// </summary>
        public ImfWriterEncoding Encoding
        {
            get { return _encoding; }
        }

        public bool IsMultiPart
        {
            get { return _isMultiPart; }
            set { _isMultiPart = value; }
        }

        public bool BodyStarted
        {
            get { return _bodyStarted; }
        }

        #endregion

        #region Functions

        public ImfWriter(Stream stream, ImfWriterEncoding encoding)
        {

            CompliancyLevel = ImfWriterCompliancyLevel.Strict;
            _bodyStarted = false;

            _currentLine = new StringBuilder();
            _lastFoldingPos = -1;
            _encoding = encoding;

            InitializeOutput(stream);
        }

        internal void InitializeOutput(Stream stream)
        {
            switch (_encoding)
            {
                case ImfWriterEncoding.EightBit:
                    _output = new StreamWriter(stream, System.Text.Encoding.UTF8);
                    break;
                case ImfWriterEncoding.SevenBit:
                    _output = new StreamWriter(stream, System.Text.Encoding.ASCII);
                    break;
            }
        }

        public void WriteHeader(string name, string value)
        {

            if (_bodyStarted)
                throw new InvalidOperationException("Cannot write mail message header because the mail body output has already been started.");

            // Write the header name and value as specified by the RFC 5322 specifications.
            Write(name);
            Write(": ");
            Write(value);
            Write("\r\n");
        }

        public void WriteHeaderStart(string name)
        {
            Write(name);
            Write(": ");
        }

        public void WriteHeaderEnd()
        {
            Write("\r\n");
        }

        public void WriteBody(string value)
        {

            // Write the header and body seperator, if it has not yet been written.
            if (!_bodyStarted)
            {
                Write("\r\n");
                _bodyStarted = true;
            }

            Write(value);
        }

        public void Write(EmailAddress emailAddress)
        {
        }

        public void Write(string data)
        {
            for (int i = 0; i < data.Length; i++)
            {

                // Write the current character to the current line.
                _currentLine.Append(data[i]);

                // Check if the current character is a folding oppertunity. If so, store this position as the last
                // folding position.
                if (data[i] == ' ')
                {

                    // When a folding oppertunity is encountered, write all characters up to and including the
                    // WSP to the current line and store the folding oppertunity.
                    _lastFoldingPos = _currentLine.Length - 1;
                }

                // If a CRLF character sequence is encountered, this counts as a line fold. Commit the current line to the
                // output buffer.
                if (i > 0)
                {
                    if (data[i - 1] == '\r'
                        && data[i] == '\n')
                    {
                        _output.Write(_currentLine.ToString());
                        _currentLine.Length = 0;
                        _lastFoldingPos = -1;
                    }
                }

                // If the maximum line length is reached, try to fold at the last known folding possible position. If no last
                // known possilbe folding is availble, force a fold at the current position.
                if (_currentLine.Length >= _maxLineLength)
                {
                    if (_lastFoldingPos > -1)
                    {
                        _output.Write(_currentLine.ToString(0, _lastFoldingPos));
                        _output.Write(" \r\n");
                        _currentLine.Remove(0, _lastFoldingPos + 1);
                        _lastFoldingPos = -1;
                    }
                    else
                    {
                        _output.Write(_currentLine.ToString());
                        _output.Write(" \r\n");
                        _currentLine.Length = 0;
                        _lastFoldingPos = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Writes the last line to the stream.
        /// </summary>
        private void Flush()
        {

            _output.Write(_currentLine.ToString());
            _currentLine.Length = 0;
            _lastFoldingPos = -1;
        }

        public void Dispose()
        {
            Flush();
            _output.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// The ImfWriterCompliancyLevel enum specifies the RFC5332 compliancy level at which the ImfWriter class will behave.
    /// </summary>
    public enum ImfWriterCompliancyLevel
    {
        /// <summary>
        /// The ImfWriter will behave fully compliant to the RFC5332 specifications, wrapping every line at 78 characters. This
        /// ensures compliancy with legacy mail clients, but results in slightly higher file sizes.
        /// </summary>
        Strict,

        /// <summary>
        /// The ImfWriter will behave compliant to the RFC5332 specifications but will not follow all advices, wrapping every 
        /// line at 988 characters.
        /// </summary>
        Loose,

        /// <summary>
        /// The ImfWriter will not behave fully compliant to the RFC5332 specifications.
        /// </summary>
        None
    }

    public enum ImfWriterEncoding
    {

        SevenBit,
        EightBit,
    }
}
