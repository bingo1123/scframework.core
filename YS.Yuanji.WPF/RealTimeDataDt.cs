using System;
using System.Collections.Generic;

namespace YS.Yuanji.WPF
{
    public class RealTimeDataDto
    {
        public long Ts { get; set; }
        public string MachineId { get; set; }
        public List<DataItemDetailDto> Details { get; set; }
    }

    public class DataItemDetailDto
    {
        public string Code { get; set; }
        public object Value { get; set; }
        public bool IsGood { get; set; }

        public string DType { get; set;}
    }
}