using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 3,func:1--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("气密性设定信息")]
    public struct AIR_KALI
    {
        [Description("设定数量属性")]
        public byte Soll_Anz_Attri;         // 设定数量属性

        [Description("设定数量")]
        public byte Soll_Anz;         // 设定数量

        [Description("实际数量属性")]
        public byte Akt_Anz_Attri;         // 实际数量属性

        [Description("实际数量")]
        public byte Akt_Anz;         // 实际数量

        [Description("校准状态")]
        public byte Dicht_Kal_Status;         // 校准状态

        [Description("前排气密性未校准属性")]
        public byte Dicht_unkorr_vo_Attri;         // 前排气密性未校准属性

        [Description("前排气密性未校准值")]
        public ushort Dicht_unkorr_vo;         // 前排气密性未校准值

        [Description("前排气密性校准因子")]
        public ushort Dicht_Korr_Faktor_vo;         // 前排气密性校准因子

        [Description("备用")]
        public byte Dummy1;         // 备用

        [Description("后排气密性未校准属性")]
        public byte Dicht_unkorr_hi_Attri;         // 后排气密性未校准属性

        [Description("后排气密性未校准值")]
        public ushort Dicht_unkorr_hi;         // 后排气密性未校准值

        [Description("后排气密性校准因子")]
        public ushort Dicht_Korr_Faktor_hi;         // 后排气密性校准因子

        [Description("备用")]
        public byte Dummy2;         // 备用

        [Description("数字菜单1属性")]
        public byte Menue_Zahl1_Attri;         // 数字菜单1属性

        [Description("数字菜单1")]
        public ushort Menue_Zahl1;         // 数字菜单1

        [Description("备用")]
        public byte Dummy3;         // 备用

        [Description("数字菜单2属性")]
        public byte Menue_Zahl2_Attri;         // 数字菜单2属性

        [Description("数字菜单2")]
        public ushort Menue_Zahl2;         // 数字菜单2

        [Description("备用")]
        public byte Dummy4;         // 备用

        [Description("数字菜单3属性")]
        public byte Menue_Zahl3_Attri;         // 数字菜单3属性

        [Description("数字菜单3")]
        public ushort Menue_Zahl3;         // 数字菜单3

        [Description("备用")]
        public byte Dummy5;         // 备用

        [Description("文字菜单1属性")]
        public byte Menue_Zeile1_Attri;         // 文字菜单1属性

        [Description("文字菜单1")]
        public ushort Menue_Zeile1;         // 文字菜单1

        [Description("备用")]
        public byte Dummy6;         // 备用

        [Description("文字菜单2属性")]
        public byte Menue_Zeile2_Attri;         // 文字菜单2属性

        [Description("文字菜单2")]
        public ushort Menue_Zeile2;         // 文字菜单2

        [Description("备用")]
        public byte Dummy7;         // 备用

        [Description("文字菜单3属性")]
        public byte Menue_Zeile3_Attri;         // 文字菜单3属性

        [Description("文字菜单3")]
        public ushort Menue_Zeile3;         // 文字菜单3

        [Description("备用")]
        public byte Dummy8;         // 备用

        [Description("消息菜单1属性")]
        public byte Meld_Zeile1_Attri;         // 消息菜单1属性

        [Description("消息菜单1")]
        public ushort Meld_Zeile1;         // 消息菜单1

        [Description("备用")]
        public byte Dummy9;         // 备用

        [Description("消息菜单2属性")]
        public byte Meld_Zeile2_Attri;         // 消息菜单2属性

        [Description("消息菜单2")]
        public ushort Meld_Zeile2;         // 消息菜单2
    }
}
