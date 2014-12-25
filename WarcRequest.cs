using System;
using System.Collections.Generic;
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
            httpRequest = str.ReadLine();

            string line;
            string[] parameters;
            while ((line = str.ReadLine()) != "")
            {
                parameters = line.Split(' ');
                switch (parameters[0])
                {
                    case "User-Agent:":
                        userAgent = String.Join(" ", parameters, 1, parameters.Length - 1);
                        break;
                    case "Accept:":
                        accept = parameters[1];
                        break;
                    case "Host:":
                        host = parameters[1];
                        break;
                    case "Connection:":
                        connection = parameters[1];
                        break;
                        //Extension 1
                    case "Referer:":
                        referer = parameters[1];
                        break;
                        //Extension 2
                    case "Cookie:":
                        cookie = parameters[1];
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Don't know what the WARC Tag \"{0}\" means.", parameters[0]));
                }
            }
        }

        string httpRequest;
        string userAgent;
        string accept;
        string host;
        string connection;
        string referer;
        string cookie;

        public string HttpRequestLine
        {
            get
            {
                return httpRequest;
            }
        }

        public string HttpRequestPath
        {
            get
            {
                return httpRequest.Split(' ')[1];
            }
        }
    }
}
