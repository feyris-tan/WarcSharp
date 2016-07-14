using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;

namespace Warc
{
    public class WarcInfo : WarcBlock
    {
        public WarcInfo(StreamReader str)
            : base(str)
        {
            headers = new NameValueCollection();
            string line;
            string[] parameters;
            while ((line = str.ReadLine()) != "")
            {
                parameters = line.Split(' ');
                headers.Add(parameters[0], parameters[1]);
            }
        }

        NameValueCollection headers;
    }
}
