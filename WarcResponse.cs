using System;
using System.Collections.Generic;
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
            httpResponse = str.ReadLine();

            string line;
            string[] parameters;
            while ((line = str.ReadLine()) != "")
            {
                parameters = line.Split(' ');
                switch (parameters[0])
                {
                    case "Date:":
                        date = DateTime.Parse(String.Join(" ", parameters, 1, parameters.Length - 1));
                        break;
                    case "Server:":
                        server = String.Join(" ", parameters, 1, parameters.Length - 1);
                        break;
                    case "Last-Modified:":
                        lastModified = DateTime.Parse(String.Join(" ", parameters, 1, parameters.Length - 1));
                        break;
                    case "ETag:":
                        etag = parameters[1].Split('\"')[1];
                        break;
                    case "Accept-Ranges:":
                        acceptRanges = parameters[1];
                        break;
                    case "Content-Length:":
                        contentLength = Int64.Parse(parameters[1]);
                        break;
                    case "Keep-Alive:":
                        keepAlive = String.Join(" ", parameters, 1, parameters.Length - 1);
                        break;
                    case "Connection:":
                        connection = parameters[1];
                        break;
                    case "Content-Type:":
                        contentType = parameters[1];
                        break;
                        //extension 1
                    case "Location:":
                        location = parameters[1];
                        break;
                    case "Transfer-Encoding:":
                        transferEncoding = parameters[1];
                        break;
                        //extension 2
                    case "Set-Cookie:":
                        if (cookies == null) cookies = new List<string>();
                        cookies.Add(String.Join(" ", parameters, 1, parameters.Length - 1));
                        break;
                    case "Content-Disposition:":
                        contentDisposition = line.Substring(parameters[0].Length + 1);
                        break;
                    case "CF-Cache-Status:":
                        cfCacheStatus = parameters[1];
                        break;
                    case "Expires:":
                        expires = DateTime.Parse(String.Join(" ", parameters, 1, parameters.Length - 1));
                        break;
                    case "Cache-Control:":
                        cacheControl = line.Substring(parameters[0].Length + 1);
                        break;
                    case "CF-RAY:":
                        cfRay = parameters[1];
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Don't know what the WARC Tag \"{0}\" means.", parameters[0]));
                }
            }
        }

        string httpResponse;
        DateTime date;
        string server;
        DateTime lastModified;
        string etag;
        string acceptRanges;
        long contentLength;
        string keepAlive;
        string connection;
        string contentType;
        string location;
        string transferEncoding;
        List<string> cookies;
        string contentDisposition;
        string cfCacheStatus;
        DateTime expires;
        string cacheControl;
        string cfRay;

        public long ContentLength
        {
            get
            {
                return contentLength;
            }
        }
    }
}
