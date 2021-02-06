using System;

namespace TicketProvider.Net.Dns
{

    #region RFC1035 Specification

    /*
3.2. RR definitions

3.2.1. Format

All RRs have the same top level format shown below:

                                    1  1  1  1  1  1
      0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                                               |
    /                                               /
    /                      NAME                     /
    |                                               |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                      TYPE                     |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                     CLASS                     |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                      TTL                      |
    |                                               |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                   RDLENGTH                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
    /                     RDATA                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+


where:

NAME            an owner name, i.e., the name of the node to which this
                resource record pertains.

TYPE            two octets containing one of the RR TYPE codes.

CLASS           two octets containing one of the RR CLASS codes.

TTL             a 32 bit signed integer that specifies the time interval
                that the resource record may be cached before the source
                of the information should again be consulted.  Zero
                values are interpreted to mean that the RR can only be
                used for the transaction in progress, and should not be
                cached.  For example, SOA records are always distributed
                with a zero TTL to prohibit caching.  Zero values can
                also be used for extremely volatile data.

RDLENGTH        an unsigned 16 bit integer that specifies the length in
                octets of the RDATA field.

RDATA           a variable length string of octets that describes the
                resource.  The format of this information varies
                according to the TYPE and CLASS of the resource record.
     */
    #endregion

    /// <summary>
    /// The ResourceRecord class represents a resource record as defined in the RFC1034 specifications section 3.6.
    /// </summary>
    /// <remarks>
    /// See http://www.ietf.org/rfc/rfc1034.txt section 3.6 for description.
    /// See http://www.ietf.org/rfc/rfc1035.txt section 3.2.1 for format specification.
    /// </remarks>
    public abstract class ResourceRecord
    {

        #region Variables

        private string _name;
        private ResourceRecordClass _class;
        private uint _ttl;
        private DateTime _created;
        private ushort _rDataLength; 

        #endregion

        #region Properties - RFC1034

        /// <summary>
        /// Gets / sets the domain name where this resource record is found.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets / sets the type of this resource record.
        /// </summary>
        public abstract ResourceRecordType Type { get; }

        /// <summary>
        /// Gets / sets the protocol family or protocol instance.
        /// </summary>
        public ResourceRecordClass Class
        {
            get { return _class; }
            set { _class = value; }
        }

        /// <summary>
        /// Gets / sets the time span for how long this resource record can be cached before it should be discarded.
        /// </summary>
        public int TTL
        {
            get { return (int)_ttl; }
            set { _ttl = (uint)value; }
        }

        /// <summary>
        /// Gets / sets the size of the RDATA of the resource record represented by this ResourceRecord class instance.
        /// </summary>
        public int RDataLength
        {
            get { return _rDataLength; }
            set { _rDataLength = (ushort)value; }
        }

        /*
        /// <summary>
        /// Gets / sets the RDATA of the resource record represented by this ResourceRecord class instance.
        /// </summary>
        public abstract byte[] RData { get; set;}
         * */

        #endregion

        #region Properties

        /// <summary>
        /// Gets the age of this representation of the resource record.
        /// </summary>
        public TimeSpan Age
        {
            get { return DateTime.Now - _created; }
        }

        /// <summary>
        /// Gets if this resource record's TTL has expired and the record should be discarded.
        /// </summary>
        public bool IsExpired
        {
            get { return (Age.TotalSeconds > _ttl); }
        }

        /// <summary>
        /// Gets / sets the time stamp at which this representation of the resource record was created.
        /// </summary>
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the ResourceRecord type.
        /// </summary>
        private ResourceRecord()
        {
            _created = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the ResourceRecord class from serialized data. The domain name
        /// is supplied ahead since the ResponseReader should already have read that value in order to be
        /// able to determine the resource record type.
        /// </summary>
        /// <param name="reader"></param>
        public ResourceRecord(ResponseReader reader, string domainName)
            : this()
        {
            _name = domainName;
            _class = (ResourceRecordClass)reader.ReadUInt16();
            _ttl = reader.ReadUInt32();
            _rDataLength = reader.ReadUInt16();
        }

        public override string ToString()
        {
            return string.Format("{0,-32} {1,-10} {2,-10} {3,-10}", _name, _ttl, _class, Type);
        }

        #endregion
    }
}
