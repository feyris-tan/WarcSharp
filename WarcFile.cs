using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using Warc.Index;

namespace Warc
{
    public class WarcFile : IDisposable
    {
        public WarcFile(Stream s,IIndexer indexer = null)
        {
            if (indexer == null) indexer = new SelfScan(s);
            if (indexer.WarcResponses == null) indexer.Scan();           //If scan was not invoked before, invoke it now.
            else if (indexer.WarcResponses.Count == 0) indexer.Scan();

            var requests = indexer.WarcRequests;
            var responses = indexer.WarcResponses;

            Debug.WriteLine("\nMapping requests to responses...");
            //Map all the responses to their requests
            foreach (WarcRequest request in requests)
            {
                WarcResponse target = null;
                foreach (WarcResponse response in responses)
                {
                    if (response.ConcurrentTo == request.RecordId)
                    {
                        target = response;
                        break;
                    }
                }
                if (target == null) continue;
                if (entries == null) entries = new List<WarcFilesystemEntry>();
                responses.Remove(target);
                
                RequestResponsePair result = new RequestResponsePair();
                result.Request = request;
                result.Response = target;
                entries.Add(result);
            }
            Debug.WriteLine("Opening the WARC has been completed!");
        }

        public WarcFile(FileInfo fi)
            : this(fi.OpenRead(),new AutoScan(fi,fi.OpenRead()))
        {
        }

        public WarcFile(string s)
            : this(new FileInfo(s))
        {

        }

        private Stream s;
        List<WarcFilesystemEntry> entries;

        public List<WarcFilesystemEntry> FilesystemEntries
        {
            get
            {
                return entries;
            }
        }

        public void Dispose()
        {
            s.Dispose();
        }
    }
}
