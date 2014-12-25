using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Warc
{
    class WarcMetadata : WarcBlock
    {
        public WarcMetadata(StreamReader str)
            : base(str)
        {
        }
    }
}
