using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns.Records
{

    #region RFC1035 Specification

/*
3.3.1. CNAME RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                     CNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

CNAME           A <domain-name> which specifies the canonical or primary
                name for the owner.  The owner name is an alias.

CNAME RRs cause no additional section processing, but name servers may
choose to restart the query at the canonical name in certain cases.  See
the description of name server logic in [RFC-1034] for details.
*/

    #endregion

    public class CNAMERecord : ResourceRecord
    {

        #region Variables

        private string _cName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the canonical or primary name for the owner.
        /// </summary>
        public string CName
        {
            get { return _cName; }
            set { _cName = value; }
        }

        public override ResourceRecordType Type
        {
            get { return ResourceRecordType.CNAME; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the CNAMERecord class from serialized data.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="domainName"></param>
        public CNAMERecord(ResponseReader reader, string domainName)
            : base(reader, domainName)
        {

            _cName = reader.ReadDomainName();
        }

        public override string ToString()
        {
            return string.Format("{0,-32} {1,-10} {2,-10} {3,-10} {4}", Name, TTL, Class, Type, _cName);
        }

        #endregion
    }
}
