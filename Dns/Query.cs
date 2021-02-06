using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TicketProvider.Net.Dns
{

    #region RFC1035 Specification

    /*
4.1.2. Question section format

The question section is used to carry the "question" in most queries,
i.e., the parameters that define what is being asked.  The section
contains QDCOUNT (usually 1) entries, each of the following format:

                                    1  1  1  1  1  1
      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                                               |
    /                     QNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                     QTYPE                     |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                     QCLASS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

QNAME           a domain name represented as a sequence of labels, where
                each label consists of a length octet followed by that
                number of octets.  The domain name terminates with the
                zero length octet for the null label of the root.  Note
                that this field may be an odd number of octets; no
                padding is used.

QTYPE           a two octet code which specifies the type of the query.
                The values for this field include all codes valid for a
                TYPE field, together with some more general codes which
                can match more than one type of RR.

QCLASS          a two octet code that specifies the class of the query.
                For example, the QCLASS field is IN for the Internet.
     */

    #endregion

    /// <summary>
    /// The Query class represents a resource record query as defined in the RFC1034 specifications section 3.7.1.
    /// </summary>
    /// <remarks>
    /// See http://www.ietf.org/rfc/rfc1034.txt section 3.7.1 for details.
    /// </remarks>
    public class Query
    {

        #region Variables

        private string _name;
        private ResourceRecordType _type;
        private ResourceRecordClass _class;

        #endregion

        #region Properties - RFC1034

        /// <summary>
        /// Gets / sets the target domain name to query for (QNAME).
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value.Length > 255)
                    throw new ArgumentException("The length of a domain name is limited to 255 octets per RFC1034 specification.");
                _name = value;
            }
        }

        /// <summary>
        /// Gets / sets the target resource record type to query for (QTYPE).
        /// </summary>
        public ResourceRecordType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Gets / sets the target resource record class to query for (QCLASS).
        /// </summary>
        public ResourceRecordClass Class
        {
            get { return _class; }
            set { _class = value; }
        }

        #endregion

        #region Functions - Constructor

        /// <summary>
        /// Initializes a new instance of the Query class.
        /// </summary>
        public Query()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Query class with the specified parameters.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="recordType"></param>
        /// <param name="recordClass"></param>
        public Query(string name, ResourceRecordType recordType, ResourceRecordClass recordClass)
        {
            _name = name;
            _type = recordType;
            _class = recordClass;
        }

        /// <summary>
        /// Initializes a new instance of the Query class from serialized data.
        /// </summary>
        /// <param name="reader"></param>
        public Query(ResponseReader reader)
        {
            Deserialize(reader);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Serializes the query into a binary format as specified in RFC1035 for transmission.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {

            List<byte> data;

            data = new List<byte>();
            data.AddRange(SerializeName());
            data.AddRange(ConvertShort((ushort)_type));
            data.AddRange(ConvertShort((ushort)_class));

            return data.ToArray();
        }

        private byte[] ConvertShort(ushort value)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
        }

        private byte[] SerializeName()
        {

            // Declare variables
            string name;
            MemoryStream memStream;
            int nameLength;
            byte[] bytes;

            // Check name. If the name is empty, immediately return the root (null) byte. 
            // Make sure the name ends with a dot.
            name = _name;
            if (name == ".")
                return new byte[1];
            if (!name.EndsWith("."))
                name += ".";

            // Iterate over the name string, building the encoded name string.
            using (memStream = new MemoryStream())
            {
                nameLength = name.Length;
                for (int i = 0, fragmentLength = 0, fragmentStart = 0; i < nameLength; i++, fragmentLength++)
                {
                    // Write current fragement and start new when encoutering a dot
                    if (name[i] == '.')
                    {
                        if (fragmentLength > 0)
                        {
                            bytes = Encoding.UTF8.GetBytes(name.Substring(fragmentStart, fragmentLength));
                            memStream.Write(BitConverter.GetBytes((byte)bytes.Length), 0, 1);
                            memStream.Write(bytes, 0, bytes.Length);
                        }
                        fragmentStart = i + 1;
                        fragmentLength = -1;
                    }
                }
                memStream.WriteByte(0);
                return memStream.ToArray();
            }
        }

        public void Deserialize(ResponseReader reader)
        {

            _name = reader.ReadDomainName();
            _type = (ResourceRecordType)reader.ReadUInt16();
            _class = (ResourceRecordClass)reader.ReadUInt16();
        }

        #endregion
    }
}
