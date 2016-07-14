using System;
using System.Collections.Generic;
using System.IO;

namespace Warc.Index
{
    /// <summary>
    /// Supposed to provide means of finding all the entries in a WARC.
    /// </summary>
    public abstract class IIndexer
    {
        /// <summary>
        /// After calling Scan(), this should contain all the Requests in the WARC file.
        /// </summary>
        public abstract List<WarcRequest> WarcRequests { get; protected set; }

        /// <summary>
        /// After calling Scan(), this should contain all the Responses in the WARC file.
        /// </summary>
        public abstract List<WarcResponse> WarcResponses { get; protected set; }

        /// <summary>
        /// Populates Info, WarcRequests, and WarcResponses.
        /// Depending on the IIndexer implementation, this might take a long time.
        /// </summary>
        public abstract void Scan();

        protected static void HandleEntry(StreamReader str, GzipLocation location, List<WarcRequest> requests, List<WarcResponse> responses)
        {
            WarcInfo info = null;
            string warcVersion;
            warcVersion = str.ReadLine();
            if (!warcVersion.StartsWith("WARC"))
            {
                throw new InvalidDataException("doesn't look like a WARC file to me.");
            }
            string type = str.ReadLine().Split(' ')[1];
            switch (type)
            {
                case "warcinfo":
                    if (info != null) throw new Exception("Sorry, megawarcs are not yet supported.");
                    info = new WarcInfo(str);
                    info.GzipLocation = location;
                    break;
                case "request":
                    WarcRequest requ = new WarcRequest(str);
                    requ.GzipLocation = location;
                    requests.Add(requ);
                    break;
                case "response":
                    WarcResponse resp = new WarcResponse(str);
                    resp.GzipLocation = location;
                    responses.Add(resp);
                    break;
                case "metadata":
                    new WarcMetadata(str); //I don't care about metadata, so I'll just discard these.
                    break;
                case "resource":
                    new WarcResource(str); //The same applies to resources.
                    break;
                default:
                    throw new NotImplementedException(string.Format("Don't know what {0} means.", type));
            }
        }
    }
}