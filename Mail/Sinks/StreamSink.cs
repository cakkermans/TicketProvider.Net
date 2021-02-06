using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Sinks
{

    /// <summary>
    /// The FileSink outputs mail messages to a stream.
    /// </summary>
    public class StreamSink : IMessageSink
    {

        #region Variables

        private ImfWriter _output;

        #endregion

        #region Properties

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the StreamSink class writing to the passed stream.
        /// </summary>
        /// <param name="stream"></param>
        public StreamSink(Stream stream)
        {

            _output = new ImfWriter(stream, ImfWriterEncoding.EightBit);
        }

        /// <summary>
        /// Initializes a new instance of the StreamSink class writing to specified file.
        /// </summary>
        /// <param name="path"></param>
        public StreamSink(string path)
        {

            Stream stream;

            stream = File.Create(path);
            _output = new ImfWriter(stream, ImfWriterEncoding.EightBit);
        }

        #endregion

        #region IMessageSink Members

        public void Send(MailMessage message)
        {

            message.Serialize(_output);
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
            _output.Dispose();
        }

        public bool IsOpen
        {
            get { return true; }
        }

        #endregion
    }
}
