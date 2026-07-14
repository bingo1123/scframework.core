
using System.ComponentModel;

namespace  YS.Yuanji.Commom
{
    public class AddressItemsList
    {
        public DateTime LastIntervalTime = DateTime.Now;
        public List<AddressItems> Params;
    }

    public class AddressItems
    {
        public IntervelEnum Key { get; set; }

        public bool Enabled { get; set; }

        public int Interval { get; set; } = 3000;

        public List<Item> SymbAdrs { get; set; } = new List<Item>();

        public string Extended { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }=Guid.NewGuid().ToString();

        [Description("数据名称")]
        public string Name { get; set; }


        [Description("数据编码")]
        public string Code { get; set; }

        [Description("数据地址")] 
        public string Address { get; set; }

        [Description("数据类型")] 
        public string ValueType { get; set; }

        [Description("地址类型")]
        public string AddressType { get; set; }

        public string StartPosition { get; set; }

        [Description("地址读写")]
        public string Authority { get; set; }

        public int AddressLen { get; set; }

        [Description("地址计算表达式")]
        public string Linear { get; set; }
    }

}
