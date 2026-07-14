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
    /// node 1,func:2--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("VE 基本设备信息")]
    public struct VE_GNRL
    {

        [Description("气闸料位检测")]
        public bool B8V;         // 气闸料位检测

        [Description("气闸板位置检测")]
        public bool B9V;         // 气闸板位置检测

        [Description("烟丝料位检测")]
        public bool B6V;         // 烟丝料位检测

        [Description("烟丝料位检测")]
        public bool B5V;         // 烟丝料位检测

        [Description("气闸板打开/关闭")]
        public bool Y37V;        // 气闸板打开/关闭

        [Description("气闸板连锁")]
        public bool Y41V;        // 气闸板连锁

        [Description("供料气闸截止阀")]
        public bool Y57V;        // 供料气闸截止阀

        [Description("输送空气速度实际值")]
        public int A14V;         // 输送空气速度实际值

        [Description("计量辊电机接通")]
        public bool M10V;        // 计量辊电机接通

        [Description("烟丝料位检测")]
        public bool B7V;         // 烟丝料位检测

        [Description("风室降低")]
        public bool A130V_Y4;    // 风室降低

        [Description("风室抬高")]
        public bool A130V_Y5;    // 风室抬高

        [Description("风室位置下")]
        public bool S16V;        // 风室位置下

        [Description("风室位置上")]
        public bool S15V;        // 风室位置上

        [Description("流化床升降")]
        public bool A130V_Y9;    // 流化床升降

        [Description("流化床位置上")]
        public bool S13V;        // 流化床位置上

        [Description("流化床位置下")]
        public bool S14V;        // 流化床位置下

        [Description("备用")]
        public int A15V;         // 备用

        [Description("备用")]
        public ushort M2V_tobacco_cluster;  // 备用

        [Description("前道吸丝带张紧")]
        public bool S17V;        // 前道吸丝带张紧

        [Description("后道吸丝带张紧")]
        public bool S20V;        // 后道吸丝带张紧

        [Description("回丝量显示")]
        public int overfeed_display;  // 回丝量显示

        [Description("提丝带电机接通")]
        public bool M205V;       // 提丝带电机接通

        [Description("针辊电机接通")]
        public bool M206V;       // 针辊电机接通

        [Description("前防护左门")]
        public bool S7V;         // 前防护左门

        [Description("前防护中门")]
        public bool S8V;         // 前防护中门

        [Description("前防护右门")]
        public bool S9V;         // 前防护右门

        [Description("磁选盖板关闭")]
        public bool S10V;        // 磁选盖板关闭

        [Description("劈刀防护罩")]
        public bool S29V;        // 劈刀防护罩

        [Description("风室护罩")]
        public bool S31V;        // 风室护罩

        [Description("弹丝辘护罩")]
        public bool S37V;        // 弹丝辘护罩

        [Description("控制柜位置检测")]
        public bool S23V;        // 控制柜位置检测

        [Description("烟丝箱单元位置")]
        public bool S32V;        // 烟丝箱单元位置

        [Description("后落丝窗")]
        public bool S33V;        // 后落丝窗

        [Description("气闸板护罩")]
        public bool S40V;        // 气闸板护罩

        [Description("后护罩左门")]
        public bool S41V;        // 后护罩左门

        [Description("后护罩上门")]
        public bool S42V;        // 后护罩上门

        [Description("后护罩右门")]
        public bool S43V;        // 后护罩右门

        [Description("后护罩下门")]
        public bool S44V;        // 后护罩下门

        [Description("切换挡板位置下")]
        public bool S48V;        // 切换挡板位置下

        [Description("切换挡板位置上")]
        public bool S49V;        // 切换挡板位置上

        [Description("切换挡板")]
        public bool Y217V;       // 切换挡板

        [Description("旋转式滤网压力空气供应")]
        public bool Y218V;       // 旋转式滤网压力空气供应

        [Description("转动筛子电机接通/静止")]
        public bool M36V;        // 转动筛子电机接通/静止

        [Description("前道回送烟丝流量监测")]
        public ushort A18V;     // 前道回送烟丝流量监测

        [Description("后道回送烟丝流量监测")]
        public ushort A19V;     // 后道回送烟丝流量监测

        [Description("负压风机转速")]
        public int M1V;          // 负压风机转速

        [Description("循环风机转速")]
        public int M2V;          // 循环风机转速

        [Description("空气分配箱气压")]
        public int A54V;         // 空气分配箱气压

        [Description("一次分选气压")]
        public int A55V;         // 一次分选气压

        [Description("风室负压")]
        public int A56V;         // 风室负压

        [Description("流化床上部负压")]
        public float A57V;       // 流化床上部负压

        [Description("风室负压调节阀")]
        public ushort Y58V;      // 风室负压调节阀

        [Description("供料按钮")]
        public bool Tobacco_feed_button;  // 供料按钮

        [Description("烟丝输入按钮")]
        public bool Needleroller_button;  // 烟丝输入按钮

        [Description("VE防护罩")]
        public bool S2V;         // VE防护罩

        [Description("风机室温度检测")]
        public ushort S85V;      // 风机室温度检测

        [Description("叶轮闸门电机接通/静止")]
        public bool M35V;        // 叶轮闸门电机接通/静止

        [Description("计量料槽光幕（左）")]
        public ushort A26V;      // 计量料槽光幕（左）

        [Description("计量料槽光幕（中）")]
        public ushort A27V;      // 计量料槽光幕（中）

        [Description("计量料槽光幕（右）")]
        public ushort A28V;      // 计量料槽光幕（右）

        [Description("后劈刀前烟丝高度")]
        public int BefTrimRear1_A10V_Value;  // 后劈刀前烟丝高度

        [Description("后劈刀后烟丝高度")]
        public int AftTrimRear1_A11V_Value;  // 后劈刀后烟丝高度

        [Description("后劈刀前烟丝高度超限标志")]
        public bool BefTrimRear1_A10V_Flag;  // 后劈刀前烟丝高度超限标志

        [Description("后劈刀前烟丝高度超限标志")]
        public bool AftTrimRear1_A11V_Flag;  // 后劈刀前烟丝高度超限标志

        [Description("吸丝带张紧压力")]
        public float A58V;       // 吸丝带张紧压力

        [Description("烟丝实际消耗量")]
        public int tobacco_used;  // 烟丝实际消耗量
    }
}
