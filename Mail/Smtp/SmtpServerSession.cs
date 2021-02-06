using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TicketProvider.Net.Mail.Smtp.Commands;

namespace TicketProvider.Net.Mail.Smtp
{
    public class SmtpServerSession
    {

        #region Variables

        private Socket _socket;
        private SessionClosedCallback _sessionClosedCallback;
        private SmtpSessionMode _mode;

        private byte[] _readBufferFragment;
        private MemoryStream _readBuffer;
        private const int ReadBufferFragmentSize = 512;

        #endregion

        #region Properties

        /// <summary>
        /// Gets / sets the mode this SmptServerSession currently is in.
        /// </summary>
        public SmtpSessionMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public EndPoint RemoteEndPoint
        {
            get { return _socket.RemoteEndPoint; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the SmtpServerSession class.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="callback"></param>
        public SmtpServerSession(Socket socket, SessionClosedCallback callback)
        {

            _socket = socket;
            _sessionClosedCallback = callback;
        }

        /// <summary>
        /// Closes the SmtpServerSession. Thread safe.
        /// </summary>
        public void Close()
        {

            _socket.BeginDisconnect(false, new AsyncCallback(DisconnectCompleted), null);
        }

        private void DisconnectCompleted(IAsyncResult asyncResult)
        {
            _socket.EndDisconnect(asyncResult);
            _sessionClosedCallback(this);
        }

        public IAsyncResult BeginSendReply(SmtpReply reply, AsyncCallback asyncCallback, object state)
        {

            // Declare variables
            AsyncResultVoid asyncVoidResult;
            StringWriter textWriter;
            byte[] buffer;

            // Create async result
            asyncVoidResult = new AsyncResult<SmtpCommand>(asyncCallback, state);

            try
            {

                // Serialize the command and begin writing the serialized command bytes.
                using (textWriter = new StringWriter())
                {
                    reply.Serialize(textWriter);
                    buffer = Encoding.UTF8.GetBytes(textWriter.ToString());
                }
                _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(BeginSendReplyComplete), asyncVoidResult);
            }
            catch (Exception ex)
            {
                asyncVoidResult.Complete(ex, true);
            }

            return asyncVoidResult;
        }

        private void BeginSendReplyComplete(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResultVoid asyncVoidResult;

            asyncVoidResult = (AsyncResult<SmtpCommand>)asyncResult.AsyncState;

            try
            {

                // Complete the Stream.BeginWrite() call.
                _socket.EndSend(asyncResult);
                asyncVoidResult.Complete(null, false);
                
            }
            catch (Exception ex)
            {
                asyncVoidResult.Complete(ex, false);
            }
        }

        public void EndSendReply(IAsyncResult asyncResult)
        {

            AsyncResultVoid asyncVoidResult;

            asyncVoidResult = (AsyncResultVoid)asyncResult;
            asyncVoidResult.EndInvoke();
        }

        /// <summary>
        /// Sends a reply to the connected client in blocking mode. Use the asynchonous methods for production.
        /// </summary>
        /// <param name="reply"></param>
        public void SendReply(SmtpReply reply)
        {

            IAsyncResult asyncResult;

            asyncResult = BeginSendReply(reply, null, null);
            EndSendReply(asyncResult);
        }

        public IAsyncResult BeginReceiveCommand(AsyncCallback asyncCallback, object state)
        {

            // Declare variables
            AsyncResult<SmtpCommand> asyncCommandResult;

            asyncCommandResult = new AsyncResult<SmtpCommand>(asyncCallback, state);

            try
            {

                // Clear the read buffer and read the first [ReadBufferFragmentSize] bytes.
                _readBuffer = new MemoryStream();
                _readBufferFragment = new byte[ReadBufferFragmentSize];
                _socket.BeginReceive(_readBufferFragment, 0, ReadBufferFragmentSize, SocketFlags.None, new AsyncCallback(ReceiveCommandComplete), asyncCommandResult);
            }
            catch (Exception ex)
            {
                asyncCommandResult.Complete(ex, true);
            }

            return asyncCommandResult;
        }

        /// <summary>
        /// The ReceiveCommandComplete function processes asynchonously reads data from the socket into commands.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="asyncResult"></param>
        private void ReceiveCommandComplete(IAsyncResult asyncResult)
        {

            // Declare variables
            AsyncResult<SmtpCommand> asyncCommandResult;
            int bytesRead, breakIndex, readIndex;
            SmtpCommand command = null;
            byte[] messageBoundary;

            // Obtain handle to the async result and construct the appropriate message boundary. 
            asyncCommandResult = (AsyncResult<SmtpCommand>)asyncResult.AsyncState;
            switch (_mode)
            {
                default:
                case SmtpSessionMode.Command:
                    messageBoundary = new byte[] { (byte)'\r', (byte)'\n' };
                    break;
                case SmtpSessionMode.Data:
                    messageBoundary = new byte[] { (byte)'\r', (byte)'\n', (byte)'.', (byte)'\r', (byte)'\n' };
                    break;
            }

            try
            {

                // Complete Socket.BeginRead() call and check if a message boundary is present in the buffer
                bytesRead = _socket.EndReceive(asyncResult);
                readIndex = 0;
                breakIndex = ArrayUtilities.IndexOf<byte>(_readBufferFragment, messageBoundary, readIndex);
                if (breakIndex > -1)
                {

                    // Append the bytes before the message boundary to the read buffer,
                    // deserialize the message, add it to the stack of reply messages. Do not copy the message
                    // boundary to the read buffer.
                    _readBuffer.Write(_readBufferFragment, readIndex, breakIndex);
                    _readBuffer.Seek(0, SeekOrigin.Begin);
                    command = SmtpCommandDeserializer.Deserialize(_readBuffer, _mode);

                    // Create a new read buffer.
                    _readBuffer.Dispose();
                    _readBuffer = new MemoryStream();

                    // Advance the read index to beyond the message boundary.
                    readIndex = breakIndex + 2;
                }

                // Store remaining data in buffer fragment in the read buffer for later use, schedule reading 
                // the next set of bytes from the network stream if a partial message is still in the buffer.
                if (readIndex != bytesRead && command == null)
                {
                    _readBuffer.Write(_readBufferFragment, readIndex, bytesRead - readIndex);
                    _socket.BeginReceive(_readBufferFragment, 0, ReadBufferFragmentSize, SocketFlags.None, ReceiveCommandComplete, asyncCommandResult);
                    return;
                }

                // All replies have been read, if more than one reply was read, collapse the replies into a single
                // multiline reply and complete the async call.
                asyncCommandResult.Complete(command, false);
            }
            catch (Exception ex)
            {
                asyncCommandResult.Complete(ex, false);
            }
        }

        public SmtpCommand EndReceiveCommand(IAsyncResult asyncResult)
        {

            AsyncResult<SmtpCommand> asyncCommandResult;

            asyncCommandResult = (AsyncResult<SmtpCommand>)asyncResult;
            return asyncCommandResult.EndInvoke();
        }

        /// <summary>
        /// Receives a command from the server in blocking mode. Use the asynchonous methods for production.
        /// </summary>
        /// <returns></returns>
        public SmtpCommand ReceiveCommand()
        {

            IAsyncResult asyncResult;

            asyncResult = BeginReceiveCommand(null, null);
            return EndReceiveCommand(asyncResult);
        }

        #endregion

    }

    public delegate void SessionClosedCallback(SmtpServerSession session);

    /// <summary>
    /// The SmtpSessionMode indicates the mode a SmtpServerSession current is in.
    /// </summary>
    public enum SmtpSessionMode
    {
        /// <summary>
        /// The SmtpServerSession is currently in command reading mode. The used message boundary is CRLF.
        /// </summary>
        Command,

        /// <summary>
        /// The SmtpServerSession is currently in data reading mode. The used message boundary is CRLF.CRLF.
        /// </summary>
        Data
    }
}
