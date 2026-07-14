using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    public class FxsZB48ProductionData
    {
        public string BrandName { get; set; }

        public string BrandCode { get; set; }

        public string ProductStartTime { get; set; }

        public string ProductTime { get; set; }

        public int MachineCycles { get; set; }

        public int GoodProducts { get; set; }

        public int WasteProducts { get; set; }

        public int CigFault { get; set; }

        public int FPIbypass { get; set; }

        public int BrandCount { get; set; }

        public int Dummy { get; set; }

    }
}
