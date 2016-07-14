using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Warc
{
    /// <summary>
    /// The compression classes in the .NET Framework are quite buggy. They read more from a backing stream than they should.
    /// This class can be used to tame them.
    /// See: http://stackoverflow.com/a/22062872
    /// </summary>
    class StreamBrake : Stream
    {
        public StreamBrake(Stream parent)
        {
            this.parent = parent;
        }

        private Stream parent;

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return parent.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a byte from the parent stream.
        /// </summary>
        /// <param name="buffer">The Buffer to store the read byte.</param>
        /// <param name="offset">Offset in the buffer where the byte should be put</param>
        /// <param name="count">Ignored</param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count == 0) return 0;

            if (parent.Read(buffer, offset, 1) != 1)
            {
                throw new IOException("Parent stream did something unexpected.");
            }
            return 1;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead {
            get { return parent.CanRead; }
        }

        public override bool CanSeek { get { return parent.CanSeek; } }

        public override bool CanWrite { get { return false; } }
        public override long Length {
            get { return parent.Length; }
        }

        public override long Position { get { return parent.Position; } set { parent.Position = value; } }
        
    }
}
