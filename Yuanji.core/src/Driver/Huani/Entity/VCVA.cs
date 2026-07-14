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
    [Description("MAX空气系统信息")]
    public struct VCVA
    {
        [Description("靠拢鼓轮清洁吹风")]
        public bool A150M_Y36;         // 靠拢鼓轮清洁吹风

        [Description("B18M导纸板支架罩壳清洁")]
        public bool A151M_Y116;         // B18M导纸板支架罩壳清洁

        [Description("减速轮清洁吹风")]
        public bool A150M_Y52;         // 减速轮清洁吹风

        [Description("传烟轮清洁吹风")]
        public bool A150M_Y88;         // 传烟轮清洁吹风

        [Description("二次分切轮清洁吹风")]
        public bool A150M_Y58;         // 二次分切轮清洁吹风

        [Description("B6M清洁吹气二次分切轮刀缝清洁吹风")]
        public bool A151M_Y136;         // B6M清洁吹气二次分切轮刀缝清洁吹风

        [Description("烟支分离轮清洁吹风")]
        public bool A150M_Y62;         // 烟支分离轮清洁吹风

        [Description("B2M/B3M/B4M烟支滤嘴吹除")]
        public bool A151M_Y114;         // B2M/B3M/B4M烟支滤嘴吹除

        [Description("检测轮清洁吹风")]
        public bool A150M_Y68;         // 检测轮清洁吹风

        [Description("低位出烟轮（高位取样轮）清洁吹风")]
        public bool A150M_Y86;         // 低位出烟轮（高位取样轮）清洁吹风

        [Description("调头轮清洁吹风")]
        public bool A150M_Y74;         // 调头轮清洁吹风

        [Description("卸出轮清洁吹风")]
        public bool A150M_Y78;         // 卸出轮清洁吹风

        [Description("激光轮清洁吹风")]
        public bool A150M_Y56;         // 激光轮清洁吹风

        [Description("负压控制系统4")]
        public bool A152M_Y156;         // 负压控制系统4

        [Description("负压控制系统5")]
        public bool A152M_Y158;         // 负压控制系统5

        [Description("负压控制系统1")]
        public bool A152M_Y150;         // 负压控制系统1

        [Description("负压控制系统2")]
        public bool A152M_Y152;         // 负压控制系统2

        [Description("负压控制系统3")]
        public bool A152M_Y154;         // 负压控制系统3

        [Description("一次分切轮清洁吹风")]
        public bool A150M_Y24;         // 一次分切轮清洁吹风

        [Description("烟条分离轮清洁吹风")]
        public bool A150M_Y26;         // 烟条分离轮清洁吹风

        [Description("滤嘴切割轮清洁吹风")]
        public bool A150M_Y38;         // 滤嘴切割轮清洁吹风

        [Description("错位轮清洁吹风")]
        public bool A150M_Y40;         // 错位轮清洁吹风

        [Description("汇合鼓轮清洁吹风")]
        public bool A150M_Y34;         // 汇合鼓轮清洁吹风

        [Description("给定负压风机速度值")]
        public ushort M8M_fan_rated_speed;         // 给定负压风机速度值

        [Description("负压风机实际速度反馈值")]
        public ushort M8M_fan_actual_rated_speed;         // 负压风机实际速度反馈值

        [Description("机器负压实际反馈值")]
        public ushort M8M_vacuum_actual_value;         // 机器负压实际反馈值

        [Description("水松纸切割装置清洁风吹除水松纸")]
        public bool A151M_Y126;         // 水松纸切割装置清洁风吹除水松纸

        [Description("搓烟轮清洁吹风")]
        public bool A150M_Y48;         // 搓烟轮清洁吹风

        [Description("检测轮清洁吹风")]
        public bool A150M_Y66;         // 检测轮清洁吹风

        [Description("端部扫描轮清洁吹风")]
        public bool A150M_Y64;         // 端部扫描轮清洁吹风

        [Description("备用")]
        public bool Reserved_01;         // 备用

        [Description("一次分切轮负压开关")]
        public bool A152M_Y208;         // 一次分切轮负压开关

        [Description("备用")]
        public bool Reserved_02;         // 备用

        [Description("备用")]
        public bool Reserved_03;         // 备用

        [Description("备用")]
        public bool Reserved_04;         // 备用

        [Description("烟灰吸尘器一次分切轮烟灰吹除")]
        public bool A151M_Y106;         // 烟灰吸尘器一次分切轮烟灰吹除

        [Description("备用")]
        public bool A150M_Y76;         // 备用

        [Description("中间轮清洁吹风")]
        public bool A150M_Y80;         // 中间轮清洁吹风

        [Description("高位出烟轮（低位取样轮）清洁吹风")]
        public bool A150M_Y90;         // 高位出烟轮（低位取样轮）清洁吹风

        [Description("B1M清洁风汇合鼓轮烟灰吹除")]
        public bool A151M_Y104;         // B1M清洁风汇合鼓轮烟灰吹除

        [Description("加速轮清洁吹风")]
        public bool A150M_Y44;         // 加速轮清洁吹风

        [Description("合一轮清洁吹风")]
        public bool A150M_Y42;         // 合一轮清洁吹风

        [Description("搓板刷清洁")]
        public bool A152M_Y202;         // 搓板刷清洁

        [Description("MAX压缩空气进给")]
        public float S74M;         // MAX压缩空气进给

        [Description("MAX检测传感器进气压力")]
        public float S79M;         // MAX检测传感器进气压力
    }
}
