using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Warc
{
    public class WarcBlock
    {
        internal WarcBlock(StreamReader str)
        {
            string line;
            string[] parameters;
            while((line = str.ReadLine()) != "")
            {
                parameters = line.Split(' ');
                switch (parameters[0])
                {
                    case "Content-Type:":
                        contentType = parameters[1];
                        break;
                    case "WARC-Date:":
                        date = DateTime.Parse(parameters[1]);
                        break;
                    case "WARC-Record-ID:":
                        parameters = parameters[1].Split(':');
                        parameters[2] = parameters[2].Replace(">", "");
                        recordId = Guid.Parse(parameters[2]);
                        break;
                    case "WARC-Filename:":
                        filename = parameters[1];
                        break;
                    case "WARC-Block-Digest:":
                        blockDigest = parameters[1];
                        break;
                    case "Content-Length:":
                        contentLength = UInt32.Parse(parameters[1]);
                        break;
                        //Extension 1 starts here
                    case "WARC-Target-URI:":
                        targetUri = parameters[1];
                        break;
                    case "WARC-IP-Address:":
                        ipAddress = IPAddress.Parse(parameters[1]);
                        break;
                    case "WARC-Warcinfo-ID:":
                        parameters = parameters[1].Split(':');
                        parameters[2] = parameters[2].Replace(">", "");
                        warcinfoId = Guid.Parse(parameters[2]);
                        break;
                        //Extension 2 starts here
                    case "WARC-Concurrent-To:":
                        parameters = parameters[1].Split(':');
                        parameters[2] = parameters[2].Replace(">", "");
                        concurrentTo = Guid.Parse(parameters[2]);
                        break;
                    case "WARC-Payload-Digest:":
                        payloadDigest = parameters[1];
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Don't know what the WARC Tag \"{0}\" means.", parameters[0]));
                }
            }
        }

        string contentType;
        DateTime date;
        Guid recordId;
        string filename;
        string blockDigest;
        uint contentLength;
        string targetUri;
        IPAddress ipAddress;
        Guid warcinfoId;
        Guid concurrentTo;
        string payloadDigest;

        public GzipHeader GzipHeader { get; internal set; }

        public Guid ConcurrentTo
        {
            get
            {
                return concurrentTo;
            }
        }

        public Guid RecordId
        {
            get
            {
                return recordId;
            }
        }

        public override string ToString()
        {
            return recordId.ToString();
        }
    }
}
