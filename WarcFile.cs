using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Warc
{
    public class WarcFile : IDisposable
    {
        const ushort WA = 16727;
        BinaryReader br;

        List<GzipHeader> gzHeaders;

        public WarcFile(Stream s)
        {
            //Read the first two bytes, to decide wheter this file is compressed, or not.
            br = new BinaryReader(s);
            ushort magic = br.ReadUInt16();
            br.BaseStream.Position -= 2;
            if (magic == WA)
            {
                throw new NotSupportedException("Uncompressed WARC files not supported.");
            }
            else if (magic != GzipHeader.GZIP)
            {
                throw new InvalidDataException("This doesn't look like a WARC file.");
            }

            //Look at all the GZIP entries
            ScanGzipEntries();

            //Find all the Files inside the WARC =)
            List<WarcRequest> requests = new List<WarcRequest>();
            List<WarcResponse> responses = new List<WarcResponse>();
            res = new List<WarcResource>();

            GzipHeader currentHdr;
            for (int i = 0; i < gzHeaders.Count; i++)
            {
                if (i % 2000 == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Parsing WARC... {0}%", (int)(((double)i * 100.0d) / (double)gzHeaders.Count));
                }

                currentHdr = gzHeaders[i];
                Stream entryStream = currentHdr.GetStream();
                StreamReader str = new StreamReader(entryStream, Encoding.ASCII);
                string warcVersion = str.ReadLine();
                if (!warcVersion.StartsWith("WARC"))
                {
                    throw new NotImplementedException("doesn't look like a WARC file.");
                }
                string type = str.ReadLine().Split(' ')[1];
                switch (type)
                {
                    case "warcinfo":
                        if (info != null) break;    //who gives a digit about zetta slow megawarcs? I don't. If you don't like this behaviour, change it. Also speeds up a little.
                        info = new WarcInfo(str);
                        info.GzipHeader = currentHdr;
                        break;
                    case "request":
                        WarcRequest requ = new WarcRequest(str);
                        requ.GzipHeader = currentHdr;
                        requests.Add(requ);
                        break;
                    case "response":
                        WarcResponse resp = new WarcResponse(str);
                        resp.GzipHeader = currentHdr;
                        responses.Add(resp);
                        break;
                    case "metadata":
                        meta = new WarcMetadata(str);
                        meta.GzipHeader = currentHdr;
                        break;
                    case "resource":
                        WarcResource newRes = new WarcResource(str);
                        newRes.GzipHeader = currentHdr;
                        this.res.Add(newRes);
                        break;
                    default:
                        throw new NotImplementedException(type);
                }
            }

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
        }

        public WarcFile(FileInfo fi)
            : this(fi.OpenRead())
        {
        }

        public WarcFile(string s)
            : this(new FileInfo(s))
        {

        }

        private void ScanGzipEntries()
        {
            GzipHeader hdr;
            while (br.BaseStream.Position != br.BaseStream.Length)
            {
                hdr = new GzipHeader(br.BaseStream);
                if (gzHeaders == null) gzHeaders = new List<GzipHeader>();
                gzHeaders.Add(hdr);
                br.BaseStream.Position = hdr.GzipHeaderOffset + hdr.CompressedSize;
            }
        }

        WarcInfo info;
        WarcMetadata meta;
        List<WarcResource> res;
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
            br.Dispose();
        }

        public WarcInfo Info
        {
            get
            {
                return info;
            }
        }
    }
}
