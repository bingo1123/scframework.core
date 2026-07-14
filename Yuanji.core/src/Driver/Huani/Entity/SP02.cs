using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using  Yuanji.core.src.Driver.Huani.Controller;
using YS.Yuanji.Commom;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SP02:MLP/HMI parameters")]
    public struct SP02
    {
        [Description("班次号")]
        public byte ShiftNo;

        [Description("语言设置")]
        public ushort Language;

        [Description("机器速度设定")]
        public ushort MachineSpeed;

        [Description("自动故障模拟")]
        public bool ForceReport;    //长度四个字节


        [Description("当前品牌")]
        public ushort ActBrand;


        [Description("编辑的品牌")]
        public ushort EditBrand;


        [Description("机器名称")]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_MACH_NAMLEN + 1)]
        public string UnitName; //长度40


        //----以下是ZJ19多出来的字段
        [Description("维修时间")]
        public UInt32? RepairTime;

        [Description("清洁时间")]
        public UInt32? CleanupTime;

        [Description("字符集编号")]
        public ushort? FontSubstitutes;

        [Description("机器速度设定")]
        public ushort? AutoBufferChange;

        [Description("忽略换班")]
        public ushort? IgnoreShiftChange;

        [Description("停机自动退出取样")]
        public ushort? SuppressSampleAutoStop;
        // Constants
        public const int MAX_MACH_NAMLEN = 40;
    }

    public class SP02Parser
    {
        public static SP02 Parse(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.M5)
        {
            SP02 sp = new SP02();
            bool type = typeEnum == ProtocolTypeEnum.ZJ119;
            int offset = HuaniConst.P18ReadDataOffset;
            sp.ShiftNo = response[offset];
            sp.Language = BitConverter.ToUInt16(response, offset + 1);
            sp.MachineSpeed = BitConverter.ToUInt16(response, offset + 3);

            sp.ForceReport = BitConverter.ToUInt32(response, offset + 5) != 0;

            sp.ActBrand = BitConverter.ToUInt16(response, offset + 9);
            sp.EditBrand = BitConverter.ToUInt16(response, 11);
            sp.UnitName = Encoding.ASCII.GetString(response, 13, 41);
            if (type)
            {
                sp.RepairTime = BitConverter.ToUInt32(response, 54);

                sp.CleanupTime = BitConverter.ToUInt32(response, 58);

                sp.FontSubstitutes = BitConverter.ToUInt16(response, 62);

                sp.AutoBufferChange = BitConverter.ToUInt16(response, 64);

                sp.IgnoreShiftChange = BitConverter.ToUInt16(response, 66);

                sp.SuppressSampleAutoStop = BitConverter.ToUInt16(response, 68);
            }
            else
            {
                sp.RepairTime = null;
                sp.CleanupTime = null;
                sp.FontSubstitutes = null;
                sp.AutoBufferChange = null;
                sp.IgnoreShiftChange = null;
                sp.SuppressSampleAutoStop = null;
            }
            return sp;
        }
    }
}
