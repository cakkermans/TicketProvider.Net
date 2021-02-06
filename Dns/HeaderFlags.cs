using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// The HeaderFlags enum value represent the possible flags set in the header bytes 17 to 32.
    /// </summary>
    [Flags]
    public enum HeaderFlags : uint
    {

        /// <summary>
        /// Indicates the message this header is part of is a query.
        /// </summary>
        Query = 0,

        /// <summary>
        /// Indicates the message this header is part of is a response to a query.
        /// </summary>
        Response = 1,


    }
}
