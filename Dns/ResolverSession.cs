using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// The DnsResolverSession is an internal class used to keep trac of a Dns request while it is being
    /// executed.
    /// </summary>
    internal class ResolverSession : IDisposable
    {

        #region Variables

        /// <summary>
        /// Gets / sets the nameserver for this session.
        /// </summary>
        public IPEndPoint DnsServer;

        /// <summary>
        /// Gets / sets the socket for this session.
        /// </summary>
        public Socket Socket;

        /// <summary>
        /// Gets / sets the receive buffer for this session.
        /// </summary>
        public byte[] Buffer;

        /// <summary>
        /// Gets / sets the number of bytes of the request used for verifying the whole request was sent.
        /// </summary>
        public int RequestSize;

        /// <summary>
        /// Gets / sets the request for this session.
        /// </summary>
        public Request Request;

        /// <summary>
        /// Gets / sets the response for this session.
        /// </summary>
        public Response Response;

        /// <summary>
        /// Gets / sets the asnync result for this session.
        /// </summary>
        public AsyncResult<Response> AsyncResult;

        /// <summary>
        /// Gets / sets the timeout in milliseconds for this session.
        /// </summary>
        public int TimeOut;

        /// <summary>
        /// Gets / sets the time out timer for this session.
        /// </summary>
        public Timer TimeOutTimer;

        /// <summary>
        /// Gets / sets if this request has timed out.
        /// </summary>
        public Int32 TimedOut;

        #endregion

        #region Functions

        public ResolverSession(IPEndPoint dnsServer, Request request, int timeOut, AsyncResult<Response> asyncResult)
        {

            DnsServer = dnsServer;
            Request = request;
            TimeOut = timeOut;
            AsyncResult = asyncResult;
        }

        public void Dispose()
        {
            if (TimeOutTimer != null)
                TimeOutTimer.Dispose();
            if (Socket != null)
                Socket.Close();
        }

        #endregion
    }
}
