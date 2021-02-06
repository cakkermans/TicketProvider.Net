using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace TicketProvider.Net.Dns
{

    #region RFC1035 Specification

    /*
4.1.1. Header section format

The header contains the following fields:

                                    1  1  1  1  1  1
      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                      ID                       |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    QDCOUNT                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ANCOUNT                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    NSCOUNT                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ARCOUNT                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ID              A 16 bit identifier assigned by the program that
                generates any kind of query.  This identifier is copied
                the corresponding reply and can be used by the requester
                to match up replies to outstanding queries.

QR              A one bit field that specifies whether this message is a
                query (0), or a response (1).

OPCODE          A four bit field that specifies kind of query in this
                message.  This value is set by the originator of a query
                and copied into the response.  The values are:

                0               a standard query (QUERY)

                1               an inverse query (IQUERY)

                2               a server status request (STATUS)

                3-15            reserved for future use

AA              Authoritative Answer - this bit is valid in responses,
                and specifies that the responding name server is an
                authority for the domain name in question section.

                Note that the contents of the answer section may have
                multiple owner names because of aliases.  The AA bit
                corresponds to the name which matches the query name, or
                the first owner name in the answer section.

TC              TrunCation - specifies that this message was truncated
                due to length greater than that permitted on the
                transmission channel.

RD              Recursion Desired - this bit may be set in a query and
                is copied into the response.  If RD is set, it directs
                the name server to pursue the query recursively.
                Recursive query support is optional.

RA              Recursion Available - this be is set or cleared in a
                response, and denotes whether recursive query support is
                available in the name server.

Z               Reserved for future use.  Must be zero in all queries
                and responses.

RCODE           Response code - this 4 bit field is set as part of
                responses.  The values have the following
                interpretation:

                0               No error condition

                1               Format error - The name server was
                                unable to interpret the query.

                2               Server failure - The name server was
                                unable to process this query due to a
                                problem with the name server.

                3               Name Error - Meaningful only for
                                responses from an authoritative name
                                server, this code signifies that the
                                domain name referenced in the query does
                                not exist.

                4               Not Implemented - The name server does
                                not support the requested kind of query.

                5               Refused - The name server refuses to
                                perform the specified operation for
                                policy reasons.  For example, a name
                                server may not wish to provide the
                                information to the particular requester,
                                or a name server may not wish to perform
                                a particular operation (e.g., zone
                                transfer) for particular data.

                6-15            Reserved for future use.

QDCOUNT         an unsigned 16 bit integer specifying the number of
                entries in the question section.

ANCOUNT         an unsigned 16 bit integer specifying the number of
                resource records in the answer section.

NSCOUNT         an unsigned 16 bit integer specifying the number of name
                server resource records in the authority records
                section.

ARCOUNT         an unsigned 16 bit integer specifying the number of
                resource records in the additional records section.
*/
    #endregion

    /// <summary>
    /// The Header class represents the header in a DNS message.
    /// </summary>
    /// <remarks>
    /// Upon instantiation, the header is automatically assigned a cryptographically secure message ID.
    /// </remarks>
    public class Header
    {

        #region Variables

        private ushort _id;
        private ushort _flags;
        private ushort _qdCount;
        private ushort _anCount;
        private ushort _nsCount;
        private ushort _arCount;

        private static RandomNumberGenerator _random;
        private static object _randomLock;

        #endregion

        #region Properties - RFC1035

        /// <summary>
        /// Gets / sets the message identifier.
        /// </summary>
        /// <remarks>
        /// The id is a 16 bit identifier assigned by the program that
        /// generates any kind of query.  This identifier is copied
        /// the corresponding reply and can be used by the requester
        /// to match up replies to outstanding queries.
        /// </remarks>
        public short Id
        {
            get { return (short)_id; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("The supplied value does not fit in an 16 bit unsigned integer.");
                _id = (ushort)value;
            }
        }

        /// <summary>
        /// Gets / sets if this message this header is part of is a query. If false, this message represents a response.
        /// </summary>
        public bool IsQuery
        {
            get { return GetFlags(0, 1) == 0; }
            set
            {
                if (value)
                    SetFlags(0, 1, 0);
                else
                    SetFlags(0, 1, 1);
            }
        }

        /// <summary>
        /// Gets / sets the type of query in the message this header is part of.
        /// </summary>
        public HeaderOpCode OpCode
        {
            get { return (HeaderOpCode)GetFlags(1, 4); }
            set { SetFlags(1, 4, (ushort)value); }
        }

        /// <summary>
        /// Gets / sets if the responding name server is an authority for the domain name in question section
        /// </summary>
        /// <remarks>
        /// Note that the contents of the answer section may have 
        /// multiple owner names because of aliases.  The AA bit
        /// corresponds to the name which matches the query name, or
        /// the first owner name in the answer section.
        /// </remarks>
        public bool AuthorativeAnswer
        {
            get { return GetFlags(5, 1) == 1; }
            set
            {
                if (value)
                    SetFlags(5, 1, 1);
                else
                    SetFlags(5, 1, 0);
            }
        }

        /// <summary>
        /// Gets / sets if the message this header is part of was truncated
        /// due to length greater than that permitted on the
        /// transmission channel.
        /// </summary>
        public bool Truncated
        {
            get { return GetFlags(6, 1) == 1; }
            set
            {
                if (value)
                    SetFlags(6, 1, 1);
                else
                    SetFlags(6, 1, 0);
            }
        }

        /// <summary>
        /// Gets / sets if the
        /// the name server should be directed to pursue the query recursively.
        /// Recursive query support is optional.
        /// </summary>
        public bool RecursionDesired
        {
            get { return GetFlags(7, 1) == 1; }
            set
            {
                if (value)
                    SetFlags(7, 1, 1);
                else
                    SetFlags(7, 1, 0);
            }
        }

        /// <summary>
        /// Gets / sets whether recursive query support is
        /// available in the name server.
        /// </summary>
        public bool RecursionAvailable
        {
            get { return GetFlags(8, 1) == 1; }
            set
            {
                if (value)
                    SetFlags(8, 1, 1);
                else
                    SetFlags(8, 1, 0);
            }
        }

        /// <summary>
        /// Gets / sets the response code provided by the name server.
        /// </summary>
        public HeaderResponseCode ResponseCode
        {
            get { return (HeaderResponseCode)GetFlags(1, 4); }
            set { SetFlags(1, 4, (ushort)value); }
        }

        /// <summary>
        /// Gets / sets the number of entries in the question section.
        /// </summary>
        public short QDCount
        {
            get { return (short)_qdCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("The supplied value does not fit in an 16 bit unsigned integer.");
                _qdCount = (ushort)value;
            }
        }

        /// <summary>
        /// Gets / sets the number of resource records in the answer section.
        /// </summary>
        public short ANCount
        {
            get { return (short)_anCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("The supplied value does not fit in an 16 bit unsigned integer.");
                _anCount = (ushort)value;
            }
        }

        /// <summary>
        /// Gets / sets the number of name server resource records in the authority records
        /// section.
        /// </summary>
        public short NSCount
        {
            get { return (short)_nsCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("The supplied value does not fit in an 16 bit unsigned integer.");
                _nsCount = (ushort)value;
            }
        }

        /// <summary>
        /// Gets / sets the number of resource records in the additional records section.
        /// </summary>
        public short ARCount
        {
            get { return (short)_arCount; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("The supplied value does not fit in an 16 bit unsigned integer.");
                _arCount = (ushort)value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the flags for this header.
        /// </summary>
        public ushort Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the Header class.
        /// </summary>
        public Header()
        {

            _id = GetNextMessageId();
            IsQuery = true;
            OpCode = HeaderOpCode.Query;
            RecursionDesired = true;
        }

        /// <summary>
        /// Deserializes an instance of the Header class from the specified ResponseReader class instance.
        /// </summary>
        /// <param name="reader"></param>
        public Header(ResponseReader reader)
            : this()
        {
            Deserialize(reader);
        }

        static Header()
        {

            _random = new RNGCryptoServiceProvider();
            _randomLock = new object();
        }

        /// <summary>
        /// Returns an new securely generated message ID.
        /// </summary>
        /// <returns></returns>
        public static ushort GetNextMessageId()
        {

            byte[] data;

            data = new byte[2];
            lock(_randomLock)
                _random.GetBytes(data);
            return BitConverter.ToUInt16(data, 0);
        }

        private void SetFlags(int start, int length, ushort value)
        {

            // Validate input
            if (start >= 16 || length <= 0 || (length + start > 16))
                throw new ArgumentException("Invalid start and length combination.");

            ushort mask;

            // Prepare mask
            start = 16 - start - length;
            mask = (ushort)((1 << length) - 1);

            // Align value and mask to their position using bit shift
            mask <<= start;
            value <<= start;

            // Clear flags using inverse mask and write the new flags
            _flags &= (ushort)~mask;
            _flags |= (ushort)(mask & value);
        }

        private ushort GetFlags(int start, int length)
        {

            // Validate input
            if (start >= 16 || length <= 0 || (length + start > 16))
                throw new ArgumentException("Invalid start and length combination.");

            ushort mask;
            ushort value;

            // Prepare mask
            start = 16 - start - length;
            mask = (ushort)((1 << length) - 1);

            // Align mask to their position using bit shift and extract the value.
            mask <<= start;
            value = (ushort)(_flags & mask);

            // Align value using bit shift and return it.
            value >>= start;
            return value;
        }

        /// <summary>
        /// Serializes this header into a byte array as specified in RFC1035 for transmission.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {

            List<byte> data;

            data = new List<byte>(24);
            data.AddRange(ConvertShort(_id));
            data.AddRange(ConvertShort(_flags));
            data.AddRange(ConvertShort(_qdCount));
            data.AddRange(ConvertShort(_anCount));
            data.AddRange(ConvertShort(_nsCount));
            data.AddRange(ConvertShort(_arCount));

            return data.ToArray();
        }

        /// <summary>
        /// Deserializes header data from the specified ResponseReader class instance and populates the members of a new
        /// Header class instance with the deserialized data.
        /// </summary>
        /// <param name="reader"></param>
        public void Deserialize(ResponseReader reader)
        {

            _id = reader.ReadUInt16();
            _flags = reader.ReadUInt16();
            _qdCount = reader.ReadUInt16();
            _anCount = reader.ReadUInt16();
            _nsCount = reader.ReadUInt16();
            _arCount = reader.ReadUInt16();
        }

        private byte[] ConvertShort(ushort value)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
        }

        #endregion
    }
}
