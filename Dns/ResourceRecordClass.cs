using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// http://www.iana.org/assignments/dns-parameters
    /// </remarks>
    public enum ResourceRecordClass : ushort
    {

        /// <summary>
        /// Reserved [RFC5395]
        /// </summary>
        Reserved0 = 0,

        /// <summary>
        /// Internet (IN) [RFC1035]
        /// </summary>
        IN = 1,

        /// <summary>
        /// Chaos (CH) [Moon1981]
        /// </summary>
        CH = 3,

        /// <summary>
        /// Hesiod (HS) [Dyer1987]
        /// </summary>
        HS = 4,

        /// <summary>
        /// QCLASS NONE [RFC2136]
        /// </summary>
        NONE = 254,

        /// <summary>
        /// QCLASS * (ANY) [RFC1035]
        /// </summary>
        ANY = 255,

    }
}
