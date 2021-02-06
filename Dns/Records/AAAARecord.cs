using System;
using System.Net;
using System.Net.Sockets;

namespace TicketProvider.Net.Dns.Records
{

    /// <summary>
    /// The AAAARecord class represents a record specifying the IPv6 address associated with a domain name.
    /// </summary>
    /// <remarks>
    /// See http://www.ietf.org/rfc/rfc1886.txt for specifications.
    /// </remarks>
    public class AAAARecord : ResourceRecord
    {

        #region Variables

        private IPAddress _ipAddress;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the IPAddress the domain name represented by this AAAARecord resolves to.
        /// </summary>
        public IPAddress IPAddress
        {
            get { return _ipAddress; }
            set
            {
                if (value.AddressFamily != AddressFamily.InterNetworkV6)
                    throw new ArgumentException("The AAAARecord can only hold an IPv6 address.");
                _ipAddress = value;
            }
        }

        public override ResourceRecordType Type
        {
            get { return ResourceRecordType.AAAA; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the AAAARecord class from serialized data.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="domainName"></param>
        public AAAARecord(ResponseReader reader, string domainName)
            : base(reader, domainName)
        {

            _ipAddress = new IPAddress(reader.ReadBytes(16));
        }

        public override string ToString()
        {
            return string.Format("{0,-32} {1,-10} {2,-10} {3,-10} {4}", Name, TTL, Class, Type, _ipAddress);
        }

        #endregion
    }
}
