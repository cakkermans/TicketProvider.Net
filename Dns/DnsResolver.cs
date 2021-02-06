using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// The static DnsResolver class wraps a single instance of the Resolver class and exposes a similar interface as the
    /// System.Net.DnsResolver class.
    /// </summary>
    public static class DnsResolver
    {

        #region Variables

        private static Resolver _resolver;

        #endregion

        static DnsResolver()
        {

            _resolver = new Resolver();
        }

        /// <summary>
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
        public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
        {
            return _resolver.GetHostAddresses(hostNameOrAddress);
        }

        /// <summary>
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
        public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback asyncCallback, object state)
        {
            return _resolver.BeginGetHostAddresses(hostNameOrAddress, asyncCallback, state);
        }

        public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
        {
            return _resolver.EndGetHostAddresses(asyncResult);
        }

        public static IAsyncResult BeginGetResponse(string domainName, ResourceRecordType recordType, AsyncCallback asyncCallback, object state)
        {
            return _resolver.BeginGetResponse(new Request(new Query(domainName, recordType, ResourceRecordClass.IN)), asyncCallback, state);
        }

        public static Response EndGetResponse(IAsyncResult asyncResult)
        {
            return _resolver.EndGetResponse(asyncResult);
        }

        public static Response GetResponse(string domainName, ResourceRecordType recordType)
        {
            return _resolver.GetResponse(new Request(new Query(domainName, recordType, ResourceRecordClass.IN)));
        }

        public static IAsyncResult BeginQuery(string domainName, ResourceRecordType recordType, AsyncCallback asyncCallback, object state)
        {
            return _resolver.BeginQuery(domainName, recordType, asyncCallback, state);
        }

        public static IAsyncResult BeginQuery(string domainName, ResourceRecordType recordType, ResourceRecordClass recordClass, AsyncCallback asyncCallback, object state)
        {
            return _resolver.BeginQuery(domainName, recordType, recordClass, asyncCallback, state);
        }

        public static ResourceRecord[] EndQuery(IAsyncResult asyncResult)
        {
            return _resolver.EndQuery(asyncResult);
        }

        public static ResourceRecord[] Query(string domainName, ResourceRecordType recordType)
        {
            return _resolver.Query(domainName, recordType);
        }
    }
}
