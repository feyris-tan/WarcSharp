using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;

namespace Warc
{
    public class WarcRequest : WarcBlock
    {
        internal WarcRequest(StreamReader str)
            : base(str)
        {
            HttpRequestLine = str.ReadLine();
            requestData = new NameValueCollection();

            string line;
            string[] parameters;
            while ((line = str.ReadLine()) != "")
            {
                parameters = line.Split(' ');
                requestData.Add(parameters[0], line.Substring(parameters[0].Length).Trim());
            }
        }

        private NameValueCollection requestData;
        public string HttpRequestLine { get; private set; }

        public string HttpRequestPath
        {
            get
            {
                return HttpRequestLine.Split(' ')[1];
            }
        }

        public string Host
        {
            get { return requestData["Host:"]; }
        }

        public NameValueCollection RequestHeaders
        {
            get { return requestData; }
        }
    }
}
