using System;
using System.Collections.Generic;
using System.Net;

namespace TicketProvider.Net.Dns
{

    #region RFC1035 Specification

    /*
4.1. Format

All communications inside of the domain protocol are carried in a single
format called a message.  The top level format of message is divided
into 5 sections (some of which are empty in certain cases) shown below:

    +---------------------+
    |        Header       |
    +---------------------+
    |       Question      | the question for the name server
    +---------------------+
    |        Answer       | RRs answering the question
    +---------------------+
    |      Authority      | RRs pointing toward an authority
    +---------------------+
    |      Additional     | RRs holding additional information
    +---------------------+

The header section is always present.  The header includes fields that
specify which of the remaining sections are present, and also specify
whether the message is a query or a response, a standard query or some
other opcode, etc.

The names of the sections after the header are derived from their use in
standard queries.  The question section contains fields that describe a
question to a name server.  These fields are a query type (QTYPE), a
query class (QCLASS), and a query domain name (QNAME).  The last three
sections have the same format: a possibly empty list of concatenated
resource records (RRs).  The answer section contains RRs that answer the
question; the authority section contains RRs that point toward an
authoritative name server; the additional records section contains RRs
which relate to the query, but are not strictly answers for the
question.
*/

    #endregion

    /// <summary>
    /// The Response class represents a response from a name server to a dns request.
    /// </summary>
    public class Response
    {

        #region Variables

        private Header _header;

        private List<Query> _queryList;
        private List<ResourceRecord> _answerList;
        private List<ResourceRecord> _authorityList;
        private List<ResourceRecord> _additionalList;

        private int _iMessageSize;
        private IPEndPoint _server;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the header associated with this response message.
        /// </summary>
        public Header Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public List<Query> Queries
        {
            get { return _queryList; }
            set { _queryList = value; }
        }

        public List<ResourceRecord> Answers
        {
            get { return _answerList; }
            set { _answerList = value; }
        }

        public List<ResourceRecord> Authority
        {
            get { return _authorityList; }
            set { _authorityList = value; }
        }

        public List<ResourceRecord> Additional
        {
            get { return _additionalList; }
            set { _additionalList = value; }
        }

        /// <summary>
        /// Gets / sets the name server this response originates from.
        /// </summary>
        public IPEndPoint Server
        {
            get { return _server; }
            set { _server = value; }
        }

        #endregion

        #region Functions - Constructor

        /// <summary>
        /// Initializes a new instance of the Response class.
        /// </summary>
        public Response()
        {

            _header = new Header();

            _queryList = new List<Query>();
            _answerList = new List<ResourceRecord>();
            _authorityList = new List<ResourceRecord>();
            _additionalList = new List<ResourceRecord>();
        }

        /// <summary>
        /// Deserializes response data from the specified ResponseReader class instance and populates the members of a new
        /// Response class instance with the deserialized data.
        /// </summary>
        /// <param name="reader"></param>
        public Response(ResponseReader reader)
            : this()
        {
            Deserialize(reader);
        }

        #endregion

        #region Functions

        public void Deserialize(ResponseReader reader)
        {

            _header.Deserialize(reader);

            // Deserialize queries
            for (int i = 0; i < _header.QDCount; i++)
                _queryList.Add(new Query(reader));

            // Deserialize answers
            for (int i = 0; i < _header.ANCount; i++)
                _answerList.Add(reader.ReadRecord());

            // Deserialize authority resource records
            for (int i = 0; i < Header.NSCount; i++)
                _authorityList.Add(reader.ReadRecord());

            // Deserialize records holding additional information
            for (int i = 0; i < Header.ARCount; i++)
                _additionalList.Add(reader.ReadRecord());
        }

        #endregion
    }
}
