using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Warc
{
    class WarcResource : WarcBlock
    {
        public WarcResource(StreamReader str)
            : base(str)
        {
        }
    }
}
