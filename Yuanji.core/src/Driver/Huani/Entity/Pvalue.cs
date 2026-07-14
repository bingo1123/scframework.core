using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    public enum PvalueFormatEnum 
    {
        valueBool = 0,
        valueByte = 1,
        valueSByte = 2,
        valueUInt16 = 3,
        valueInt16 = 4,
        valueUInt32 = 5,
        valueInt32 = 6,
        valueFloat = 7,
        valueInt16StringTID = 8,
        valueInt32StringTID = 14,
        valueStartEndTime = 15,
        value64StartEndTime = 10,
        ValueDate = 12,
        ValueTime = 13,
        valueString = 11,
        ValueButton = 9
    }

    // <summary>
    /// 参数值
    /// </summary>
    [Serializable]
    public struct Pvalue
    {
        //Parameter number = kkffnnnn kk = node address 0 ... 255 
        //ff = function number 0 ... 255 
        //nnnn = parameter no. 00 .. FFFF
        /// <summary>
        /// 节点地址
        /// </summary>
        public byte nodeAddress { get; set; }
        /// <summary>
        /// 功能号
        /// </summary>
        public byte functionNumber { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public short number { get; set; }

        public string ParaNumerName { get; set; }

        /// <summary>
        /// 参数格式
        /// </summary>
        public byte format { get; set; }


        public PvalueFormatEnum pvalueFormat { get; set; }


        public object ParameterValue { get; set; }
    }
}
