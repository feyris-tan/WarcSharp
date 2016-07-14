using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Warc
{
    public class WarcResponse : WarcBlock
    {
        internal WarcResponse(StreamReader str)
            : base(str)
        {
            responseData = new NameValueCollection();
            httpResponse = str.ReadLine();

            string line;
            string[] parameters;
            while (true)
            {
                line = str.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                parameters = line.Split(' ');
                responseData.Add(parameters[0], line.Substring(parameters[0].Length).Trim());
            }
            
        }

        private string httpResponse;
        private NameValueCollection responseData;

        public long ContentLength
        {
            get { return Convert.ToInt64(responseData["Content-Length:"]); }
        }

        public string ContentEncoding
        {
            get
            {
                if (!responseData.AllKeys.Contains("Content-Encoding:")) return "";
                return responseData["Content-Encoding:"];
            }
        }

        public NameValueCollection ResponseHeaders
        {
            get { return responseData; }
        }
    }
}
