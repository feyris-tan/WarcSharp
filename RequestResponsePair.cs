using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public System.IO.Stream GetStream()
        {
            long junkSize = Response.GzipHeader.UncompressedSize - (Response.ContentLength + 4);
            Stream gz = Response.GzipHeader.GetStream();
            byte[] junk = new byte[junkSize];
            gz.Read(junk, 0, (int)junkSize);
            return gz;
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
