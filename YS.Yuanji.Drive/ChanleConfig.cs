using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Drive
{
    public class ChanleConfig
    {
        public string ChannelName { get; set;}

        public string ChannelID { get; set; }

        public string ChannelType { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public Dictionary<string,string> ParameterDict { get; set; }
    }
}
