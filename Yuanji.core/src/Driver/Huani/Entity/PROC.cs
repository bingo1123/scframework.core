using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;
using  Yuanji.core.src.Driver.Huani.Controller;
using  Yuanji.core.src.Driver.ZB48A;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    ///<summary>
    /// P18 与ZJ19 : 前三班生产数据分别存储在PRO1、PRO2、PRO3中
    ///</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Description("当前班的生产数据")]
    [Serializable]
    public struct PROC
    {
        [Description("总生产量[千支]")]
        public uint TotalProduction;      // 总生产量 [支/千支]

        [Description("总废品数量[支]")]
        public uint TotalWasteCount;      // 总废品数量 [支]

        [Description("总废品率[%]")]
        public float TotalWastePercent;   // 总废品百分比 [%]

        [Description("机器效率[%]")]
        public float EffMachine;          // 机器效率 [%]

        [Description("生产效率[%]")]
        public float EffProduction;      // 生产效率 [%]

        [Description("目标产量[支]")]
        public uint ProductionTarget;     // 生产目标 [支]

        [Description("估算产量[支]")]
        public uint ProductionEstimation; // 生产估算 [支]

        [Description("最大良品生产量[支]")]
        public uint MAX_Production;      // 最大良品生产量 [支]

        [Description("SE良品生产量[支]")]
        public uint SE_Production;       // 良品生产量 [支]

        //------ZJ119单独多出的一个字段
        [Description("Imperial efficiency")]
        public float? ITL_Efficiency;
    }


    public struct TEST
    {
        [Description("生产速度")]
        public UInt16 MachineSpeed;      //速度
    }

    /// <summary>
    /// zj119数据格式.
    /// </summary>
    public struct GNRL
    {
        [Description("封口器温度")]
        public UInt16 Temperature1;      //速度

        [Description("封口器1温度")]
        public UInt16 Temperature2;      //速度

        [Description("烟枪温度")]
        public UInt16 Temperature3;      //速度

        [Description("生产速度")]
        public UInt16 MachineSpeed;      //速度
    }

    /// <summary>
    /// zj118数据格式.
    /// </summary>
    public struct zsgb
    {
        [Description("烙铁温度1")]
        public UInt16 Temperature1;     

        [Description("烙铁温度2")]
        public UInt16 Temperature2;     

        [Description("烟枪温度")]
        public UInt16 Temperature3;     
    
    }

    /// <summary>
    /// zj118数据格式.
    /// </summary>
    public struct entn
    {

        [Description("搓板温度")]
        public UInt16 Temperature4;

        [Description("水松纸温度")]
        public UInt16 Temperature5;

    }

    /// <summary>
    /// zj118数据格式.
    /// </summary>
    public struct tsth
    {
        [Description("进刀剩余数")]
        public UInt16 FeedResidue;
    }

    /// <summary>
    /// zj119数据格式.
    /// </summary>
    public struct MDRM
    {
        [Description("搓板实际温度")]
        public UInt16 Temperature4;      //速度

    }
    /// <summary>
    /// zj119数据格式.
    /// </summary>
    public struct TPGL_1
    {
        [Description("水松纸温度")]
        public UInt16 Temperature5;      //速度

    }

    /// <summary>
    /// zj116数据格式.
    /// </summary>
    public struct ZIGH
    {
        [Description("搓板温度")]
        public UInt16 Temperature4;      //速度

    }

    /// <summary>
    /// zj116数据格式.
    /// </summary>
    public struct BPTR
    {
        [Description("水松纸温度")]
        public UInt16 Temperature5;      //速度

    }

    /// <summary>
    /// zj116数据格式.
    /// </summary>
    public struct ITOS
    {
        [Description("吸丝带张紧压力S60V")]
        public float S60V;
        [Description("风室负压S63V")]
        public Int16 S63V;
        [Description("一次风选压力S61V")]
        public Int16 S61V;
        [Description("空气分配箱压力S62V")]
        public Int16 S62V;
        [Description("流化床顶部压力A571V")]
        public float A571V;

        [Description("MAX总气源S71M")]
        public float S71M;
        [Description("MAX切纸轮压力S70M")]
        public Int16 S70M;
        [Description("MAX风机负压")]
        public Int16 S7XX;
        [Description("MAX进气压力S77M")]
        public float S77M;

    }

    public class PROCParser
    {
        public static PROC Parse(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            PROC pROC = new PROC();
            int offset = HuaniConst.P18ReadDataOffset;//开始读取数据的地址是12 
            pROC.TotalProduction = BitConverter.ToUInt32(response, offset);//占四个字节，根据数据类型来确认
            pROC.TotalWasteCount = BitConverter.ToUInt32(response, offset + 4);
            pROC.TotalWastePercent = BitConverter.ToSingle(response, offset + 8);
            pROC.EffMachine = BitConverter.ToSingle(response, offset + 12);
            pROC.EffProduction = BitConverter.ToSingle(response, offset + 16);
            pROC.ProductionTarget = BitConverter.ToUInt32(response, offset + 20);
            pROC.ProductionEstimation = BitConverter.ToUInt32(response, offset + 24);
            pROC.MAX_Production = BitConverter.ToUInt32(response, offset + 28);
            pROC.SE_Production = BitConverter.ToUInt32(response, offset + 32);

            if (typeEnum == ProtocolTypeEnum.ZJ119)
                pROC.ITL_Efficiency = BitConverter.ToUInt32(response, offset + 36);
            else
                pROC.ITL_Efficiency = null;
            return pROC;
        }

        public static TEST ParseTEST(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            TEST tEST = new TEST();

            int offset = HuaniConst.P18ReadDataOffset;//开始读取数据的地址是12 
            if (typeEnum == ProtocolTypeEnum.P18)
            {
                tEST.MachineSpeed = BitConverter.ToUInt16(response, offset + 16);//占四个字节，根据数据类型来确认
            }
            else if (typeEnum == ProtocolTypeEnum.ZJ118)
            {

                tEST.MachineSpeed = BitConverter.ToUInt16(response, offset + 46);//占四个字节，根据数据类型来确认
            }
            else if (typeEnum == ProtocolTypeEnum.ZJ116A)
            {

                tEST.MachineSpeed = BitConverter.ToUInt16(response, offset + 16);//占四个字节，根据数据类型来确认
            }

            return tEST;
        }

        public static Int32 ParseTemp(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            int offset = HuaniConst.P18ReadDataOffset;//开始读取数据的地址是12 
            var temp = BitConverter.ToUInt16(response, offset);//占四个字节，根据数据类型来确认

            return temp;
        }

        public static zsgb Parsezsgb(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            int offset = 12;//开始读取数据的地址是12 
            zsgb zsgb = new zsgb();
            if (typeEnum == ProtocolTypeEnum.ZJ118)
            {
                zsgb.Temperature1 = BitConverter.ToUInt16(response, offset); offset += 2;//占四个字节，根据数据类型来确认
                zsgb.Temperature2 = BitConverter.ToUInt16(response, offset); offset += 4;//占四个字节，根据数据类型来确认
                zsgb.Temperature3 = BitConverter.ToUInt16(response, offset); offset += 2;//占四个字节，根据数据类型来确认
            }
            else if (typeEnum == ProtocolTypeEnum.ZJ116A)
            {
                zsgb.Temperature1 = BitConverter.ToUInt16(response, offset); offset += 3;//占四个字节，根据数据类型来确认
                zsgb.Temperature2 = BitConverter.ToUInt16(response, offset); offset += 2;//占四个字节，根据数据类型来确认
                zsgb.Temperature3 = BitConverter.ToUInt16(response, offset); offset += 2;//占四个字节，根据数据类型来确认
            }
           
            return zsgb;
        }

        public static tsth Parsetsth(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            int offset = 13;//开始读取数据的地址是12 
            tsth zsgb = new tsth();
            zsgb.FeedResidue = BitConverter.ToUInt16(response, offset); offset += 2;//占四个字节，根据数据类型来确认
            return zsgb;
        }

        public static entn Parseentn(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            int offset = 12;//开始读取数据的地址是12 
            entn zsgb = new entn();
            zsgb.Temperature4 = BitConverter.ToUInt16(response, offset); offset += 2;//占四个字节，根据数据类型来确认
            zsgb.Temperature5 = BitConverter.ToUInt16(response, offset); offset += 2;//占四个字节，根据数据类型来确认
            return zsgb;
        }

        public static GNRL ParseGNRL(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.ZJ119)
        {
            GNRL data = new GNRL();
            int offset = 112;//开始读取数据的地址是12 
            data.Temperature1 = BitConverter.ToUInt16(response, offset); offset += 5;
            data.Temperature2 = BitConverter.ToUInt16(response, offset); offset += 10;
            data.Temperature3 = BitConverter.ToUInt16(response, offset); offset += 65;
            data.MachineSpeed = BitConverter.ToUInt16(response, offset);
            return data;
        }

        public static MDRM ParseMDRM(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.ZJ119)
        {
            MDRM data = new MDRM();
            int offset = 23;//开始读取数据的地址是12 
            data.Temperature4 = BitConverter.ToUInt16(response, offset); offset += 5;

            return data;
        }

        public static TPGL_1 ParseTPGL_1(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.ZJ119)
        {
            TPGL_1 data = new TPGL_1();
            int offset = 56;//开始读取数据的地址是12 
            data.Temperature5 = BitConverter.ToUInt16(response, offset); offset += 5;

            return data;
        }

        public static List<(string, object)> ParseQUAL(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.ZJ119)
        {
            var data = new List<(string, object)>();
            int offset = 12;//开始读取数据的地址是12 
            int num = 0;
            string prefix = nameof(ParseQUAL).Replace("Parse","");
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3,'0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            return data;
        }

        public static List<(string, object)> ParseQUAC(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.ZJ119)
        {
            var data = new List<(string, object)>();
            int offset = 16;//开始读取数据的地址是12 
            int num = 0;
            string prefix = nameof(ParseQUAC).Replace("Parse", "");
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;

            return data;
        }

        public static List<(string, object)> Parsemsst(byte[] response,string name, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.ZJ119)
        {
            var data = new List<(string, object)>();
            int offset = 14;//开始读取数据的地址是12 
            int num = 0;
            string prefix = name;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToUInt16(response, offset))); offset += 2;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToUInt16(response, offset))); offset += 2;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            data.Add(($"{prefix}.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 4;
            return data;
        }

        internal static BPTR ParseBPTR(byte[] response, ProtocolTypeEnum huaniProtocolTypeEnum)
        {
            BPTR data = new BPTR();
            int offset = 12;
            data.Temperature5 = BitConverter.ToUInt16(response, offset); offset += 5;

            return data;
        }

        internal static ZIGH ParseZIGH(byte[] response, ProtocolTypeEnum huaniProtocolTypeEnum)
        {
            ZIGH data = new ZIGH();
            int offset = 12;
            data.Temperature4 = BitConverter.ToUInt16(response, offset); offset += 5;

            return data;
        }

        internal static ITOS ParseITOS(byte[] response, ProtocolTypeEnum huaniProtocolTypeEnum)
        {
            ITOS data = new ITOS();
            int offset = 184;
            data.S60V = BitConverter.ToSingle(response, offset); offset += 4;
            data.S63V = BitConverter.ToInt16(response, offset); offset += 2;
            data.S61V = BitConverter.ToInt16(response, offset); offset += 2;
            data.S62V = BitConverter.ToInt16(response, offset); offset += 2;
            data.A571V = BitConverter.ToSingle(response, offset); offset += 14;
            data.S71M = BitConverter.ToSingle(response, offset); offset += 4;
            data.S70M = BitConverter.ToInt16(response, offset); offset += 2;
            data.S7XX = BitConverter.ToInt16(response, offset); offset += 2;
            data.S77M = BitConverter.ToSingle(response, offset); offset += 4;
            return data;
        }

        public static List<(string, object)> ParseSMN4(byte[] response, string name, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.ZJ119)
        {
            var data = new List<(string, object)>();
            int offset = 12;//开始读取数据的地址是12 
            int dj = 0;
            string prefix = "SMN4";
            for(int i = 0;i < response.Length - offset - 56; i = i + 80)
            {
                int num = 1;
                dj++;
                offset = 12 + 24 + i;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
                data.Add(($"{prefix}.{dj.ToString()}.{num++.ToString()}", BitConverter.ToSingle(response, offset))); offset += 4;
            }
            return data;
        }

        //public static List<(string, object)> ParseQUAL(byte[] response, HuaniProtocolTypeEnum typeEnum = HuaniProtocolTypeEnum.ZJ119)
        //{
        //    var data = new List<(string, object)>();
        //    int offset = 12;//开始读取数据的地址是12 
        //    int num = 0;
        //    data.Add(($"QUAL.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
        //    data.Add(($"QUAL.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
        //    data.Add(($"QUAL.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
        //    data.Add(($"QUAL.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;
        //    data.Add(($"QUAL.{num++.ToString().PadLeft(3, '0')}", BitConverter.ToSingle(response, offset))); offset += 8;

        //    return data;
        //}
    }
}
