using System;
using System.Collections.Generic;
using System.Text;

namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// The Request class represents a request message sent to a name server and encapsulates all the segments a request message
    /// to a name server contains.
    /// </summary>
    public class Request
    {

        #region Variables

        private Header _header;
        private List<Query> _queries;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the header associated with this request message.
        /// </summary>
        public Header Header
        {
            get { return _header; }
            set { _header = value; }
        }

        /// <summary>
        /// Gets / sets a list containing all the queries this request message contains.
        /// </summary>
        public List<Query> Queries
        {
            get { return _queries; }
            set { _queries = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the Request class.
        /// </summary>
        public Request()
        {

            _header = new Header();
            _header.IsQuery = true;
            _header.OpCode = HeaderOpCode.Query;

            _queries = new List<Query>();
        }

        /// <summary>
        /// Initializes a new instance of the Request class wrapping the specified query.
        /// </summary>
        /// <param name="query"></param>
        public Request(Query query)
            : this()
        {

            _queries.Add(query);
        }

        /// <summary>
        /// Initializes a new instance of the Request class wrapping the specified queries.
        /// </summary>
        /// <param name="queries"></param>
        public Request(IEnumerable<Query> queries)
            : this()
        {

            _queries.AddRange(queries);
        }

        /// <summary>
        /// Serializes this Request class instance for network transmission.
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize()
        {

            List<byte> data;

            // Update header properties
            _header.QDCount = (short)_queries.Count;
            _header.ANCount = 0;
            _header.ARCount = 0;
            _header.NSCount = 0;

            // Serialize members
            data = new List<byte>();
            data.AddRange(_header.Serialize());
            foreach (Query query in _queries)
                data.AddRange(query.Serialize());

            return data.ToArray();
        }

        #endregion
    }
}
