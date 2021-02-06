using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TicketProvider.Net.Mail.Smtp
{

    /// <summary>
    /// The SmtpServer class provides a low level basic interface to run a SMTP server.
    /// </summary>
    public class SmtpServer
    {

        #region Variables

        private IPEndPoint _localEp;
        private Socket _listenSocket;
        
        private List<SmtpServerSession> _sessions;

        private object _syncRoot;
        private ManualResetEvent _acceptedCompleteEvent;
        private volatile bool _stopping, _listening;

        private int _connectionLimit;
        private SmtpReply _connectionLimitExceededReply;


        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the address and port to listen on for this SmtpServer.
        /// </summary>
        public IPEndPoint ListenAddress
        {
            get { return _localEp; }
            set { _localEp = value; }
        }

        /// <summary>
        /// Gets / sets the maximum number of connections to accept.
        /// </summary>
        public int ConnectionLimit
        {
            get { return _connectionLimit; }
            set { _connectionLimit = value; }
        }

        /// <summary>
        /// Gets / sets the SmtpReply (server greeting) sent to a connection being rejected when the 
        /// connection limit is exceeded prior to closing the connection.
        /// </summary>
        public SmtpReply ConnectionLimitExceededReply
        {
            get { return _connectionLimitExceededReply; }
            set { _connectionLimitExceededReply = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the SmtpServer class.
        /// </summary>
        public SmtpServer()
        {

            _syncRoot = new object();
            _sessions = new List<SmtpServerSession>();
            _acceptedCompleteEvent = new ManualResetEvent(false);
            _connectionLimit = 10;
            _connectionLimitExceededReply = new SmtpReply(554, "Connection limit exceeded. Please try again later.");
        }

        /// <summary>
        /// Initializes a new instance of the SmtpServer class listening on the specified local end point.
        /// </summary>
        /// <param name="endPoint"></param>
        public SmtpServer(IPEndPoint localEp)
            : this()
        {

            _localEp = localEp;
        }

        /// <summary>
        /// Starts the SmtpServer.
        /// </summary>
        public void Start()
        {

            // Make sure the server isn't started yet.
            lock (_syncRoot)
            {
                if (_listening)
                    return;
                _listening = true;
                _stopping = false;
            }

            try
            {

                // Create listen socket, put is in listen mode and start accepting the first connection.
                _listenSocket = new Socket(_localEp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _listenSocket.Bind(_localEp);
                _listenSocket.Listen(16);
                _listenSocket.BeginAccept(BeginAcceptCallback, null);
            }
            catch (Exception)
            {
                lock (_syncRoot)
                    _listening = true;
            }
        }

        public void Stop()
        {

            _stopping = true;
        }

        private void BeginAcceptCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            Socket socket;
            SmtpServerSession session;

            // Complete the BeginAccept() call
            socket = _listenSocket.EndAccept(asyncResult);

            // Start accepting the next connection if the server is not being stopped.
            if (!_stopping)
                _listenSocket.BeginAccept(BeginAcceptCallback, null);
            else
                _listening = false;

            // Construct a new server session and register the server session.
            session = new SmtpServerSession(socket, SessionClosed);
            if (!RegisterSession(session))
            {
                session.BeginSendReply(_connectionLimitExceededReply, ConnectionRefuseComplete, this);
            }
        }

        private void ConnectionRefuseComplete(object state)
        {
            ((SmtpServerSession)state).Close();
        }

        /// <summary>
        /// Attempts to register the specified SmtpServerSession with the SmtpServer. Returns false if the connection
        /// limit has been reached.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private bool RegisterSession(SmtpServerSession session)
        {

            lock (_syncRoot)
            {
                if (_sessions.Count < _connectionLimit)
                    _sessions.Add(session);
                else
                    return false;
            }

            SessionConnected(this, new SmtpServerSessionEventArgs(session));
            return true;
        }

        private void UnregisterSession(SmtpServerSession session)
        {

            lock (_syncRoot)
            {
                _sessions.Remove(session);
            }

            SessionDisconnected(this, new SmtpServerSessionEventArgs(session));
        }

        private void SessionClosed(SmtpServerSession session)
        {

            UnregisterSession(session);
        }

        #endregion

        #region Events

        public event EventHandler<SmtpServerSessionEventArgs> SessionConnected = delegate { };

        public event EventHandler<SmtpServerSessionEventArgs> SessionDisconnected = delegate { };

        #endregion
    }

    public class SmtpServerSessionEventArgs : EventArgs
    {

        private SmtpServerSession _session;

        public SmtpServerSession Session
        {
            get { return _session; }
            set { _session = value; }
        }

        public SmtpServerSessionEventArgs(SmtpServerSession session)
        {
            _session = session;
        }
    }
}
