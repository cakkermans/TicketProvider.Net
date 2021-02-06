using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns.Records
{

    #region RFC1035 Specification

    /*
3.3.11. NS RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   NSDNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

NSDNAME         A <domain-name> which specifies a host which should be
                authoritative for the specified class and domain.

NS records cause both the usual additional section processing to locate
a type A record, and, when used in a referral, a special search of the
zone in which they reside for glue information.

The NS RR states that the named host should be expected to have a zone
starting at owner name of the specified class.  Note that the class may
not indicate the protocol family which should be used to communicate
with the host, although it is typically a strong hint.  For example,
hosts which are name servers for either Internet (IN) or Hesiod (HS)
class information are normally queried using IN class protocols.
*/

    #endregion

    /// <summary>
    /// The NSRecord class represents an NS resource record.
    /// </summary>
    public class NSRecord : ResourceRecord
    {

        #region Variables

        private string _nsDomainName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the host which should be authoritative for the specified class and domain.
        /// </summary>
        public string NsDomainName
        {
            get { return _nsDomainName; }
            set { _nsDomainName = value; }
        }

        public override ResourceRecordType Type
        {
            get { return ResourceRecordType.NS; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new NSRecord class instance from serialized data.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="domainName"></param>
        public NSRecord(ResponseReader reader, string domainName)
            : base(reader, domainName)
        {

            _nsDomainName = reader.ReadDomainName();
        }

        public override string ToString()
        {
            return string.Format("{0,-32} {1,-10} {2,-10} {3,-10} {4}", Name, TTL, Class, Type, _nsDomainName);
        }

        #endregion
    }
}
