using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Warc.Index;

namespace Warc.CDX
{
    /// <summary>
    /// Parses a CDX File in order to find the entries inside the WARC file.
    /// 
    /// The CDX file may either be uncompressed or GzipMagicNumber compressed.
    /// </summary>
    public class CDXParser : IIndexer
    {
        /// <summary>
        /// Prepares CDX parsing.
        /// </summary>
        /// <param name="cdxStream">The stream containing the CDX file.</param>
        /// <param name="warcIO">The WARC file matching the CDX file.</param>
        /// <param name="warcName">Tells the parser which WARC to look for inside the CDX. Since CDX may contain entries to multiple WARCs, 
        /// you have to specify the WARC name here.</param>
        public CDXParser(Stream cdxStream,Stream warcIO,string warcName)
        {
            this.warcIO = warcIO;
            this.warcName = warcName;

            if (GzipUtils.isGzip(cdxStream))
            {
                cdxStream = new GZipStream(cdxStream, CompressionMode.Decompress);
            }
            cdxIO = new StreamReader(cdxStream);
        }

        private Stream warcIO;
        private StreamReader cdxIO;
        private string warcName;
        

        public override List<WarcRequest> WarcRequests { get; protected set; }
        public override List<WarcResponse> WarcResponses { get; protected set; }
        public override void Scan()
        {
            string[] legend = cdxIO.ReadLine().Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
            if (legend[0] != "CDX") throw new InvalidDataException("Not a CDX file!");
            int fileNameField = 0, compressedOffsetField = 0, compressedSizeField = 0;

            for (int i = 0; i < legend.Length; i++)
            {
                if (legend[i].Equals("g")) fileNameField = i - 1;
                if (legend[i].Equals("V")) compressedOffsetField = i - 1;
                if (legend[i].Equals("S")) compressedSizeField = i - 1;
            }

            if (fileNameField == 0) throw new InvalidDataException("Can't determine WARC file name from this CDX!");
            if (compressedOffsetField == 0) throw new InvalidDataException("Can't determine compressed Offset fields from this CDX!");
            if (compressedSizeField == 0) throw new InvalidDataException("Can't determine compressed Size fields from this CDX!");

            string line;
            string[] lineArgs;
            string fname;
            long offset;
            int csize;
            while (!string.IsNullOrEmpty(line = cdxIO.ReadLine()))
            {
                lineArgs = line.Split(' ');
                fname = lineArgs[fileNameField];
                if (!fname.Contains(warcName)) continue;
                offset = Convert.ToInt64(lineArgs[compressedOffsetField]);
                csize = Convert.ToInt32(lineArgs[compressedSizeField]);

                warcIO.Position = offset;

                throw new NotImplementedException("CDX parser is not done yet!");
            }
        }
    }
}
