using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TicketProvider.Net.Mail.Smtp
{

    /// <summary>
    /// The SmtpClient class provides a low level asynchonous interface to a remote SMTP server.
    /// </summary>
    /// <remarks>
    /// The SmtpClient class is not thread safe.
    /// </remarks>
    public class SmtpClient : IDisposable
    {

        #region Variables

        private IPEndPoint _server;
        private Socket _socket;

        private byte[] _readBufferFragment;
        private MemoryStream _readBuffer;
        private const int ReadBufferFragmentSize = 512;

        private List<SmtpReply> _replyBuffer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the SMTP server to connect to.
        /// </summary>
        public IPEndPoint Server
        {
            get { return _server; }
            set
            {
                if (_socket.Connected)
                    throw new InvalidOperationException("Unable to change the SMTP server while the SMTP client is connected.");
                _server = value;
            }
        }

        /// <summary>
        /// Gets if this SMTP client is connected to the SMTP server.
        /// </summary>
        public bool Connected
        {
            get 
            {
                if(_socket == null)
                    return false;
                return _socket.Connected; 
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the SmtpClient class.
        /// </summary>
        public SmtpClient()
        {

            _replyBuffer = new List<SmtpReply>();
        }

        /// <summary>
        /// Initializes a new instance of the SmtpClient class connecting to the specified SMTP server.
        /// </summary>
        /// <param name="smtpServer"></param>
        public SmtpClient(IPAddress server)
            : this()
        {

            _server = new IPEndPoint(server, 25);
        }

        /// <summary>
        /// Initializes a new instance of the SmtpClient class connecting to the specified SMTP server.
        /// </summary>
        /// <param name="smtpServer"></param>
        public SmtpClient(IPEndPoint server)
            : this()
        {

            _server = server;
        }

        /// <summary>
        /// Connects the SmtpClient to the specified SMTP server and returns the greeting SMTP response.
        /// </summary>
        public SmtpReply Connect()
        {

            IAsyncResult asyncResult;

            asyncResult = BeginConnect(null, null);
            return EndConnect(asyncResult);
        }

        /// <summary>
        /// Asynchonously connects the SmtpClient to the specified SMTP server and returns the 
        /// greeting SMTP response.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginConnect(AsyncCallback callback, object state)
        {

            // Declare variables
            AsyncResult<SmtpReply> asyncCommandReply;

            // Create the async result and start connecting the socket.
            asyncCommandReply = new AsyncResult<SmtpReply>(callback, state);
            _socket = new Socket(_server.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.BeginConnect(Server, BeginConnectCallback, asyncCommandReply);

            return asyncCommandReply;
        }

        private void BeginConnectCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<SmtpReply> asyncCommandReply;

            asyncCommandReply = (AsyncResult<SmtpReply>)asyncResult.AsyncState;
            try
            {

                // Complete the socket connect call and wrap the socket in a NetworkStream.
                _socket.EndConnect(asyncResult);

                // Clear the read buffer and read the first [ReadBufferFragmentSize] bytes of the greeting message.
                _readBuffer = new MemoryStream();
                _replyBuffer.Clear();
                _readBufferFragment = new byte[ReadBufferFragmentSize];
                _socket.BeginReceive(_readBufferFragment, 0, ReadBufferFragmentSize, SocketFlags.None, ReceiveReplyCallback, asyncCommandReply);
            }
            catch (Exception ex)
            {
                asyncCommandReply.Complete(ex, false);
            }
        }

        /// <summary>
        /// Completes an asynchonous connect operation.
        /// </summary>
        /// <param name="asyncResult"></param>
        public SmtpReply EndConnect(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<SmtpReply> asyncCommandReply;

            asyncCommandReply = (AsyncResult<SmtpReply>)asyncResult;
            return asyncCommandReply.EndInvoke();
        }

        /// <summary>
        /// Disconnects the SmtpClient from the SMTP server.
        /// </summary>
        public void Disconnect()
        {

            IAsyncResult asyncResult;

            asyncResult = BeginDisconnect(null, null);
            EndDisconnect(asyncResult);
        }

        /// <summary>
        /// Asynchonously disconnects the SmtpClient from the SMTP server.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginDisconnect(AsyncCallback asyncCallback, object state)
        {

            // Declare variables
            AsyncResultVoid asyncResultVoid;

            // Create the async result and start connecting the socket.
            asyncResultVoid = new AsyncResultVoid(asyncCallback, state);
            _socket.BeginDisconnect(true, BeginDisconnectCallback, asyncResultVoid);

            return asyncResultVoid;
        }

        private void BeginDisconnectCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResultVoid asyncResultVoid;

            asyncResultVoid = (AsyncResultVoid)asyncResult.AsyncState;
            try
            {

                // Complete the socket disconnect call and clean up the NetworkStream wrapping the socket.
                _socket.EndDisconnect(asyncResult);
                _socket.Close();
                _socket = null;

                // Complete async call.
                asyncResultVoid.Complete(null, false);
            }
            catch (Exception ex)
            {
                asyncResultVoid.Complete(ex, false);
            }
        }

        /// <summary>
        /// Completes an asynchonous disconnect operation.
        /// </summary>
        /// <param name="asyncResult"></param>
        public void EndDisconnect(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResultVoid asyncResultVoid;

            asyncResultVoid = (AsyncResultVoid)asyncResult;
            asyncResultVoid.EndInvoke();
        }

        /// <summary>
        /// Asynchonously sends the specified command to the SMTP server this SmptClient class instance is connected to
        /// and returns the reply received from the server.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="asyncCallback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IAsyncResult BeginSendCommand(SmtpCommand command, AsyncCallback asyncCallback, object state)
        {

            // Declare variables
            AsyncResult<SmtpReply> asyncCommandReply;
            MemoryStream memStream;
            byte[] buffer;

            // Create async result
            asyncCommandReply = new AsyncResult<SmtpReply>(asyncCallback, state);

            try
            {

                // Serialize the command and begin writing the serialized command bytes.
                using (memStream = new MemoryStream())
                {
                    command.Serialize(memStream);
                    buffer = memStream.ToArray();
                }

                _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, BeginSendCommandCallback, asyncCommandReply);
            }
            catch (Exception ex)
            {
                asyncCommandReply.Complete(ex, true);
            }

            return asyncCommandReply;
        }

        private void BeginSendCommandCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<SmtpReply> asyncCommandReply;

            asyncCommandReply = (AsyncResult<SmtpReply>)asyncResult.AsyncState;

            try
            {

                // Complete the Stream.BeginWrite() call.
                _socket.EndSend(asyncResult);

                // Clear the read buffer and read the first [ReadBufferFragmentSize] bytes.
                _readBuffer = new MemoryStream();
                _replyBuffer.Clear();
                _readBufferFragment = new byte[ReadBufferFragmentSize];
                _socket.BeginReceive(_readBufferFragment, 0, ReadBufferFragmentSize, SocketFlags.None, ReceiveReplyCallback, asyncCommandReply);
            }
            catch (Exception ex)
            {
                asyncCommandReply.Complete(ex, false);
            }
        }

        /// <summary>
        /// The ReceiveReplyCallback function processes asynchonously read data from the socket into messages.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="asyncResult"></param>
        private void ReceiveReplyCallback(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<SmtpReply> asyncCommandReply;
            byte[] messageBoundary;
            int bytesRead, breakIndex, readIndex;
            SmtpReply reply = null;
            StreamReader streamReader;

            asyncCommandReply = (AsyncResult<SmtpReply>)asyncResult.AsyncState;
            messageBoundary = new byte[] { (byte)'\r', (byte)'\n' };

            try
            {

                // Complete Socket.BeginRead() call and check if a message boundary is present in the buffer
                bytesRead = _socket.EndReceive(asyncResult);
                readIndex = 0;
                while (true)
                {
                    breakIndex = IndexOf(_readBufferFragment, messageBoundary, readIndex);
                    if (breakIndex < 0)
                    {
                        
                        // No message boundary found break out of the read loop.
                        break;
                    }
                    else
                    {

                        // Append the bytes before the message boundary and the boundary to the read buffer,
                        // deserialize the message, add it to the stack of reply messages.
                        _readBuffer.Write(_readBufferFragment, readIndex, breakIndex + 2);
                        _readBuffer.Seek(0, SeekOrigin.Begin);
                        using (streamReader = new StreamReader(_readBuffer, Encoding.UTF8))
                        {
                            reply = new SmtpReply();
                            reply.Deserialize(streamReader);
                            _replyBuffer.Add(reply);
                        }
                        
                        // Create a new read buffer since if was destroyed wehen the streamReader
                        // was destroyed.
                        _readBuffer.Dispose();
                        _readBuffer = new MemoryStream();
                    }

                    // Advance the read index
                    readIndex = breakIndex + 2;
                }

                // Store remaining data in buffer fragment in the read buffer for later use, schedule reading 
                // the next set of bytes from the network stream if a partial message is still in the buffer.
                if (readIndex != bytesRead)
                {
                    _readBuffer.Write(_readBufferFragment, readIndex, bytesRead - readIndex);
                    _socket.BeginReceive(_readBufferFragment, 0, ReadBufferFragmentSize, SocketFlags.None, ReceiveReplyCallback, asyncCommandReply);
                    return;
                }

                // Check if the last message was a multi line message. If so schedule reading the remainder of
                // multi line message.
                if (reply != null)
                {
                    if (reply.IsMultiLine)
                    {
                        _socket.BeginReceive(_readBufferFragment, 0, ReadBufferFragmentSize, SocketFlags.None, ReceiveReplyCallback, asyncCommandReply);
                        return;
                    }
                }

                // All replies have been read, if more than one reply was read, collapse the replies into a single
                // multiline reply and complete the async call.
                asyncCommandReply.Complete(CollapseReplies(), false);
            }
            catch (Exception ex)
            {
                asyncCommandReply.Complete(ex, false);
            }
        }

        private int IndexOf(byte[] haystack, byte[] pattern, int start)
        {

            int haystackLength;

            haystackLength = haystack.Length - pattern.Length;
            for (int i = start; i < haystackLength; i++)
            {
                if (IsMatch(haystack, i, pattern))
                    return i;
            }

            return -1;
        }

        private bool IsMatch(byte[] haystack, int index, byte[] pattern)
        {

            int patternLength;

            patternLength = pattern.Length;
            for (int j = 0; j < patternLength; j++)
            {
                if (haystack[index + j] != pattern[j])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Collapses all the replies into the reply buffer into a single reply concatenating the reply messages
        /// into a single multi line message string and clears the reply buffer afterward.
        /// </summary>
        /// <returns></returns>
        public SmtpReply CollapseReplies()
        {

            SmtpReply reply, nextReply;
            StringWriter writer;

            // Check the number of messages
            reply = _replyBuffer[0];
            if (_replyBuffer.Count == 1 || !reply.IsMultiLine)
            {
                _replyBuffer.Clear();
                return reply;
            }

            // Concatenate all the reply messages.
            using (writer = new StringWriter())
            {
                writer.Write(reply.Message);
                for (int i = 1; i < _replyBuffer.Count; i++)
                {

                    // Append CR LF
                    if (i > 1)
                        writer.Write("\r\n");
                    nextReply = _replyBuffer[i];

                    // Check message
                    if (nextReply.Code != reply.Code)
                        throw new InvalidDataException("Invalid multiline reply received from server.");
                    writer.Write(nextReply.Message);
                    if (!nextReply.IsMultiLine)
                        break;
                }

                // Return reply and clear buffer.
                _replyBuffer.Clear();
                reply.Message = writer.ToString();
                return reply;
            }
        }

        /// <summary>
        /// Handles the end of an asynchonous message transmission.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public SmtpReply EndSendCommand(IAsyncResult asyncResult)
        {
            
            // Declare variables
            AsyncResult<SmtpReply> asyncCommandReply;

            asyncCommandReply = (AsyncResult<SmtpReply>)asyncResult;
            return asyncCommandReply.EndInvoke();
        }

        /// <summary>
        /// Sends the specified command to the SMTP server this SmptClient class instance is connected to
        /// and returns the reply received from the server.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public SmtpReply SendCommand(SmtpCommand command)
        {

            IAsyncResult asyncResult;

            asyncResult = BeginSendCommand(command, null, null);
            return EndSendCommand(asyncResult);
        }

        /// <summary>
        /// Releases any unmanaged resources associated with this instance of the SmtpClient class.
        /// </summary>
        public void Dispose()
        {

            if(_readBuffer != null)
                _readBuffer.Dispose();
            if (_socket != null)
                _socket.Close();
        }

        #endregion
    }
}
