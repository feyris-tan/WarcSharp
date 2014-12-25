using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Warc
{
    public class WarcInfo : WarcBlock
    {
        public WarcInfo(StreamReader str)
            : base(str)
        {
            string line;
            string[] parameters;
            while ((line = str.ReadLine()) != "")
            {
                parameters = line.Split(' ');
                switch (parameters[0])
                {
                    case "software:":
                        software = parameters[1];
                        break;
                    case "format:":
                        format = String.Join(" ", parameters, 1, parameters.Length - 1);
                        break;
                    case "conformsTo:":
                        conformsTo = parameters[1];
                        break;
                    case "robots:":
                        robots = parameters[1];
                        break;
                    case "wget-arguments:":
                        wgetArguments = line.Substring(parameters[0].Length + 1);
                        break;
                    case "operator:":
                        operatorName = line.Substring(parameters[0].Length + 1);
                        break;
                    case "puush-dld-script-version:":
                        version = parameters[1];
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Don't know what the WARC Tag \"{0}\" means.", parameters[0]));
                }
            }
            str.Close();
        }

        string software;
        string format;
        string conformsTo;
        string robots;
        string wgetArguments;
        string operatorName;
        string version;

        public string WgetArguments
        {
            get
            {
                return wgetArguments;
            }
        }
    }
}
