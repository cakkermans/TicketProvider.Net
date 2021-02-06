using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns.Records
{

    /// <summary>
    /// The unknown record represents a resource record of a type that is unknown 
    /// to the TicketProvider.Net.Dns system.
    /// </summary>
    public class UnknownRecord : ResourceRecord
    {

        #region Variables

        private byte[] _data;
        private ResourceRecordType _type;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the raw RDATA for this 
        /// </summary>
        public byte[] RData
        {
            get { return _data; }
            set { _data = value;
            base.RDataLength = _data.Length;}
        }

        /// <summary>
        /// Gets / sets the type of the resource record represented by this UnknownRecord class instance.
        /// </summary>
        public override ResourceRecordType Type
        {
            get { return _type; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the UnknownRecord class from serialized data of the specified type.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="domainName"></param>
        /// <param name="type"></param>
        public UnknownRecord(ResponseReader reader, string domainName, ResourceRecordType type)
            : base(reader, domainName)
        {

            _type = type;
            _data = reader.ReadBytes(RDataLength);
        }

        #endregion
    }
}
