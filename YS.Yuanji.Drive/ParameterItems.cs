using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;

namespace YS.Yuanji.Drive
{
    public class DataConfig
    {
        public List<ParameterItems> Params;
    }

    public class ParameterItems
    {
        public string Key { get; set; }

        public bool Enabled { get; set; }

        public int Interval { get; set; } = 3000;

        public List<Item> SymbAdrs { get; set; } = new List<Item>();

        public string Extended { get; set; }
    }
   
}
