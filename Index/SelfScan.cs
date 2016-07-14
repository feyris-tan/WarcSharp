using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
namespace Warc.Index
{
    /// <summary>
    /// Use this if you neither have a corresponding meta warc, nor a WAT file - and the WARC does not contain the WARC_EXTRA_DATA field.
    /// 
    /// Please note that this is really slow and error-prone, because it unpacks every entry in the WARC 
    /// file, to find Requests and Responses.
    /// </summary>
    class SelfScan : IIndexer
    {
        public SelfScan(Stream target)
        {
            this.target = target;
        }

        private Stream target;

        public override List<WarcRequest> WarcRequests { get; protected set; }
        public override List<WarcResponse> WarcResponses { get; protected set; }
        public override void Scan()
        {
            target.Position = 0;
            StreamBrake brake = new StreamBrake(target);    //GZIP Streams need to be wrapped, otherwise they won't play nicely...
            long counter = 0;
            string warcVersion;
            string temp = "";
            List<WarcRequest> requests = new List<WarcRequest>();
            List<WarcResponse> responses = new List<WarcResponse>();

            Debug.Write("Parsing WARC..");
            while (target.Position != target.Length)
            {
                if (counter++%2000 == 0)
                    Debug.Write(".");

                WarcInfo info = null;
                GzipLocation location = new GzipLocation(target, brake.Position);
                GZipStream gzStream = new GZipStream(brake, CompressionMode.Decompress);
                StreamReader str = new StreamReader(gzStream, Encoding.ASCII);
                IIndexer.HandleEntry(str, location, requests, responses);
                

                while (str.ReadLine() != null) ;
            }

            WarcRequests = requests;
            WarcResponses = responses;
        }

        
    }
}
