using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Warc
{
    public interface WarcFilesystemEntry
    {
        string Filename
        {
            get;
        }
        long Length
        {
            get;
        }
        Stream GetStream();
    }
}
