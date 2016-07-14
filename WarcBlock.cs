using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            warcBlockData = new NameValueCollection();
            string line;
            string[] parameters;
            while((line = str.ReadLine()) != "")
            {
                parameters = line.Split(' ');
                warcBlockData.Add(parameters[0], parameters[1]);
            }
        }

        internal GzipLocation GzipLocation { get; set; }

        public Guid ConcurrentTo
        {
            get
            {
                string result = warcBlockData["WARC-Concurrent-To:"];
                string[] parameters = result.Split(':');
                parameters[2] = parameters[2].Replace(">", "");
                return Guid.Parse(parameters[2]);
            }
        }

        public Guid RecordId
        {
            get
            {
                string result = warcBlockData["WARC-Record-ID:"];
                string[] parameters = result.Split(':');
                parameters[2] = parameters[2].Replace(">", "");
                return Guid.Parse(parameters[2]);
            }
        }

        public long WarcContentLength
        {
            get
            {
                string result = warcBlockData["Content-Length:"];
                if (result == null) return 0;
                return Convert.ToInt64(result);
            }
        }

        public override string ToString()
        {
            if (!warcBlockData.AllKeys.Contains("WARC-Record-ID:")) return "<no record id>";
            return RecordId.ToString();
        }

        NameValueCollection warcBlockData;
    }
}
