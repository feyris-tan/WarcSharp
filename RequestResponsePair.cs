using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Warc
{
    public class RequestResponsePair : WarcFilesystemEntry
    {
        internal RequestResponsePair()
        {
        }

        public WarcResponse Response { get; internal set; }
        public WarcRequest Request { get; internal set; }


        public string Filename
        {
            get 
            {
                string result = Request.HttpRequestPath;
                if (result.EndsWith("/"))
                {
                    result += "index.html";
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the content of the response as stream.
        /// </summary>
        /// <returns></returns>
        Stream GetStream()
        {
            Stream level1 = Response.GzipLocation.str;
            level1.Position = Response.GzipLocation.offset;
            GZipStream level2 = new GZipStream(level1, CompressionMode.Decompress, true);
            StreamBrake level3 = new StreamBrake(level2);
            StreamReader fastForwarder = new StreamReader(level3, Encoding.ASCII, false, 1);

            int emptyLines = 0;
            while (emptyLines != 2)
            {
                if (fastForwarder.ReadLine() == "") emptyLines++;
            }

            if (Response.ContentEncoding.Equals("gzip"))
            {
                //Sometimes, there is junk before the actual GZIP data starts! I could not figure out when and why, therefore i made this slow workaround:
                byte[] buffer = new byte[Response.WarcContentLength];
                level2.Read(buffer, 0, (int)Response.WarcContentLength);
                int gzipOffset = 0;
                while (buffer[gzipOffset] != 0x1F) gzipOffset++;
                MemoryStream level3a = new MemoryStream(buffer, gzipOffset, buffer.Length - gzipOffset);
                GZipStream level4 = new GZipStream(level3a, CompressionMode.Decompress);
                return level4;
            }
            return level2;
        }

        /// <summary>
        /// Returns the content of the response as byte array
        /// </summary>
        /// <returns></returns>
        public byte[] ExtractResponse()
        {
            MemoryStream temp = new MemoryStream();
            Stream input = GetStream();
            input.CopyTo(temp);
            byte[] buffer =  temp.GetBuffer();
            if (buffer.Length != temp.Position)
            {
                byte[] returnable = new byte[temp.Position];
                Array.Copy(buffer, returnable, returnable.Length);
                File.WriteAllBytes("test.bin", returnable);
                return returnable;
            }
            return buffer;
        }

        public override string ToString()
        {
            return Filename;
        }


        public long Length
        {
            get { return Response.ContentLength; }
        }
    }
}
