using System;
using System.Net;

namespace TicketProvider.Net.Dns.Records
{

    #region RFC1035 Specification

    /*
3.4.1. A RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ADDRESS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ADDRESS         A 32 bit Internet address.

Hosts that have multiple Internet addresses will have multiple A
records.
*/

    #endregion

    public class ARecord : ResourceRecord
    {

        #region Variables

        private IPAddress _ipAddress;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the IPAddress the domain name represented by this ARecord resolves to.
        /// </summary>
        public IPAddress IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        public override ResourceRecordType Type
        {
            get { return ResourceRecordType.A; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the ARecord class from serialized data.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="domainName"></param>
        public ARecord(ResponseReader reader, string domainName)
            : base(reader, domainName)
        {

            _ipAddress = new IPAddress(reader.ReadBytes(4));
        }

        public override string ToString()
        {
            return string.Format("{0,-32} {1,-10} {2,-10} {3,-10} {4}", Name, TTL, Class, Type, _ipAddress);
        }

        #endregion
    }
}
