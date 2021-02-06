using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// The DnsException class represents an error originating in the TicketProvider.Net.Dns system.
    /// </summary>
    [Serializable]
    public class DnsException : Exception, ISerializable
    {

        #region Variables

        #endregion

        #region Properties

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the DnsException class.
        /// </summary>
        public DnsException()
        {
        }

        public DnsException(string message)
            : base(message)
        {
        }

        public DnsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DnsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion
    }
}
