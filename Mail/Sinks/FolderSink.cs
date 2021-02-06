using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TicketProvider.Net.Mail.Sinks
{

    /// <summary>
    /// The FolderSink outputs mail messages as files to a folder.
    /// </summary>
    public class FolderSink : IMessageSink
    {

        #region Variables

        private string _outputPath;

        #endregion

        #region Properties

        public string OutputPath
        {
            get { return _outputPath;}
            set
            {


                string path;

                // Verify the passed path is valid
                if (Directory.Exists(value))
                {
                    _outputPath = value;
                    return;
                }
                path = Path.GetDirectoryName(value);
                if (!Directory.Exists(path))
                    throw new ArgumentException("The specified path does not exist.");

                _outputPath = path + Path.DirectorySeparatorChar;
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes a new instance of the StreamSink class writing to specified path.
        /// </summary>
        /// <param name="path"></param>
        public FolderSink(string outputPath)
        {

            OutputPath = outputPath;
        }

        #endregion

        #region IMessageSink Members

        public void Send(MailMessage message)
        {

            Stream outputStream;
            string outputFile;
            int count;

            outputFile = Path.Combine(_outputPath, message.MessageId.ToString());
            count = 1;
            while (File.Exists(outputFile))
            {
                outputFile = Path.Combine(_outputPath, string.Format("{0} ({1})", message.MessageId, count));
                count++;
            }

            using (outputStream = File.Create(outputFile))
            {
                using (ImfWriter writer = new ImfWriter(outputStream, ImfWriterEncoding.EightBit))
                {
                    message.Serialize(writer);
                }
            }
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }

        public bool IsOpen
        {
            get { return true; }
        }

        #endregion
    }
}

