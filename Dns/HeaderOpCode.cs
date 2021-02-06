using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns
{

    public enum HeaderOpCode
    {

        /// <summary>
        /// A standard query (QUERY)
        /// </summary>
        Query = 0,

        /// <summary>
        /// An inverse query (IQUERY)
        /// </summary>
        IQuery = 1,

        /// <summary>
        /// A server status request (STATUS)
        /// </summary>
        Status = 2
    }
}
