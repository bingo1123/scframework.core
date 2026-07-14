using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("每小时的废品率")]
    public struct WASP
    {
        [Description("每小时的 MAX 产量")]
        public uint MAX_Production;                 // 小时更迭时的 MAX 生产

        [Description("每小时的 SE 产量")]
        public uint SE_TotalProduction;             // 小时更迭时的 SE 生产

        [Description("班次更迭时的 MAX 生产")]
        public uint MAX_PrevProduction;             // 班次更迭时的 MAX 生产

        [Description("班次更迭时的 SE 生产")]
        public uint SE_PrevTotalProduction;         // 班次更迭时的 SE 生产

        [Description("实时小时的剔废百分比")]
        public float WastePercent_act;              // 实时小时的剔废百分比

        [Description("第１小时的剔废百分比")]
        public float WastePercent_1;                // 第１小时的剔废百分比

        [Description("第2小时的剔废百分比")]
        public float WastePercent_2;                // 第２小时的剔废百分比

        [Description("第3小时的剔废百分比")]
        public float WastePercent_3;                // 第３小时的剔废百分比

        [Description("第4小时的剔废百分比")]
        public float WastePercent_4;                // 第４小时的剔废百分比

        [Description("第5小时的剔废百分比")]
        public float WastePercent_5;                // 第５小时的剔废百分比

        [Description("第6小时的剔废百分比")]
        public float WastePercent_6;                // 第６小时的剔废百分比

        [Description("第7小时的剔废百分比")]
        public float WastePercent_7;                // 第７小时的剔废百分比
    }
}
