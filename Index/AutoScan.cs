using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Warc.Index
{
    /// <summary>
    /// Automaticially tries to find the most appropriate method for scanning the WARC contents.
    /// </summary>
    class AutoScan : IIndexer
    {

        public AutoScan(FileInfo fi, Stream s)
        {
            stream = s;
            fileInfo = fi;
        }

        private Stream stream;
        private FileInfo fileInfo;
        private IIndexer child;

        public override List<WarcRequest> WarcRequests { get; protected set; }
        public override List<WarcResponse> WarcResponses { get; protected set; }
        public override void Scan()
        {
            //Check for additional GZIP field.
            Debug.WriteLine("Checking if the WARC_EXTRA_DATA field is present...");
            stream.Position = 0;
            byte[] buffer = new byte[12];
            stream.Read(buffer, 0, 12);
            
            bool hasExtraFields = (buffer[3] & 0x04) != 0;
            if (hasExtraFields && buffer[10] == 12)
            {
                Debug.WriteLine("It is - let's use it!");
                child = new WarcExtraDataScan(stream);
                child.Scan();
                WarcRequests = child.WarcRequests;
                WarcResponses = child.WarcResponses;
                return;
            }
            else
            {
                Debug.WriteLine("It isn't - the opening may take some time...");
                child = new SelfScan(stream);
                child.Scan();
                WarcRequests = child.WarcRequests;
                WarcResponses = child.WarcResponses;
                return;
            }
        }
    }
}
