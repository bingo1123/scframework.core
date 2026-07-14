using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// 一周内已设定的班次时间
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("一周内已设定的班次时间")]
    public struct SHFT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DaysPerWeek * ShiftsPerDay)]
        public TIME[][] Shift;  // 7天，每天最多4个班次

        public const int ShiftsPerDay = 4; // 每天的班次数量
        public const int DaysPerWeek = 7; // 一周的天数

    }
}
