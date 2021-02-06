using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using TicketProvider.Net.Dns.Records;


namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// The Resolver class provides a clean API for resolving 
    /// </summary>
    public class Resolver
    {

        #region Variables

        private bool _recursion;
        private List<IPEndPoint> _dnsServers;
        private int _timeOut;
        private int _retries;
        private ResolverTransport _transport;

        /// <summary>
        /// The default port to connect to when connecting to a name server.
        /// </summary>
        public const int DefaultPort = 53;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets if the resolver should perform recursion when resolving a host name.
        /// </summary>
        public bool Recursion
        {
            get { return _recursion; }
            set { _recursion = value; }
        }

        /// <summary>
        /// Gets / sets a list containing the DNS servers this resolver instance will user for resolving
        /// host names.
        /// </summary>
        public List<IPEndPoint> DnsServers
        {
            get { return _dnsServers; }
            set { _dnsServers = value; }
        }

        /// <summary>
        /// Gets / sets the timeout in milliseconds to use when resolving host names.
        /// </summary>
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        /// <summary>
        /// Gets / sets the number of retrier to attempt when resolving a host name.
        /// </summary>
        public int Retries
        {
            get { return _retries; }
            set { _retries = value; }
        }

        /// <summary>
        /// Gets / sets the transport this resolver class instance should use when resolving a host name.
        /// </summary>
        public ResolverTransport Transport
        {
            get { return _transport; }
            set { _transport = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the Resolver class.
        /// </summary>
        public Resolver()
        {

            _dnsServers = GetDnsServers();
            _transport = ResolverTransport.Udp;
            _timeOut = 3000;
        }

        /// <summary>
        /// Initializes a new instance of the Resolver class which will use the specified
        /// DNS server for resolving host names.
        /// </summary>
        /// <param name="dnsServer"></param>
        public Resolver(IPEndPoint dnsServer)
        {

            _dnsServers = new List<IPEndPoint>();
            _dnsServers.Add(dnsServer);
            _transport = ResolverTransport.Udp;
            _timeOut = 3000;
        }

        /// <summary>
        /// Returns a list of the DNS servers used on any of the active network interfaces.
        /// </summary>
        /// <returns></returns>
        public static List<IPEndPoint> GetDnsServers()
        {

            // Declare variabels
            List<IPEndPoint> servers;
            IPEndPoint server;
            NetworkInterface[] networkInterfaces;
            IPInterfaceProperties interfaceProperties;

            servers = new List<IPEndPoint>();
            networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // Iterate over network interfaces
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    interfaceProperties = networkInterface.GetIPProperties();
                    foreach (IPAddress ipAddress in interfaceProperties.DnsAddresses)
                    {
                        server = new IPEndPoint(ipAddress, DefaultPort);
                        if (!servers.Contains(server))
                            servers.Add(server);
                    }
                }
            }

            return servers;
        }

        /// <summary>
        /// Returns a list of the local addresses on any of the active network interface.
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalAddress()
        {

            // Declare variabels
            List<IPAddress> addresses;
            NetworkInterface[] networkInterfaces;
            IPInterfaceProperties interfaceProperties;

            addresses = new List<IPAddress>();
            networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            // Iterate over network interfaces
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    interfaceProperties = networkInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ipAddress in interfaceProperties.UnicastAddresses)
                    {
                        if (!addresses.Contains(ipAddress.Address))
                            addresses.Add(ipAddress.Address);
                    }
                }
            }

            return addresses;
        }

        /// <summary>
        /// Gets the host name of the local computer.
        /// </summary>
        /// <returns></returns>
        public string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// The GetHostAddresses method queries a DNS server for the IP addresses associated with a host name. 
        /// If hostNameOrAddress is an IP address, this address is returned without querying the DNS server.
        ///
        /// When an empty string is passed as the host name, this method returns the IPv4 addresses of the local 
        /// host for all operating systems except Windows Server 2003; for Windows Server 2003, both IPv4 and IPv6 
        /// addresses for the local host are returned.
        /// </remarks>
        /// <param name="hostName"></param>
        /// <returns>An array of type IPAddress that holds the IP addresses for the host that is specified by the 
        /// hostNameOrAddress parameter.</returns>
        public IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {

            // Declare variables
            List<IPAddress> addresses;
            IPAddress ipAddress;
            Request request;
            IAsyncResult asyncResult;
            Response response;

            // Return local ip addresses
            if (string.IsNullOrEmpty(hostNameOrAddress))
                return GetLocalAddress().ToArray();

            // Check if input is a valid ip address
            if (IPAddress.TryParse(hostNameOrAddress, out ipAddress))
                return new IPAddress[] { ipAddress };

            // Create request requesting both A and AAAA records for the passed host name.
            request = new Request();
            request.Queries.Add(new Query(hostNameOrAddress, ResourceRecordType.A, ResourceRecordClass.IN));
            //request.Queries.Add(new Query(hostNameOrAddress, ResourceRecordType.AAAA, ResourceRecordClass.IN));

            // Wrap the async interface and execute the request
            asyncResult = BeginGetResponse(request, null, null);
            response = EndGetResponse(asyncResult);

            addresses = new List<IPAddress>();
            foreach (ResourceRecord record in response.Answers)
            {
                switch (record.Type)
                {
                    case ResourceRecordType.A:
                        addresses.Add(((ARecord)record).IPAddress);
                        break;
                    case ResourceRecordType.AAAA:
                        addresses.Add(((AAAARecord)record).IPAddress);
                        break;
                }
            }

            return addresses.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// The GetHostAddresses method queries a DNS server for the IP addresses associated with a host name. 
        /// If hostNameOrAddress is an IP address, this address is returned without querying the DNS server.
        ///
        /// When an empty string is passed as the host name, this method returns the IPv4 addresses of the local 
        /// host for all operating systems except Windows Server 2003; for Windows Server 2003, both IPv4 and IPv6 
        /// addresses for the local host are returned.
        /// </remarks>
        /// <param name="hostName"></param>
        /// <returns>An array of type IPAddress that holds the IP addresses for the host that is specified by the 
        /// hostNameOrAddress parameter.</returns>
        public IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback asyncCallback, object state)
        {

            // Declare variables
            IPAddress ipAddress;
            Request request;
            IAsyncResult asyncResult;
            AsyncResult<IPAddress[]> asyncAddressesResult;

            asyncAddressesResult = new AsyncResult<IPAddress[]>(asyncCallback, state);

            try
            {
                // Return local ip addresses
                if (string.IsNullOrEmpty(hostNameOrAddress))
                {
                    asyncAddressesResult.Complete( GetLocalAddress().ToArray(), true);
                    return asyncAddressesResult;
                }

                // Check if input is a valid ip address
                if (IPAddress.TryParse(hostNameOrAddress, out ipAddress))
                {
                    asyncAddressesResult.Complete(new IPAddress[] { ipAddress }, true);
                    return asyncAddressesResult;
                }

                // Create request requesting both A and AAAA records for the passed host name.
                request = new Request();
                request.Queries.Add(new Query(hostNameOrAddress, ResourceRecordType.A, ResourceRecordClass.IN));
                //request.Queries.Add(new Query(hostNameOrAddress, ResourceRecordType.AAAA, ResourceRecordClass.IN));

                // Wrap the async interface and execute the request
                asyncResult = BeginGetResponse(request, BeginGetHostAddressesCallback, asyncAddressesResult);
                
            }
            catch (Exception ex)
            {
                asyncAddressesResult.Complete(ex, true);
            }

            return asyncAddressesResult;
        }

        private void BeginGetHostAddressesCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<IPAddress[]> asyncAddressesResult;
            Response response;
            List<IPAddress> addresses;

            asyncAddressesResult = (AsyncResult<IPAddress[]>)asyncResult.AsyncState;

            try
            {

                // Complete the asynchonous call to GetResponse().
                response = EndGetResponse(asyncResult);

                // Process the response
                addresses = new List<IPAddress>();
                foreach (ResourceRecord record in response.Answers)
                {
                    switch (record.Type)
                    {
                        case ResourceRecordType.A:
                            addresses.Add(((ARecord)record).IPAddress);
                            break;
                        case ResourceRecordType.AAAA:
                            addresses.Add(((AAAARecord)record).IPAddress);
                            break;
                    }
                }

                asyncAddressesResult.Complete(addresses.ToArray(), false);
            }
            catch (Exception ex)
            {
                asyncAddressesResult.Complete(ex, false);
            }
        }

        public IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<IPAddress[]> asyncAddressesResult;

            asyncAddressesResult = (AsyncResult<IPAddress[]>)asyncResult;
            return asyncAddressesResult.EndInvoke();
        }

        public IAsyncResult BeginGetResponse(Request request, AsyncCallback callback, object state)
        {

            // Declare variables
            AsyncResult<Response> asyncRequestResult;
            ResolverSession session;

            // Create async result and session
            asyncRequestResult = new AsyncResult<Response>(callback, state);
            session = new ResolverSession(_dnsServers[0], request, _timeOut, asyncRequestResult);
            switch (_transport)
            {
                case ResolverTransport.Udp:
                    SendRequestUdp(session);
                    break;
                case ResolverTransport.Tcp:
                    throw new NotSupportedException("The TCP transport is currently not implemented.");
                    break;
            }

            return asyncRequestResult;
        }

        public Response EndGetResponse(IAsyncResult asyncResult)
        {

            AsyncResult<Response> asyncRequestResult;

            asyncRequestResult = (AsyncResult<Response>)asyncResult;
            return asyncRequestResult.EndInvoke();
        }

        public Response GetResponse(Request request)
        {

            IAsyncResult asyncResult;

            asyncResult = BeginGetResponse(request, null, null);
            return EndGetResponse(asyncResult);
        }

        /// <summary>
        /// Begins an asynchonous dns query for the specified domain name requesting records of the specified type.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public IAsyncResult BeginQuery(string domainName, ResourceRecordType recordType, AsyncCallback asyncCallback, object state)
        {
            return BeginQuery(domainName, recordType, ResourceRecordClass.IN, asyncCallback, state);
        }

        /// <summary>
        /// Begins an asynchonous dns query for the specified domain name requesting records of the specified type and class.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="recordType"></param>
        /// <param name="recordClass"></param>
        /// <returns></returns>
        public IAsyncResult BeginQuery(string domainName, ResourceRecordType recordType, ResourceRecordClass recordClass, AsyncCallback asyncCallback, object state)
        {

            // Declare variables
            Request request;

            // Build and send DNS request
            request = new Request(new Query(domainName, recordType, recordClass));
            return BeginGetResponse(request, asyncCallback, state);
        }

        public ResourceRecord[] EndQuery(IAsyncResult asyncResult)
        {

            // Declare variables
            Response response;

            // Return answers
            response = EndGetResponse(asyncResult);
            return response.Answers.ToArray();
        }

        /// <summary>
        /// Executes a dns query for the specified domain name requesting records of the specified type and returns the
        /// returned resources records.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public ResourceRecord[] Query(string domainName, ResourceRecordType recordType)
        {

            // Declare variables
            IAsyncResult asyncResult;

            // Wrap async interface
            asyncResult = BeginQuery(domainName, recordType, null, null);
            return EndQuery(asyncResult);
        }

        #endregion

        #region Functions - UDP Internals

        private void SendRequestUdp(ResolverSession session)
        {

            // Declare variables
            byte[] sendBuffer;

            try
            {

                // Create socket
                session.Socket = new Socket(session.DnsServer.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                // Serialize message
                sendBuffer = session.Request.Serialize();
                session.RequestSize = sendBuffer.Length;

                // Connect udp socket (no need to connect asynchonously, since no real connection is made when
                // 'connection' a datagram protocol.
                session.Socket.Connect(session.DnsServer);
                session.Socket.BeginSendTo(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, session.DnsServer, new AsyncCallback(SendRequestUdpComplete), session);
            }
            catch (Exception ex)
            {
                session.AsyncResult.Complete(ex, true);
            }
        }

        private void SendRequestUdpComplete(IAsyncResult asyncResult)
        {

            // Declare variables
            ResolverSession session;
            EndPoint endPoint;

            // Check if all data was sent.
            session = (ResolverSession)asyncResult.AsyncState;
            try
            {
                if (session.Socket.EndSend(asyncResult) != session.RequestSize)
                {
                    session.AsyncResult.Complete(new DnsException("Unable to complete DNS request. Not all request data was sent."), false);
                    return;
                }

                // Create and setup time out timer
                session.TimeOutTimer = new Timer(new TimerCallback(ReveiceResponseUdpTimeOut), session, session.TimeOut, session.TimeOut);

                // Begin receiving the response.
                session.Buffer = new byte[512];
                endPoint = session.DnsServer;
                session.Socket.BeginReceiveFrom(session.Buffer, 0, session.Buffer.Length, SocketFlags.None, ref endPoint, new AsyncCallback(ReceiveResponseUdpComplete), session);
            }
            catch (Exception ex)
            {
                session.AsyncResult.Complete(ex, false);
            }
        }

        private void ReveiceResponseUdpTimeOut(object state)
        {

            // Declare variables
            ResolverSession session;
            int timedOut;

            // Flag the session as timed out, if the session wasn't timed out yet,
            session = (ResolverSession)state;
            timedOut = Interlocked.CompareExchange(ref session.TimedOut, 1, 0);
            if (timedOut == 0)
            {
                session.Socket.Close();
            }

            // Destroy the timer
            if (session.TimeOutTimer != null)
            {
                session.TimeOutTimer.Dispose();
                session.TimeOutTimer = null;
            }
        }

        private void ReceiveResponseUdpComplete(IAsyncResult asyncResult)
        {

            // Declare variables
            ResolverSession session;
            int receivedBytes;
            EndPoint endPoint;
            ResponseReader reader;

            // Get received data
            session = (ResolverSession)asyncResult.AsyncState;
            try
            {
                endPoint = session.DnsServer;
                if (!session.Socket.Connected)
                {
                    if (session.TimedOut == 1)
                        session.AsyncResult.Complete(new DnsException("The name server timed out."), false);
                    else
                        session.AsyncResult.Complete(new DnsException("No socket was connected."), false);
                    return;
                }
                receivedBytes = session.Socket.EndReceiveFrom(asyncResult, ref endPoint);
                if (receivedBytes == 0)
                {
                    session.AsyncResult.Complete(new DnsException("No response received from name server."), false);
                    session.Socket.Close();
                    return;
                }
                session.Socket.Close();

                // Read the response
                reader = new ResponseReader(session.Buffer);
                session.Response = new Response(reader);

                // Verify that the received response belongs to the send request.
                if (session.Response.Header.Id != session.Request.Header.Id)
                {
                    session.AsyncResult.Complete(new DnsException("The name server returned a response not associated with the supplied request."), false);
                    return;
                }

                // Complete the async call
                session.AsyncResult.Complete(session.Response, false);
            }
            catch (Exception ex)
            {
                session.AsyncResult.Complete(ex, false);
            }
        }

        #endregion
    }
}
