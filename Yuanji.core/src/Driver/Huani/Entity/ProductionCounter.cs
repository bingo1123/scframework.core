using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using  Yuanji.core.src.Driver.Huani.Controller;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Description("当前生产数据")]
    [Serializable]
    public struct ProductionCounter
    {
        [Description("总生产数量[1000支]")]
        public uint? TotalProduction;       // 总生产数量 [1000 Cig]

        [Description("总废料数量[支]")]
        public uint TotalWasteCount;       // 总废料数量 [Cig]

        [Description("总废料百分比[%]")]
        public float TotalWastePercent;    // 总废料百分比 [%]

        [Description("生产目标[1000支]")]
        public uint ProductionTarget;      // 生产目标 [1000 Cig]

        [Description("生产预估[1000支]")]
        public uint ProductionEstimation;  // 生产预估 [1000 Cig]

        [Description("生产预估[支]")]
        public uint MaxProduction;         // 最大生产数量 [Cig]

        [Description("外部废料数量")]
        public uint ExternalWasteCount;    // 外部废料数量，例如每次启动浪费 200 个

        [Description("运行阶段的生产损失")]
        public uint ExternalLossCount;     // 运行阶段的生产损失

        [Description("因速度降低导致的生产损失")]
        public uint SpeedReductionLossCount;  // 因速度降低导致的生产损失

        [Description("因检测到异物而造成的废料")]
        public uint ForeignBodyWasteCount; // 因检测到异物而造成的废料

        [Description("内部废料数量")]
        public uint InternalWasteCount;    // 内部废料数量 = 总废料 - 外部废料 - 异物废料
    }

    public class ProductionCounterParser
    {
        public static ProductionCounter Parse(byte[] response)
        {
            ProductionCounter counter = new ProductionCounter();
            int offset = HuaniConst.M5ReadDataOffset;

            counter.TotalProduction = BitConverter.ToUInt32(response, offset);
            counter.TotalWasteCount = BitConverter.ToUInt32(response, offset + 4);
            counter.TotalWastePercent = BitConverter.ToSingle(response, offset + 8);
            counter.ProductionTarget = BitConverter.ToUInt32(response, offset + 12);
            counter.ProductionEstimation = BitConverter.ToUInt32(response, offset + 16);
            counter.MaxProduction = BitConverter.ToUInt32(response, offset + 20);
            counter.ExternalWasteCount = BitConverter.ToUInt32(response, offset + 24);
            counter.ExternalLossCount = BitConverter.ToUInt32(response, offset + 28);
            counter.SpeedReductionLossCount = BitConverter.ToUInt32(response, offset + 32);
            counter.ForeignBodyWasteCount = BitConverter.ToUInt32(response, offset + 36);
            counter.InternalWasteCount = BitConverter.ToUInt32(response, offset + 40);

            return counter;
        }
    }
}
