using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  Yuanji.core.src.Driver.Huani.Controller;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// M5 当前班次数据
    /// </summary>
    [Serializable]
    [Description("当前班次数据")]
    public struct BOBMeShiftData
    {
        [Description("当前班次日期")]
        public uint ShiftData;            // 当前班次日期

        [Description("当前班次开始时间")]
        public uint ShiftStart;            // 当前班次开始时间

        [Description("班次总时长(秒)")]
        public uint ShiftTime;             // 班次总时长（秒）

        [Description("运行时间（秒）")]
        public uint RunTime;               // 运行时间（秒）

        [Description("停机时间（秒）")]
        public uint StopTime;              // 停机时间（秒）

        [Description("停机次数")]
        public uint StopCnt;               // 停机次数

        [Description("基于时间的机器效率")]
        public uint EffMachineT;          // 基于时间的机器效率

        [Description("基于时间的生产效率")]
        public uint EffProductionT;       // 基于时间的生产效率

        [Description("盘纸更换(时段)")]
        public uint TotalBobbins;    // 盘纸更换(时段)
    }

    /// <summary>
    /// M5 当前班次数据
    /// </summary>
    [Serializable]
    [Description("当前设备数据")]
    public struct BOBMeCurrentData
    {
        [Description("速度mm/分钟")]
        public ushort SpeedOfBOB;               // 速度mm/分钟

        [Description("右盘纸直径(mm)")]
        public ushort DiameterRightBobbi;          // 右盘纸直径(mm)

        [Description("左盘纸直径(mm)")]
        public ushort DiameterLeftBobbin;       // 左盘纸直径(mm)

        [Description("盘纸更换(全部)")]
        public uint Bobinchangecounter;    // 盘纸更换(全部)
    }


    public class BOBMeDataParser 
    {
        public static BOBMeShiftData BOBProductDataParse(byte[] response)
        {
            var result = new BOBMeShiftData();
            //数据类型，数据的开始地址即可，这样就实现了读取数据地址，读取数据需要符号地址，根据符号返回地址
            int offset = HuaniConst.M5ReadDataOffset;//数据开始位73，其下一个数据地位为74
            // 手动解析每个字段并设置值
            result.ShiftData = BitConverter.ToUInt32(response, offset); offset += 74;
            result.ShiftStart = BitConverter.ToUInt32(response, offset); offset += 74;
            result.ShiftTime = BitConverter.ToUInt32(response, offset); offset += 74;
            result.RunTime = BitConverter.ToUInt32(response, offset); offset += 74;
            result.StopTime = BitConverter.ToUInt32(response, offset); offset += 74;
            result.StopCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.EffMachineT = BitConverter.ToUInt32(response, offset); offset += 74;
            result.EffProductionT = BitConverter.ToUInt32(response, offset); offset += 74;
            result.TotalBobbins = BitConverter.ToUInt32(response, offset); offset += 74;
            return result;
        }

        public static BOBMeCurrentData BOBCurrentDataParse(byte[] response)
        {
            var result = new BOBMeCurrentData();
            int offset = HuaniConst.M5ReadDataOffset;
            // 手动解析每个字段并设置值
            result.SpeedOfBOB = BitConverter.ToUInt16(response, offset); offset += 72;
            result.DiameterRightBobbi = BitConverter.ToUInt16(response, offset); offset += 72;
            result.DiameterLeftBobbin = BitConverter.ToUInt16(response, offset); offset += 72;
            result.Bobinchangecounter = BitConverter.ToUInt32(response, offset); offset += 74;
            return result;
        }
    }
}
