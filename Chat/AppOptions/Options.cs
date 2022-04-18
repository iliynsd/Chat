using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.AppOptions
{
   public class Options
    {
        public const string ApplicationPath = "appPath";
        public string Protocol { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
    }
}
