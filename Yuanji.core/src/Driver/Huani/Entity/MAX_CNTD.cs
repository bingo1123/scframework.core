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
    /// node 2,func:2--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("MAX 班次数据信息")]
    public struct MAX_CNTD
    {
        [Description("MAX滤棒消耗（支）")]
        public int Filter_Rods_Used;         // MAX滤棒消耗（支）

        [Description("MAX盘纸消耗（盘）")]
        public int Tipping_Bobbins_Used;         // MAX盘纸消耗（盘）

        [Description("MAX胶水消耗（公斤）")]
        public int MAX_Glue_Used;         // MAX胶水消耗（公斤）

        [Description("卷烟纸消耗（盘）")]
        public int SE_Bobbins_Used;         // 卷烟纸消耗（盘）

        [Description("烟丝消耗（公斤）")]
        public int Tobacco_used;         // 烟丝消耗（公斤）

        [Description("SE胶水消耗（公斤）")]
        public int SE_Glue_Used;         // SE胶水消耗（公斤）

        [Description("气耗（立方米）")]
        public int CompressedAir_used;         // 气耗（立方米）

        [Description("电耗（度）")]
        public int Power_Used;         // 电耗（度）

        [Description("卷烟纸拼接头（个）")]
        public int SE_splice;         // 卷烟纸拼接头（个）

        [Description("水松纸拼接头（个）")]
        public int Tippingsplices;         // 水松纸拼接头（个）

        [Description("手动剔除废烟")]
        public int C_ManualEjection;         // 手动剔除废烟

        [Description("水松纸消耗（米）")]
        public int Tipping_Used;         // 水松纸消耗（米）

        [Description("卷烟纸消耗（米）")]
        public int CigarettePaper_Used;         // 卷烟纸消耗（米）

        [Description("备用")]
        public int SE_splice_inner;         // 备用

        [Description("备用")]
        public int Tipping_splices_inner;         // 备用

        [Description("备用")]
        public int MAX_monitor_Area1;         // 备用

        [Description("备用")]
        public int MAX_monitor_Area2;         // 备用

        [Description("备用")]
        public int MAX_monitor_Area3;         // 备用

        [Description("备用")]
        public int MAX_monitor_Area4;         // 备用

        [Description("备用")]
        public int Reserved_17;         // 备用

        [Description("备用")]
        public int Reserved_18;         // 备用

        [Description("备用")]
        public int Reserved_19;         // 备用

        [Description("备用")]
        public int Reserved_20;         // 备用
    }
}
