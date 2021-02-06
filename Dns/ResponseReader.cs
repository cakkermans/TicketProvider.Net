using System;
using System.Collections.Generic;
using System.Text;
using TicketProvider.Net.Dns.Records;

namespace TicketProvider.Net.Dns
{

    /// <summary>
    /// The ResponseReader class provides a System.IO.BinaryReader like interface for reading bytes
    /// with specific functionality for the TicketProvider.Net.Dns namespace.
    /// </summary>
    public class ResponseReader
    {

        #region Variables

        private byte[] _data;
        private int _position;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying buffer of this BytesReader class instance.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                // Check out of bounds
                if (value < 0 || value >= _data.Length)
                    throw new ArgumentException("The specified position is out of bounds.");
                _position = value;
            }
        }

        #endregion

        #region Functions - Construction

        /// <summary>
        /// Initializes a new instance of the ResponseReader class wrapping the specified buffer.
        /// </summary>
        /// <param name="data"></param>
        public ResponseReader(byte[] data)
        {

            _data = data;
            _position = 0;
        }

        /// <summary>
        /// Initializes a new instance of the ResponseReader class wrapping the specified buffer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="position"></param>
        public ResponseReader(byte[] data, int position)
            : this(data)
        {

            _position = position;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Reads the specified number of bytes starting from the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int index, int count)
        {

            byte[] data;

            // Check out of bounds
            if ((index + count) >= _data.Length)
                throw new ArgumentException("The specified position and count are out of bounds.");

            data = new byte[count];
            Array.Copy(_data, index, data, 0, count);
            return data;
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {

            byte[] data;

            // Check out of bounds
            if ((_position + count) > _data.Length)
                throw new ArgumentException("The specified count is out of bounds.");

            data = new byte[count];
            Array.Copy(_data, _position, data, 0, count);
            _position += count;
            return data;
        }

        /// <summary>
        /// Reads and returns the next byte.
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            if (_position > _data.Length)
                return 0;
            return _data[_position++];
        }

        public UInt16 ReadUInt16()
        {
            return (UInt16)(ReadByte() << 8 | ReadByte());
        }

        public Int16 ReadInt16()
        {
            return (Int16)(ReadByte() << 8 | ReadByte());
        }

        public UInt32 ReadUInt32()
        {
            return (UInt32)(ReadByte() << 24 | ReadByte() << 16 | ReadByte() << 8 | ReadByte());
        }

        public string ReadDomainName()
        {

            // Declare variables
            StringBuilder sBuilder = null;
            byte length;
            string chunk;
            byte[] chunkData;

            while ((length = ReadByte()) != 0)
            {

                // Check if this name is a reference.
                if ((length & 0xC0) == 0xC0)
                {

                    int oldPosition;
                    string referencedName;

                    // Resolve referenced position and store the current position (+1 byte for the byte read
                    // while resolving the referenced position).
                    oldPosition = _position + 1;
                    _position = ((length & 0x3F) << 8 | ReadByte());

                    // Read the referenced name and restore the current position.
                    referencedName = ReadDomainName();
                    _position = oldPosition;
                    if (sBuilder == null)
                        return referencedName;
                    else
                    {
                        sBuilder.Append(referencedName);
                        return sBuilder.ToString();
                    }
                }

                // Late string builder initilisation.
                if(sBuilder == null)
                    sBuilder = new StringBuilder();

                // Read the chuck and append it to the result string.
                chunkData = ReadBytes(length);
                chunk = Encoding.UTF8.GetString(chunkData);
                sBuilder.Append(chunk);
                sBuilder.Append('.');
            }

            // Return read name
            if (sBuilder == null)
                return ".";
            else
                return sBuilder.ToString();
        }

        /// <summary>
        /// Reads and returns the next resource record.
        /// </summary>
        /// <returns></returns>
        public ResourceRecord ReadRecord()
        {

            // Declare variables
            string domainName;
            ResourceRecordType type;

            // In order to be able to determine the type to instantiate to correct ResourceRecord type class, it
            // is required to read part of the resource record ahead. Performance wasting is prevented by supplying
            // the read ahead data to the ReasourceRecord inheriting class constructor.
            domainName = ReadDomainName();
            type = (ResourceRecordType)ReadUInt16();
            switch (type)
            {
                case ResourceRecordType.A:
                    return new ARecord(this, domainName);
                case ResourceRecordType.AAAA:
                    return new AAAARecord(this, domainName);
                case ResourceRecordType.CNAME:
                    return new CNAMERecord(this, domainName);
                case ResourceRecordType.MX:
                    return new MXRecord(this, domainName);
                case ResourceRecordType.NS:
                    return new NSRecord(this, domainName);
                default:
                    return new UnknownRecord(this, domainName, type);
            }
        }

        #endregion
    }
}
