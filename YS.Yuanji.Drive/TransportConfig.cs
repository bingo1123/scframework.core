using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Drive
{
    public class TransportConfig
    {
        public string TransName { get; set;}

        public string TransID { get; set; }

        public string TransType { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public Dictionary<string,string> ParameterDict { get; set; }
    }
}
