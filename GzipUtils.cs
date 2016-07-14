using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Warc
{
    public static class GzipUtils
    {
        public const ushort GzipMagicNumber = 35615;
        public static bool isGzip(Stream s)
        {
            byte[] buffer = new byte[2];
            if (s.Read(buffer, 0, 2) != 2) return false;
            s.Position -= 2;
            return BitConverter.ToUInt16(buffer, 0) == GzipMagicNumber;
        }
    }

    public struct GzipLocation
    {
        public GzipLocation(Stream str, long offset)
        {
            this.str = str;
            this.offset = offset;
        }
        public Stream str;
        public long offset;

        public override string ToString()
        {
            return string.Format("{0}@{1}", str.ToString(), offset);
        }
    }
    
}
