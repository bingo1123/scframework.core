using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Controller
{
    public enum VisuCommandNameEnum : byte
    {
        ReadData = 0x11,//读取数据

        ReadBrandParameter = 0x21,//读取品牌参数

        ReadMachineParameter = 0x31,//读取机台参数

        /// <summary>
        /// 在ZJ16机型中的功能是ReadMLParameter
        /// </summary>
        ReadParameter = 0x51,//读取参数

        ReadMessageParameter = 0x61,//读取信息参数

        /// <summary>
        /// PROTOS 1–8, 对应ZJ16机型
        /// </summary>
        //ReadShiftDataOnlyFor1to8 = 0x83,
        WriteBrandParameter = 0x23,
        WriteMachineParameter = 0x33,
        Others = 0xFF
    }
}
