using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Commom
{
    public class WriteModel
    {
        public Item Key { get; set; }

        public object Value { get; set; }  

        public WriteModel(Item key, object value)
        {
            Key = key;
            Value = value;
        }

        public WriteModel()
        {
        }

        public WriteModel(Item key, object value ,int delay = 10)
        {
            Key = key;
            Value = value;
            Delay = delay;
        }

        public int Delay { get; set; } = 10;
    }
}
