using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Log;

namespace  Yuanji.core.src.Driver.Huani.Entity
{

    public class StopReasonInfo
    {
        [Description("停机原因")]
        public string StopReason { get; set; } // 停机原因

        [Description("停机时间")]
        public string StopTime { get; set; } // 停机时间

        [Description("停机次数")]
        public int StopCount { get; set; } // 停机次数
    }

    [Serializable]
    public class ZJ17ProductionData
    {
        public void PrintFieldDescriptionsAndValues()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var property in this.GetType().GetProperties())
            {
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute));
                if (descriptionAttribute != null)
                {
                    var value = property.GetValue(this, null) ?? "null";
                    stringBuilder.AppendLine($"{descriptionAttribute.Description}: {value}");
                }
            }
            //Log4net.Manager.LogHelper.ErrorLogger.Info(stringBuilder.ToString());
        }

        [Description("模式")]
        public string Mode { get; set; } // 模式,共有3种

        [Description("当前的故障代码")]
        public string MsgCode { get; set; } // 当前的故障代码

        [Description("开机时间")]
        public string PSTime { get; set; } // 开机时间，格式为YYYY-MM-DD HH:NN

        [Description("当前的班次号")]
        public int CurrentShift { get; set; } // 当前的班次号

        [Description("当前的产量(/1000)")]
        public double Production { get; set; } // 当前的产量/1000

        [Description("当前消耗的嘴棒")]
        public int Filter { get; set; } // 当前消耗的嘴棒

        [Description("当前的SE盘纸")]
        public int SEPaper { get; set; } // 当前的SE盘纸

        [Description("当前的水松纸")]
        public int MaxPaper { get; set; } // 当前的水松纸

        [Description("废品总支数")]
        public int TotalWas { get; set; } // 废品总支数

        [Description("废品率")]
        public double WasteRate { get; set; } // 废品率

        [Description("利用率")]
        public double UsageRate { get; set; } // 利用率

        [Description("效率")]
        public double Efficent { get; set; } // 效率

        [Description("当前车速")]
        public double CurrentSpeed { get; set; } // 当前车速

        [Description("总运行时间HH:NN")]
        public string TotalRunTime { get; set; } // 总运行时间HH:NN

        [Description("班次起始时间HH:NN")]
        public string PTime { get; set; } // 班次起始时间HH:NN

        [Description("停机时间HH:NN")]
        public string HaltTime { get; set; } // 停机时间HH:NN

        [Description("停机次数")]
        public int HaltNum { get; set; } // 停机次数

        [Description("双长烟计数")]
        public int DoubleRodNum1 { get; set; } // 双长烟计数

        [Description("盘纸长度")]
        public int SEPaperLen1 { get; set; } // 盘纸长度

        [Description("水松纸长度")]
        public int MaxPaperLen1 { get; set; } // 水松纸长度

        [Description("当前重量偏差")]
        public double CurWDeviation { get; set; } // 当前重量偏差

        [Description("当前重量偏移")]
        public double CurWOffset { get; set; } // 当前重量偏移

        [Description("当前短期标准偏差")]
        public double CurShortSD { get; set; } // 当前短期标准偏差

        [Description("当前长期标准偏差")]
        public double CurLongSD { get; set; } // 当前长期标准偏差

        [Description("当前压实端位置")]
        public double CurdensedPos { get; set; } // 当前压实端位置

        [Description("当前压实量")]
        public double CurDensed { get; set; } // 当前压实量

        [Description("当前平整盘位置")]
        public double CurTrimmerPos { get; set; } // 当前平整盘位置

        [Description("平均重量偏差")]
        public double AvgWDeviation { get; set; } // 平均重量偏差

        [Description("平均重量偏移")]
        public double AvgWOffset { get; set; } // 平均重量偏移

        [Description("平均短期标准偏差")]
        public double AvgShortSD { get; set; } // 平均短期标准偏差

        [Description("平均长期标准偏差")]
        public double AvgLongSD { get; set; } // 平均长期标准偏差

        [Description("平均压实端位置")]
        public double AvgdensedPos { get; set; } // 平均压实端位置

        [Description("平均压实量")]
        public double AvgDensed { get; set; } // 平均压实量

        [Description("平均平整盘位置")]
        public double AvgTrimmerPos { get; set; } // 平均平整盘位置

        // 质量控制
        [Description("当前过轻")]
        public double CurTooLight { get; set; } // 当前过轻

        [Description("当前过重")]
        public double CurTooHeavy { get; set; } // 当前过重

        [Description("当前软点")]
        public double CurSoftSpot { get; set; } // 当前软点

        [Description("当前硬点")]
        public double CurHardSpot { get; set; } // 当前硬点

        [Description("当前轻烟端")]
        public double CurLightEnd { get; set; } // 当前轻烟端

        [Description("当前OTIS")]
        public double CurOtis { get; set; } // 当前OTIS

        [Description("当前水松纸缺陷")]
        public double CurWSTFTR { get; set; } // 当前水松纸缺陷

        [Description("当前异物")]
        public double CurNTRM { get; set; } // 当前异物

        [Description("当前SRM取样")]
        public double CurSRMSampling { get; set; } // 当前SRM取样

        [Description("平均过轻")]
        public double AvgTooLight { get; set; }

        [Description("平均过重")]
        public double AvgTooHeavy { get; set; }

        [Description("平均软点")]
        public double AvgSoftSpot { get; set; }

        [Description("平均硬点")]
        public double AvgHardSpot { get; set; }

        [Description("平均轻烟端")]
        public double AvgLightEnd { get; set; }

        [Description("平均OTIS")]
        public double AvgOtis { get; set; }

        [Description("平均水松纸缺陷")]
        public double AvgWSTFTR { get; set; }

        [Description("平均异物")]
        public double AvgNTRM { get; set; }

        [Description("平均SRM取样")]
        public double AvgSRMSampling { get; set; }

        [Description("当前漏气")]
        public double CurAirleakage { get; set; }

        [Description("当前松头")]
        public double CurLooseEnd { get; set; }

        [Description("当前缺嘴")]
        public double CurMissFilter { get; set; }

        [Description("当前手动剔除")]
        public double CurManuel { get; set; }

        [Description("平均漏气")]
        public double AvgAirleakage { get; set; }

        [Description("平均松头")]
        public double AvgLooseEnd { get; set; }

        [Description("平均缺嘴")]
        public double AvgMissFilter { get; set; }

        [Description("平均手动剔除")]
        public double AvgManuel { get; set; }


        // 计数
        [Description("过轻支数")]
        public int TooLight { get; set; } // 过轻支数

        [Description("过重支数")]
        public int TooHeavy { get; set; } // 过重支数

        [Description("软点支数")]
        public int SoftSpot { get; set; } // 软点支数

        [Description("硬点支数")]
        public int HardSpot { get; set; } // 硬点支数

        [Description("轻烟端支数")]
        public int LightEnd { get; set; } // 轻烟端支数

        [Description("SRM取样支数")]
        public int SRMSampling { get; set; } // SRM取样支数

        [Description("漏气支数")]
        public int Airleakage { get; set; } // 漏气支数

        [Description("松头支数")]
        public int LooseEnd { get; set; } // 松头支数

        [Description("缺嘴支数")]
        public int MissFilter { get; set; } // 缺嘴支数

        [Description("手动剔除支数")]
        public int Manuel { get; set; } // 手动剔除支数

        [Description("OTIS剔除数")]
        public int OTIS { get; set; } // OTIS剔除数

        // 停机信息
        public StopReasonInfo[] StopReasonInfos { get; set; }


        // 运行时间
        [Description("-7时的运行时间")]
        public TimeSpan RUNTIME1 { get; set; } // -7时的运行时间

        [Description("-6时的运行时间")]
        public TimeSpan RUNTIME2 { get; set; } // -6时的运行时间

        [Description("-5时的运行时间")]
        public TimeSpan RUNTIME3 { get; set; } // -5时的运行时间

        [Description("-4时的运行时间")]
        public TimeSpan RUNTIME4 { get; set; } // -4时的运行时间

        [Description("-3时的运行时间")]
        public TimeSpan RUNTIME5 { get; set; } // -3时的运行时间

        [Description("-2时的运行时间")]
        public TimeSpan RUNTIME6 { get; set; } // -2时的运行时间

        [Description("-1时的运行时间")]
        public TimeSpan RUNTIME7 { get; set; } // -1时的运行时间

        [Description("现在的运行时间")]
        public TimeSpan RUNTIME8 { get; set; } // 现在的运行时间

        [Description("现在的停车时间")]
        public TimeSpan STOPTIME { get; set; } // 现在的停车时间

        // 标准偏差和废品
        // 标准偏差
        [Description("标准偏差1")]
        public double SD1 { get; set; }

        [Description("标准偏差2")]
        public double SD2 { get; set; }

        [Description("标准偏差3")]
        public double SD3 { get; set; }

        [Description("标准偏差4")]
        public double SD4 { get; set; }

        [Description("标准偏差5")]
        public double SD5 { get; set; }

        [Description("标准偏差6")]
        public double SD6 { get; set; }

        [Description("标准偏差7")]
        public double SD7 { get; set; }

        [Description("标准偏差8")]
        public double SD8 { get; set; }

        [Description("废品1")]
        public double WAS1 { get; set; }

        [Description("废品2")]
        public double WAS2 { get; set; }

        [Description("废品3")]
        public double WAS3 { get; set; }

        [Description("废品4")]
        public double WAS4 { get; set; }

        [Description("废品5")]
        public double WAS5 { get; set; }

        [Description("废品6")]
        public double WAS6 { get; set; }

        [Description("废品7")]
        public double WAS7 { get; set; }

        [Description("废品8")]
        public double WAS8 { get; set; }

        [Description("烟支总数")]
        public int CIG { get; set; }

        // 烟条监视器相关参数
        [Description("烟条监视器")]
        public string Pa1 { get; set; }

        [Description("快门")]
        public string Pa2 { get; set; }

        [Description("指示脉冲")]
        public string Pa3 { get; set; }

        [Description("平整盘电机")]
        public string Pa4 { get; set; }

        [Description("平整盘位置")]
        public double Pa5 { get; set; }

        [Description("机器速度")]
        public int Pa6 { get; set; }

        [Description("扫描器值")]
        public int Pa7 { get; set; }

        [Description("扫描器基准值")]
        public int Pa8 { get; set; }

        // 复位及扫描器相关参数
        [Description("复位计数")]
        public int R131Para1 { get; set; }

        [Description("主循环计数")]
        public int R131Para2 { get; set; }

        [Description("扫描器计数")]
        public int R131Para3 { get; set; }

        [Description("INDEX计数")]
        public int R131Para4 { get; set; }

        [Description("INCR计数")]
        public int R131Para5 { get; set; }

        [Description("25MS")]
        public int R131Para6 { get; set; }

        [Description("SIO计数")]
        public int R131Para7 { get; set; }

        [Description("HSO计数")]
        public int R131Para8 { get; set; }

        [Description("增量累积错")]
        public int R131Para9 { get; set; }

        [Description("扫描超限")]
        public int R131Para10 { get; set; }

        [Description("参考超限")]
        public int R131Para11 { get; set; }

        [Description("轴编码错")]
        public int R131Para12 { get; set; }

        //[Description("测试一")]
        //public int R131Para13 { get; set; }

        //[Description("测试二")]
        //public int R131Para14 { get; set; }

        //[Description("测试三")]
        //public int R131Para15 { get; set; }

        //[Description("测试四")]
        //public int R131Para16 { get; set; }

        // 废品及重量控制相关参数
        [Description("废品功能")]
        public bool R121Para1 { get; set; }

        //[Description("目标重量偏移功能")]
        //public bool R121Para2 { get; set; }

        //[Description("扫描器选择")]
        //public int R121Para3 { get; set; }

        [Description("重量控制")]
        public bool R121Para4 { get; set; }

        //[Description("偏差超限停车")]
        //public bool R121Para5 { get; set; }

        //[Description("微波选择")]
        //public int R121Para6 { get; set; }

        // 其他参数
        [Description("牌号")]
        public string Brand { get; set; }

        [Description("目标重量")]
        public double TWgt { get; set; }

        [Description("重量调整")]
        public double WgtAdj { get; set; }

        [Description("校准调整")]
        public double StdAdj { get; set; }

        [Description("内部标重")]
        public double ITWgt { get; set; }

        [Description("校准斜率")]
        public double StdRate { get; set; }

        [Description("废品重量低限")]
        public double LLmt { get; set; }

        [Description("废品重量高限")]
        public double Hlmt { get; set; }

        [Description("最大重量偏移")]
        public double MaxTD { get; set; }

        [Description("质量极限")]
        public double MassLmt { get; set; }

        [Description("质量范围")]
        public double MassRge { get; set; }

        [Description("最大废品率")]
        public double MaxWaste { get; set; }

        [Description("软点限度")]
        public double SDLmt { get; set; }

        [Description("硬点限度")]
        public double HDLmt { get; set; }

        [Description("轻烟端限度")]
        public double LPosLmt { get; set; }

        [Description("启动位置")]
        public double StPos { get; set; }

        [Description("切口距离")]
        public double CutDts { get; set; }

        [Description("调整增量")]
        public double AdjInc { get; set; }

        [Description("废品门烟条")]
        public int WasCig { get; set; }

        [Description("废品门残余增量")]
        public double WasLftInc { get; set; }

        [Description("取样门烟条")]
        public int SmpCig { get; set; }

        [Description("取样门参与增量")]
        public double SmpLftInc { get; set; }

        [Description("烟支长度")]
        public double CigLen { get; set; }

        // 废品门及重量控制详细参数
        [Description("废品门运行时间")]
        public double R123Para1 { get; set; }

        [Description("取样增量时间")]
        public double R123Para2 { get; set; }

        [Description("控制阀值")]
        public double R123Para3 { get; set; }

        [Description("标重偏移烟条数")]
        public int R123Para4 { get; set; }

        [Description("最大偏移步幅")]
        public double R123Para5 { get; set; }

        [Description("标重偏移方式")]
        public int R123Para6 { get; set; }

        [Description("平整盘滞后")]
        public double R123Para7 { get; set; }

        [Description("扫描器噪声补偿")]
        public double R123Para8 { get; set; }

        [Description("重量偏差重限")]
        public double R123Para9 { get; set; }

        [Description("重量偏差轻限")]
        public double R123Para10 { get; set; }

        [Description("缓冲区改变方式")]
        public int R123Para11 { get; set; }

        [Description("轨道选择")]
        public int R123Para12 { get; set; }

        [Description("取样目标重量")]
        public double R123Para13 { get; set; }

        [Description("循环间隔")]
        public double R123Para14 { get; set; }

        [Description("校准重量偏差")]
        public double R123Para15 { get; set; }

        [Description("引导性装入")]
        public int R123Para16 { get; set; }

        [Description("目标速度")]
        public int R123Para17 { get; set; }

        // R124 Parameters
        [Description("参考间隔")]
        public double R124Para1 { get; set; } // 参考间隔

        [Description("位移反馈补偿")]
        public double R124Para2 { get; set; } // 位移反馈补偿

        [Description("滤波常数")]
        public double R124Para3 { get; set; } // 滤波常数

        [Description("控制周期")]
        public double R124Para4 { get; set; } // 控制周期

        [Description("积分加速限")]
        public double R124Para5 { get; set; } // 积分加速限

        [Description("加速系数")]
        public double R124Para6 { get; set; } // 加速系数

        [Description("积分系数")]
        public double R124Para7 { get; set; } // 积分系数

        [Description("最大扫描脉冲")]
        public int R124Para8 { get; set; } // 最大扫描脉冲

        [Description("最小扫描脉冲")]
        public int R124Para9 { get; set; } // 最小扫描脉冲

        [Description("备用R124Para10")]
        public int R124Para10 { get; set; } // 备用

        [Description("可变带宽")]
        public double R124Para11 { get; set; } // 可变带宽

        [Description("内部控制指令")]
        public int R124Para12 { get; set; } // 内部控制指令

        // Sampling Parameters
        [Description("取样方式(本次)")]
        public int SM1 { get; set; } // 取样方式(本次)

        [Description("取样方式(上次)")]
        public int SM2 { get; set; } // 取样方式(上次)

        [Description("取样方式(上上次)")]
        public int SM3 { get; set; } // 取样方式(上上次)

        [Description("轨道选择ts1")]
        public int TS1 { get; set; } // 轨道选择

        [Description("轨道选择ts2")]
        public int TS2 { get; set; }

        [Description("轨道选择ts3")]
        public int TS3 { get; set; }

        [Description("取样目标重量STW1")]
        public double STW1 { get; set; } // 取样目标重量

        [Description("取样目标重量STW2")]
        public double STW2 { get; set; }

        [Description("取样目标重量STW3")]
        public double STW3 { get; set; }

        [Description("取样数量TN1")]
        public int TN1 { get; set; } // 取样数量

        [Description("取样数量TN2")]
        public int TN2 { get; set; }

        [Description("取样数量TN3")]
        public int TN3 { get; set; }

        [Description("已取烟支CE1")]
        public int CE1 { get; set; } // 已取烟支

        [Description("已取烟支CE2")]
        public int CE2 { get; set; }

        [Description("已取烟支CE3")]
        public int CE3 { get; set; }

        [Description("平均重量AW1")]
        public double AW1 { get; set; } // 平均重量

        [Description("平均重量AW2")]
        public double AW2 { get; set; }

        [Description("平均重量AW3")]
        public double AW3 { get; set; }

        [Description("标准偏差SamplingSD1")]
        public double SamplingSD1 { get; set; } // 标准偏差

        [Description("标准偏差SamplingSD2")]
        public double SamplingSD2 { get; set; }

        [Description("标准偏差SamplingSD3")]
        public double SamplingSD3 { get; set; }

        // Operation Parameters
        [Description("启动速度")]
        public double Operate_Para13 { get; set; }

        [Description("目标速度")]
        public double Operate_Para14 { get; set; }

        [Description("手动速度")]
        public double Operate_Para15 { get; set; }

        // SE Parameters
        [Description("进刀报警设置值")]
        public double SE_Para43 { get; set; }

        [Description("进刀实际值")]
        public double SE_Para44 { get; set; }

        [Description("胶枪开关,0关1开")]
        public int SE_Para25 { get; set; }

        [Description("胶枪温度")]
        public double SE_Para24 { get; set; }

        [Description("胶枪设定温度")]
        public double SE_Para23 { get; set; }

        [Description("烙铁1温度")]
        public double SE_Para27 { get; set; }

        [Description("烙铁1设定温度")]
        public double SE_Para26 { get; set; }

        [Description("烙铁2温度")]
        public double SE_Para29 { get; set; }

        [Description("烙铁2设定温度")]
        public double SE_Para28 { get; set; }

        [Description("搓板温度")]
        public double MAX_Para25 { get; set; }

        [Description("搓板设定温度")]
        public double MAX_Para24 { get; set; }

        [Description("水松纸温度")]
        public double MAX_Para27 { get; set; }

        [Description("水松纸设定温度")]
        public double MAX_Para26 { get; set; }

        // VE Parameters
        [Description("进料方式")]
        public int VE_Para1 { get; set; }

        [Description("关闭活门延时")]
        public double VE_Para2 { get; set; }

        [Description("针辊回丝量")]
        public double VE_Para3 { get; set; }

        [Description("停止针辊时机器速度")]
        public double VE_Para4 { get; set; }

        [Description("风室堵塞延时时间")]
        public double VE_Para5 { get; set; }

        [Description("小车装料时间")]
        public double VE_Para6 { get; set; }

        [Description("堆料槽料位调节积分常数")]
        public double VE_Para7 { get; set; }

        [Description("堆料槽料位空延时停机")]
        public double VE_Para8 { get; set; }

        // Additional SE Parameters
        [Description("SRM启动")]
        public int SE_Para8 { get; set; }

        [Description("程序停机")]
        public int SE_Para9 { get; set; }

        [Description("大小油墨选择（0大1小）")]
        public int SE_Para33 { get; set; }

        [Description("GD要求降速比例")]
        public double SE_Para19 { get; set; }

        [Description("涂胶投入")]
        public int SE_Para2 { get; set; }

        [Description("涂胶关断")]
        public int SE_Para5 { get; set; }

        [Description("降落铁")]
        public int SE_Para3 { get; set; }

        [Description("提升烙铁")]
        public int SE_Para6 { get; set; }

        [Description("打条")]
        public int SE_Para4 { get; set; }

        [Description("无烟条启动")]
        public int SE_Para7 { get; set; }

        [Description("烟条监视静态")]
        public int SE_Para10 { get; set; }

        [Description("烟条监视动态")]
        public int SE_Para11 { get; set; }

        [Description("回复正常速度（0否1是）")]
        public int SE_Para14 { get; set; }

        [Description("开始加速的TCP值")]
        public double SE_Para15 { get; set; }

        [Description("开始减速的TCP值")]
        public double SE_Para16 { get; set; }

        [Description("是否自动引纸（0否1是）")]
        public int SE_Para12 { get; set; }

        [Description("自动引纸延时时间")]
        public double SE_Para13 { get; set; }

        [Description("是否降速接纸（0否1是）")]
        public int SE_Para17 { get; set; }

        [Description("降速接纸速度")]
        public double SE_Para18 { get; set; }

        [Description("进刀间隔 (注意：SE_Para1 已被重新定义为进刀间隔，这里用 SE_Para1_Alt 代替另一个可能的含义)")]
        public double SE_Para1 { get; set; }

        [Description("纸接头检测（0动态1静态）")]
        public int SE_Para20 { get; set; }

        [Description("纸接头移位数")]
        public double SE_Para21 { get; set; }

        [Description("拼接段剔除数")]
        public int SE_Para22 { get; set; }

        [Description("钢印调整启动速度开关值")]
        public double SE_Para30 { get; set; }

        [Description("钢印调整最终速度开关值")]
        public double SE_Para31 { get; set; }

        [Description("钢印调整加速时间开关值")]
        public double SE_Para32 { get; set; }

        [Description("车速表调校")]
        public double SE_Para42 { get; set; }

        [Description("烟支长度")]
        public double SE_Para40 { get; set; }

        [Description("SE接纸时机微调")]
        public double SE_Para46 { get; set; }

        // MAX Parameters
        [Description("纸盘转换")]
        public int MAX_Para1 { get; set; }

        [Description("纸盘加速")]
        public int MAX_Para2 { get; set; }

        [Description("纸盘加速时间")]
        public double MAX_Para3 { get; set; }

        [Description("纸接头移位数")]
        public double MAX_Para4 { get; set; }

        [Description("刮纸器回位")]
        public int MAX_Para5 { get; set; }

        [Description("拼接段是否上胶（0是1否）")]
        public int MAX_Para6 { get; set; }

        [Description("拼接段到提升臂")]
        public double MAX_Para7 { get; set; }

        [Description("拼接段剔除数")]
        public int MAX_Para8 { get; set; }

        [Description("刮纸器接通")]
        public int MAX_Para9 { get; set; }

        [Description("OTIS接通")]
        public int MAX_Para10 { get; set; }

        [Description("胶水接通")]
        public int MAX_Para11 { get; set; }

        [Description("水松纸接通")]
        public int MAX_Para12 { get; set; }

        [Description("供嘴接通")]
        public int MAX_Para13 { get; set; }

        [Description("Y4剔除关")]
        public int MAX_Para14 { get; set; }

        [Description("刮纸器关断")]
        public int MAX_Para15 { get; set; }

        [Description("程序停机延时")]
        public int MAX_Para16 { get; set; }

        [Description("水松纸关断")]
        public int MAX_Para17 { get; set; }

        [Description("胶水关断")]
        public int MAX_Para18 { get; set; }

        [Description("供嘴关断")]
        public int MAX_Para19 { get; set; }

        [Description("Y4剔除接通")]
        public int MAX_Para20 { get; set; }

        [Description("清洁阀周期")]
        public int MAX_Para21 { get; set; }

        [Description("进刀时间间隔")]
        public int MAX_Para22 { get; set; }

        [Description("水松纸剔除装置（0无1有）")]
        public int MAX_Para23 { get; set; }

        [Description("是否旋转胶缸（0否1是）")]
        public int MAX_Para28_Spin { get; set; }

        [Description("激光打孔装置（0无1有)")]
        public int MAX_Para28_Laser { get; set; }

        // Cis22 相关参数
        [Description("松头绝对门坎")]
        public int Cis22Para1 { get; set; }

        [Description("松头门坎")]
        public int Cis22Para2 { get; set; }

        [Description("漏气绝对门坎")]
        public int Cis22Para3 { get; set; } // 漏气绝对门坎

        [Description("漏气门坎")]
        public int Cis22Para4 { get; set; } // 漏气门坎

        [Description("OTIS绝对门坎")]
        public int Cis22Para5 { get; set; } // OTIS绝对门坎

        [Description("OTIS门坎")]
        public int Cis22Para6 { get; set; } // OTIS门坎

        [Description("剔废滞后")]
        public int Cis22Para7 { get; set; } // 剔废滞后

        [Description("剔废阀维持")]
        public int Cis22Para8 { get; set; } // 剔废阀维持

        [Description("帽盖报警限")]
        public int Cis22Para9 { get; set; } // 帽盖报警限

        [Description("DCP3上升沿")]
        public int Cis22Para10 { get; set; } // DCP3上升沿

        [Description("DCP3下降沿")]
        public int Cis22Para11 { get; set; } // DCP3下降沿

        [Description("SCP1上升沿")]
        public int Cis22Para12 { get; set; } // SCP1上升沿

        [Description("SCP1下降沿")]
        public int Cis22Para13 { get; set; } // SCP1下降沿

        [Description("参数初始化")]
        public int Cis22Para14 { get; set; } // 参数初始化 (假设这是一个布尔值，根据上下文可能需要调整)

        // Cis24 相关参数
        [Description("SCP偏移")]
        public double Cis24Para1 { get; set; } // SCP偏移

        [Description("OTIS测点距离")]
        public double Cis24Para2 { get; set; } // OTIS测点距离

        [Description("中剔阀距离")]
        public double Cis24Para3 { get; set; } // 中剔阀距离

        [Description("松头测点距离")]
        public double Cis24Para4 { get; set; } // 松头测点距离

        [Description("漏气测点距离")]
        public double Cis24Para5 { get; set; } // 漏气测点距离

        [Description("同步槽号")]
        public int Cis24Para6 { get; set; } // 同步槽号

        [Description("漏气测点槽号")]
        public int Cis24Para7 { get; set; } // 漏气测点槽号

        [Description("控制字")]
        public int Cis24Para8 { get; set; } // 控制字 (假设这是一个整型，根据上下文可能需要调整)

        //// 生产统计相关参数
        //[Description("上一班产量")]
        //public int LastTotal { get; set; } // 上一班产量

        //[Description("上一班嘴棒")]
        //public int LastFilter { get; set; } // 上一班嘴棒

        //[Description("上一班盘纸米数")]
        //public int LastSePaperLen { get; set; } // 上一班盘纸米数

        //[Description("上一盘纸用量")]
        //public int LastSEUsage { get; set; } // 上一盘纸用量

        //[Description("当前盘用量")]
        //public int CurSEUsage { get; set; } // 当前盘用量
    }

    public class ZJ17DataConverter
    {
        public static ZJ17ProductionData ParseData(string byteConvert)
        {
            // 去除头部和尾部不必要的字符
            byteConvert = byteConvert.TrimStart((char)(0x01)).TrimEnd((char)(0x1A));

            double dd = 0;
            int tt = 0;
            // 分割字符串，按分号
            var sections = byteConvert.Split(';');

            var productionData = new ZJ17ProductionData();

            // 基本信息解析
            var basicInfo = sections[1].Split(',');
            try
            {
                productionData.Mode = basicInfo[0];
                productionData.MsgCode = basicInfo[1];
                productionData.PSTime = basicInfo[2];
                productionData.CurrentShift = int.TryParse(basicInfo[3], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 基本信息解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 生产数据解析
            var productionDataInfo = sections[2].Split(',');
            try
            {
                productionData.Production = double.TryParse(productionDataInfo[0], out dd) ? dd : 0;
                productionData.Filter = int.TryParse(productionDataInfo[1], out  tt) ? tt : 0;
                productionData.SEPaper = int.TryParse(productionDataInfo[2], out  tt) ? tt : 0;
                productionData.MaxPaper = int.TryParse(productionDataInfo[3], out  tt) ? tt : 0;
                productionData.TotalWas = int.TryParse(productionDataInfo[4], out  tt) ? tt : 0;
                productionData.WasteRate = double.TryParse(productionDataInfo[5], out dd) ? dd : 0;
                productionData.UsageRate = double.TryParse(productionDataInfo[6], out dd) ? dd : 0;
                productionData.Efficent = double.TryParse(productionDataInfo[7], out dd) ? dd : 0;   
                productionData.CurrentSpeed = double.TryParse(productionDataInfo[8], out dd) ? dd : 0;
                productionData.TotalRunTime = productionDataInfo[9];
                productionData.PTime = productionDataInfo[10];
                productionData.HaltTime = productionDataInfo[11];
                productionData.HaltNum = int.TryParse(productionDataInfo[12], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 生产数据解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 重量和偏差数据解析
            var weightAndDeviation = sections[3].Split(',');
            try
            {
                productionData.CurWDeviation = double.TryParse(weightAndDeviation[0], out double res) ? res : 0;
                productionData.CurWOffset = double.TryParse(weightAndDeviation[1], out dd) ? dd : 0;
                productionData.CurShortSD = double.TryParse(weightAndDeviation[2], out dd) ? dd : 0;
                productionData.CurLongSD = double.TryParse(weightAndDeviation[3], out dd) ? dd : 0;   
                productionData.CurdensedPos = double.TryParse(weightAndDeviation[4], out dd) ? dd : 0;
                productionData.CurDensed = double.TryParse(weightAndDeviation[5], out dd) ? dd : 0;
                productionData.CurTrimmerPos = double.TryParse(weightAndDeviation[6], out dd) ? dd : 0;
                productionData.AvgWDeviation = double.TryParse(weightAndDeviation[7], out dd) ? dd : 0;
                productionData.AvgWOffset = double.TryParse(weightAndDeviation[8], out dd) ? dd : 0;
                productionData.AvgShortSD = double.TryParse(weightAndDeviation[9], out dd) ? dd : 0;
                productionData.AvgLongSD = double.TryParse(weightAndDeviation[10], out dd) ? dd : 0;
                productionData.AvgdensedPos = double.TryParse(weightAndDeviation[11], out dd) ? dd : 0;
                productionData.AvgDensed = double.TryParse(weightAndDeviation[12], out dd) ? dd : 0;
                productionData.AvgTrimmerPos = double.TryParse(weightAndDeviation[13], out dd) ? dd : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 重量和偏差数据解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 质量控制数据
            var qualityControl = sections[4].Split(',');
            try
            {
                productionData.CurTooLight = double.TryParse(qualityControl[0], out dd) ? dd : 0;
                productionData.CurTooHeavy = double.TryParse(qualityControl[1], out dd) ? dd : 0;
                productionData.CurSoftSpot = double.TryParse(qualityControl[2], out dd) ? dd : 0;
                productionData.CurHardSpot = double.TryParse(qualityControl[3], out dd) ? dd : 0;
                productionData.CurLightEnd = double.TryParse(qualityControl[4], out dd) ? dd : 0;
                productionData.CurOtis = double.TryParse(qualityControl[5], out dd) ? dd : 0;
                productionData.CurWSTFTR = double.TryParse(qualityControl[6], out dd) ? dd: 0;
                productionData.CurNTRM = double.TryParse(qualityControl[7], out dd) ? dd : 0;
                productionData.CurSRMSampling = double.TryParse(qualityControl[8], out dd) ? dd : 0;

                productionData.AvgTooLight = double.TryParse(qualityControl[9], out dd) ? dd : 0;
                productionData.AvgTooHeavy = double.TryParse(qualityControl[10], out dd) ? dd : 0;
                productionData.AvgSoftSpot = double.TryParse(qualityControl[11], out dd) ? dd : 0;
                productionData.AvgHardSpot = double.TryParse(qualityControl[12], out dd) ? dd : 0;
                productionData.AvgLightEnd = double.TryParse(qualityControl[13], out dd) ? dd : 0;
                productionData.AvgOtis = double.TryParse(qualityControl[14], out dd) ? dd : 0;
                productionData.AvgWSTFTR = double.TryParse(qualityControl[15], out dd) ? dd : 0;
                productionData.AvgNTRM = double.TryParse(qualityControl[16], out dd) ? dd : 0;
                productionData.AvgSRMSampling = double.TryParse(qualityControl[17], out dd) ? dd : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 质量控制数据解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //继续质量控制数据
            var errorData = sections[5].Split(',');
            try
            {
                productionData.CurAirleakage = double.TryParse(errorData[0], out dd) ? dd : 0;
                //根据文档此处默认一个0--index1
                productionData.CurLooseEnd = double.TryParse(errorData[2], out dd) ? dd : 0;
                productionData.CurMissFilter = double.TryParse(errorData[3], out dd) ? dd: 0;
                productionData.CurManuel = double.TryParse(errorData[4], out dd) ? dd : 0;
                //根据文档此处默认一个0--index5
                //根据文档此处默认一个0--index6
                productionData.AvgAirleakage = double.TryParse(errorData[7], out dd) ? dd : 0;
                //根据文档此处默认一个0--index8
                productionData.AvgLooseEnd = double.TryParse(errorData[9], out dd) ? dd : 0;
                productionData.AvgMissFilter = double.TryParse(errorData[10], out dd) ? dd : 0;
                productionData.AvgManuel = double.TryParse(errorData[11], out dd) ? dd : 0;
                //根据文档此处默认一个0--index12
                //根据文档此处默认一个0--index13
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 质量控制数据解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 继续解析计数数据
            var countData = sections[6].Split(',');
            try
            {
                productionData.TooLight = int.TryParse(countData[0], out  tt) ? tt : 0;
                productionData.TooHeavy = int.TryParse(countData[1], out  tt) ? tt : 0;
                productionData.SoftSpot = int.TryParse(countData[2], out  tt) ? tt : 0;
                productionData.HardSpot = int.TryParse(countData[3], out  tt) ? tt : 0;
                productionData.LightEnd = int.TryParse(countData[4], out  tt) ? tt : 0;
                //根据文档此处默认一个0--index5
                //根据文档此处默认一个0--index6
                //根据文档此处默认一个0--index7
                productionData.SRMSampling = int.TryParse(countData[8], out  tt) ? tt : 0;

                productionData.Airleakage = int.TryParse(countData[9], out  tt) ? tt : 0;
                //根据文档此处默认一个0--index10
                productionData.LooseEnd = int.TryParse(countData[11], out  tt) ? tt : 0;
                productionData.MissFilter = int.TryParse(countData[12], out  tt) ? tt : 0;
                productionData.Manuel = int.TryParse(countData[13], out  tt) ? tt : 0;
                productionData.OTIS = int.TryParse(countData[14], out  tt) ? tt : 0;
                //根据文档此处默认一个0--index15
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 继续解析计数数据 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 继续解析停机信息
            var stopData = sections[7].Split(',');
            try
            {
                productionData.StopReasonInfos = new StopReasonInfo[10];
                for (int i = 0; i < 10; i++)
                {
                    if (!string.IsNullOrEmpty(stopData[0 + i * 3]) && !stopData[0 + i * 3].Equals("其他原因", StringComparison.OrdinalIgnoreCase))
                    {
                        productionData.StopReasonInfos[i] = new StopReasonInfo();
                        productionData.StopReasonInfos[i].StopReason = stopData[0 + i * 3];
                        if (!string.IsNullOrEmpty(stopData[1 + i * 3]))
                            productionData.StopReasonInfos[i].StopTime = stopData[1 + i * 3];
                        if (!string.IsNullOrEmpty(stopData[2 + i * 3]))
                            productionData.StopReasonInfos[i].StopCount = int.TryParse(stopData[2 + i * 3], out  tt) ? tt : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 停机信息解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 标准偏差和废品和总数
            var sdAndWaste = sections[8].Split(',');
            try
            {
                productionData.SD1 = double.TryParse(sdAndWaste[0], out dd) ? dd : 0;
                productionData.SD2 = double.TryParse(sdAndWaste[1], out dd) ? dd : 0;
                productionData.SD3 = double.TryParse(sdAndWaste[2], out dd) ? dd : 0;
                productionData.SD4 = double.TryParse(sdAndWaste[3], out dd) ? dd : 0;
                productionData.SD5 = double.TryParse(sdAndWaste[4], out dd) ? dd : 0;
                productionData.SD6 = double.TryParse(sdAndWaste[5], out dd) ? dd : 0;
                productionData.SD7 = double.TryParse(sdAndWaste[6], out dd) ? dd : 0;
                productionData.SD8 = double.TryParse(sdAndWaste[7], out dd) ? dd : 0;
                productionData.WAS1 = double.TryParse(sdAndWaste[8], out dd) ? dd : 0;
                productionData.WAS2 = double.TryParse(sdAndWaste[9], out dd) ? dd : 0;
                productionData.WAS3 = double.TryParse(sdAndWaste[10], out dd) ? dd : 0;
                productionData.WAS4 = double.TryParse(sdAndWaste[11], out dd) ? dd : 0;
                productionData.WAS5 = double.TryParse(sdAndWaste[12], out dd) ? dd : 0;
                productionData.WAS6 = double.TryParse(sdAndWaste[13], out dd) ? dd : 0;
                productionData.WAS7 = double.TryParse(sdAndWaste[14], out dd) ? dd : 0;
                productionData.WAS8 = double.TryParse(sdAndWaste[15], out dd) ? dd : 0;
                productionData.CIG = int.TryParse(sdAndWaste[16], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 标准偏差和废品和总数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 烟条指示器参数
            var PaData = sections[9].Split(',');
            try
            {
                productionData.Pa1 = PaData[0];
                productionData.Pa2 = PaData[1];
                productionData.Pa3 = PaData[2];
                productionData.Pa4 = PaData[3];

                productionData.Pa5 = double.TryParse(PaData[4], out dd) ? dd : 0;
                productionData.Pa6 = int.TryParse(PaData[5], out  tt) ? tt : 0;
                productionData.Pa7 = int.TryParse(PaData[6], out  tt) ? tt : 0;
                productionData.Pa8 = int.TryParse(PaData[7], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 烟条指示器参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }

            //R131参数
            var r131Data = sections[10].Split(',');
            try
            {
                productionData.R131Para1 = int.TryParse(r131Data[0], out  tt) ? tt : 0;
                productionData.R131Para2 = int.TryParse(r131Data[1], out  tt) ? tt : 0;
                productionData.R131Para3 = int.TryParse(r131Data[2], out  tt) ? tt : 0;
                productionData.R131Para4 = int.TryParse(r131Data[3], out  tt) ? tt : 0;
                productionData.R131Para5 = int.TryParse(r131Data[4], out  tt) ? tt : 0;
                productionData.R131Para6 = int.TryParse(r131Data[5], out  tt) ? tt : 0;
                productionData.R131Para7 = int.TryParse(r131Data[6], out  tt) ? tt : 0;
                productionData.R131Para8 = int.TryParse(r131Data[7], out  tt) ? tt : 0;
                productionData.R131Para9 = int.TryParse(r131Data[8], out  tt) ? tt : 0;
                productionData.R131Para10 = int.TryParse(r131Data[9], out  tt) ? tt : 0;
                productionData.R131Para11 = int.TryParse(r131Data[10], out  tt) ? tt : 0;
                productionData.R131Para12 = int.TryParse(r131Data[11], out  tt) ? tt : 0;
                //productionData.R131Para13 = int.TryParse(r131Data[12], out  tt) ? tt : 0;
                //productionData.R131Para14 = int.TryParse(r131Data[13], out  tt) ? tt : 0;
                //productionData.R131Para15 = int.TryParse(r131Data[14], out  tt) ? tt : 0;
                //productionData.R131Para16 = int.TryParse(r131Data[15], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData R131参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //打开,打开
            var r121Data = sections[11].Split(',');

            try
            {
                productionData.R121Para1 = r121Data[0] == "关闭" ? false:true;
                productionData.R121Para4 = r121Data[1] == "关闭" ? false : true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData R121参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");

            }

            //1,470,0,-66,893,0.60,-50,50,-15,-60,98.0,2.00,40.0,40.0,30.0,-4.0,0,4,263,0,86,26,58.0
            //Brand 参数
            var brandData = sections[12].Split(',');
            try
            {
                productionData.Brand = brandData[0];
                productionData.TWgt = double.TryParse(brandData[1], out dd) ? dd : 0;
                productionData.WgtAdj = double.TryParse(brandData[2], out dd) ? dd : 0;
                productionData.StdAdj = double.TryParse(brandData[3], out dd) ? dd : 0;
                productionData.ITWgt = double.TryParse(brandData[4], out dd) ? dd : 0;
                productionData.StdRate = double.TryParse(brandData[5], out dd) ? dd : 0;
                productionData.LLmt = double.TryParse(brandData[6], out dd) ? dd : 0;
                productionData.Hlmt = double.TryParse(brandData[7], out dd) ? dd : 0;
                productionData.MaxTD = double.TryParse(brandData[8], out dd) ? dd : 0;
                productionData.MassLmt = double.TryParse(brandData[9], out dd) ? dd : 0;

                productionData.MassRge = double.TryParse(brandData[10], out dd) ? dd: 0;
                productionData.MaxWaste = double.TryParse(brandData[11], out dd) ? dd : 0;
                productionData.SDLmt = double.TryParse(brandData[12], out dd) ? dd : 0;
                productionData.HDLmt = double.TryParse(brandData[13], out dd) ? dd : 0;
                productionData.LPosLmt = double.TryParse(brandData[14], out dd) ? dd : 0;
                productionData.StPos = double.TryParse(brandData[15], out dd) ? dd : 0;
                productionData.CutDts = double.TryParse(brandData[16], out dd) ? dd : 0;
                productionData.AdjInc = double.TryParse(brandData[17], out dd) ? dd : 0;
                productionData.WasCig = int.TryParse(brandData[18], out  tt) ? tt : 0;
                productionData.WasLftInc = double.TryParse(brandData[19], out dd) ? dd : 0;
                productionData.SmpCig = int.TryParse(brandData[20], out tt) ? tt : 0;
                productionData.SmpLftInc = double.TryParse(brandData[21], out dd) ? dd : 0;
                productionData.CigLen = double.TryParse(brandData[22], out double res) ? res : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData Brand参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }

            //R123参数
            var r123Data = sections[13].Split(',');
            try
            {
                productionData.R123Para1 = double.TryParse(r123Data[0], out dd) ? dd : 0;
                productionData.R123Para2 = double.TryParse(r123Data[1], out dd) ? dd : 0;
                productionData.R123Para3 = double.TryParse(r123Data[2], out dd) ? dd : 0;
                productionData.R123Para4 = int.TryParse(r123Data[3], out  tt) ? tt : 0;
                productionData.R123Para5 = double.TryParse(r123Data[4], out dd) ? dd : 0;
                productionData.R123Para6 = int.TryParse(r123Data[5], out  tt) ? tt : 0;
                productionData.R123Para7 = double.TryParse(r123Data[6], out dd) ? dd : 0;
                productionData.R123Para8 = double.TryParse(r123Data[7], out dd) ? dd : 0;
                productionData.R123Para9 = double.TryParse(r123Data[8], out dd) ? dd : 0;
                productionData.R123Para10 = double.TryParse(r123Data[9], out dd) ? dd : 0;
                productionData.R123Para11 = int.TryParse(r123Data[10], out  tt) ? tt : 0;
                productionData.R123Para12 = int.TryParse(r123Data[11], out  tt) ? tt : 0;
                productionData.R123Para13 = double.TryParse(r123Data[12], out dd) ? dd : 0;
                productionData.R123Para14 = double.TryParse(r123Data[13], out dd) ? dd : 0;
                productionData.R123Para15 = double.TryParse(r123Data[14], out dd) ? dd : 0;
                productionData.R123Para16 = int.TryParse(r123Data[15], out  tt) ? tt : 0;
                productionData.R123Para17 = int.TryParse(r123Data[16], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData R123参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //R124参数
            var r124Data = sections[14].Split(',');
            try
            {
                productionData.R124Para1 = double.TryParse(r124Data[0], out dd) ? dd : 0;
                productionData.R124Para2 = double.TryParse(r124Data[1], out dd) ? dd : 0;
                productionData.R124Para3 = double.TryParse(r124Data[2], out dd) ? dd : 0;
                productionData.R124Para4 = double.TryParse(r124Data[3], out dd) ? dd : 0;
                productionData.R124Para5 =  double.TryParse(r124Data[4], out dd) ? dd : 0;
                productionData.R124Para6 = double.TryParse(r124Data[5], out dd) ? dd : 0;
                productionData.R124Para7 = double.TryParse(r124Data[6], out dd) ? dd : 0;
                productionData.R124Para8 = int.TryParse(r124Data[7], out  tt) ? tt : 0;
                productionData.R124Para9 = int.TryParse(r124Data[8], out  tt) ? tt : 0;
                productionData.R124Para10 = int.TryParse(r124Data[9], out  tt) ? tt : 0;
                productionData.R124Para11 = double.TryParse(r124Data[10], out dd) ? dd : 0;
                productionData.R124Para12 = int.TryParse(r124Data[11], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData R124参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            // 取样参数 SM,TS,STW,TN,CE,AW,SD
            var anotherSmpData = sections[15].Split(',');
            try
            {
                productionData.SM1 = int.TryParse(anotherSmpData[0], out  tt) ? tt : 0;
                productionData.SM2 = int.TryParse(anotherSmpData[1], out  tt) ? tt : 0;
                productionData.SM3 = int.TryParse(anotherSmpData[2], out  tt) ? tt : 0;
                productionData.TS1 = int.TryParse(anotherSmpData[3], out  tt) ? tt : 0;
                productionData.TS2 = int.TryParse(anotherSmpData[4], out  tt) ? tt : 0;
                productionData.TS3 = int.TryParse(anotherSmpData[5], out  tt) ? tt : 0;
                productionData.STW1 = double.TryParse(anotherSmpData[6], out dd) ? dd : 0;
                productionData.STW2 =   double.TryParse(anotherSmpData[7], out dd) ? dd : 0;
                productionData.STW3 = double.TryParse(anotherSmpData[8], out dd) ? dd : 0;

                productionData.TN1 = int.TryParse(anotherSmpData[9], out  tt) ? tt : 0;
                productionData.TN2 = int.TryParse(anotherSmpData[10], out  tt) ? tt : 0;
                productionData.TN3 = int.TryParse(anotherSmpData[11], out  tt) ? tt : 0;

                productionData.CE1 = int.TryParse(anotherSmpData[12], out  tt) ? tt : 0;
                productionData.CE2 = int.TryParse(anotherSmpData[13], out  tt) ? tt : 0;
                productionData.CE3 = int.TryParse(anotherSmpData[14], out  tt) ? tt : 0;

                productionData.AW1 = double.TryParse(anotherSmpData[15], out dd) ? dd : 0;
                productionData.AW2 = double.TryParse(anotherSmpData[16], out dd) ? dd : 0;
                productionData.AW3 = double.TryParse(anotherSmpData[17], out dd) ? dd : 0;
                //productionData.SD1 = double.TryParse(anotherSmpData[18], out dd) ? dd : 0;
                //productionData.SD2 = double.TryParse(anotherSmpData[19], out dd) ? dd : 0;
                //productionData.SD3 = double.TryParse(anotherSmpData[20], out dd) ? dd : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 取样参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //速度温度参数 
            var speedAndTmpData = sections[16].Split(',');
            try
            {
                productionData.Operate_Para13 = double.TryParse(speedAndTmpData[0], out dd) ? dd : 0;
                productionData.Operate_Para14 = double.TryParse(speedAndTmpData[1], out dd) ? dd : 0;
                productionData.Operate_Para15 = double.TryParse(speedAndTmpData[2], out dd) ? dd : 0;

                productionData.SE_Para43 = double.TryParse(speedAndTmpData[3], out dd) ? dd : 0;
                productionData.SE_Para44 = double.TryParse(speedAndTmpData[4], out dd) ? dd : 0;
                productionData.SE_Para24 = double.TryParse(speedAndTmpData[5], out dd) ? dd : 0;
                productionData.SE_Para23 = double.TryParse(speedAndTmpData[6], out dd) ? dd : 0;
                productionData.SE_Para27 = double.TryParse(speedAndTmpData[7], out dd) ? dd : 0;
                productionData.SE_Para26 = double.TryParse(speedAndTmpData[8], out dd) ? dd : 0;
                productionData.SE_Para29 = double.TryParse(speedAndTmpData[9], out dd) ? dd : 0;
                productionData.SE_Para28 = double.TryParse(speedAndTmpData[10], out dd) ? dd : 0;

                productionData.MAX_Para25 = double.TryParse(speedAndTmpData[11], out dd) ? dd : 0;
                productionData.MAX_Para24 = double.TryParse(speedAndTmpData[12], out dd) ? dd : 0;
                productionData.MAX_Para27 = double.TryParse(speedAndTmpData[13], out dd) ? dd : 0;
                productionData.MAX_Para26 = double.TryParse(speedAndTmpData[14], out dd) ? dd : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 速度温度参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //进料参数
            var veData = sections[17].Split(',');
            try
            {
                productionData.VE_Para1 = int.TryParse(veData[0], out  tt) ? tt : 0;
                productionData.VE_Para2 = double.TryParse(veData[1], out dd) ? dd : 0;
                productionData.VE_Para3 = double.TryParse(veData[2], out dd) ? dd : 0;
                productionData.VE_Para4 = double.TryParse(veData[3], out dd) ? dd : 0;
                productionData.VE_Para5 = double.TryParse(veData[4], out dd) ? dd : 0;
                productionData.VE_Para6 = double.TryParse(veData[5], out dd) ? dd : 0;
                productionData.VE_Para7 = double.TryParse(veData[6], out dd) ? dd : 0;
                productionData.VE_Para8 = double.TryParse(veData[7], out dd) ? dd : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 进料参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //其他设备参数
            var seMachineData = sections[18].Split(',');
            try
            {
                productionData.SE_Para8 = int.TryParse(seMachineData[0], out  tt) ? tt : 0;
                productionData.SE_Para9 = int.TryParse(seMachineData[1], out  tt) ? tt : 0;
                productionData.SE_Para33 = int.TryParse(seMachineData[2], out  tt) ? tt : 0;
                productionData.SE_Para19 = int.TryParse(seMachineData[3], out  tt) ? tt : 0;
                productionData.SE_Para2 = int.TryParse(seMachineData[4], out  tt) ? tt : 0;
                productionData.SE_Para5 = int.TryParse(seMachineData[5], out  tt) ? tt : 0;
                productionData.SE_Para3 = int.TryParse(seMachineData[6], out  tt) ? tt : 0;
                productionData.SE_Para6 = int.TryParse(seMachineData[7], out  tt) ? tt : 0;

                productionData.SE_Para4 = int.TryParse(seMachineData[8], out  tt) ? tt : 0;
                productionData.SE_Para7 = int.TryParse(seMachineData[9], out  tt) ? tt : 0;
                productionData.SE_Para10 = int.TryParse(seMachineData[10], out  tt) ? tt : 0;
                productionData.SE_Para11 = int.TryParse(seMachineData[11], out  tt) ? tt : 0;
                productionData.SE_Para14 = int.TryParse(seMachineData[12], out  tt) ? tt : 0;
                productionData.SE_Para15 = int.TryParse(seMachineData[13], out  tt) ? tt : 0;
                productionData.SE_Para16 = int.TryParse(seMachineData[14], out  tt) ? tt : 0;
                productionData.SE_Para12 = int.TryParse(seMachineData[15], out  tt) ? tt == 256 ? 1 : 0 : 0;

                productionData.SE_Para13 = double.TryParse(seMachineData[16], out dd) ? dd : 0;
                productionData.SE_Para17 = int.TryParse(seMachineData[17], out  tt) ? tt==256? 1:0 : 0;
                productionData.SE_Para18 = double.TryParse(seMachineData[18], out dd) ? dd : 0;
                productionData.SE_Para1 = double.TryParse(seMachineData[19], out dd) ? dd : 0;
                productionData.SE_Para20 = int.TryParse(seMachineData[20], out  tt) ? tt : 0;
                productionData.SE_Para21 = double.TryParse(seMachineData[21], out dd) ? dd : 0;
                productionData.SE_Para22 = int.TryParse(seMachineData[22], out  tt) ? tt : 0;
                productionData.SE_Para30 = double.TryParse(seMachineData[23], out dd) ? dd : 0;

                productionData.SE_Para31 = double.TryParse(seMachineData[24], out dd) ? dd : 0;
                productionData.SE_Para32 = double.TryParse(seMachineData[25], out dd) ? dd : 0;
                productionData.SE_Para42 = double.TryParse(seMachineData[26], out dd) ? dd : 0;
                productionData.SE_Para40 = double.TryParse(seMachineData[27], out dd) ? dd : 0;
                productionData.SE_Para46 = double.TryParse(seMachineData[28], out dd) ? dd : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData Se其他设备参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }

            //转盘其他设备参数
            var spinMachineData = sections[19].Split(',');
            try
            {
                productionData.MAX_Para1 = int.TryParse(spinMachineData[0], out  tt) ? tt : 0;
                productionData.MAX_Para2 = int.TryParse(spinMachineData[1], out  tt) ? tt : 0;
                productionData.MAX_Para3 = double.TryParse(spinMachineData[2], out dd) ? dd : 0;
                productionData.MAX_Para4 = double.TryParse(spinMachineData[3], out dd) ? dd : 0;
                productionData.MAX_Para5 = int.TryParse(spinMachineData[4], out  tt) ? tt : 0;
                productionData.MAX_Para6 = int.TryParse(spinMachineData[5], out  tt) ? tt : 0;
                productionData.MAX_Para7 = double.TryParse(spinMachineData[6], out dd) ? dd : 0;
                productionData.MAX_Para8 = int.TryParse(spinMachineData[7], out  tt) ? tt : 0;
                productionData.MAX_Para9 = int.TryParse(spinMachineData[8], out  tt) ? tt : 0;

                productionData.MAX_Para10 = int.TryParse(spinMachineData[9], out  tt) ? tt : 0;
                productionData.MAX_Para11 = int.TryParse(spinMachineData[10], out  tt) ? tt : 0;
                productionData.MAX_Para12 = int.TryParse(spinMachineData[11], out  tt) ? tt : 0;
                productionData.MAX_Para13 = int.TryParse(spinMachineData[12], out  tt) ? tt : 0;
                productionData.MAX_Para14 = int.TryParse(spinMachineData[13], out  tt) ? tt : 0;
                productionData.MAX_Para15 = int.TryParse(spinMachineData[14], out  tt) ? tt : 0;
                productionData.MAX_Para16 = int.TryParse(spinMachineData[15], out  tt) ? tt : 0;
                productionData.MAX_Para17 = int.TryParse(spinMachineData[16], out  tt) ? tt : 0;

                productionData.MAX_Para18 = int.TryParse(spinMachineData[17], out  tt) ? tt : 0;
                productionData.MAX_Para19 = int.TryParse(spinMachineData[18], out  tt) ? tt : 0;
                productionData.MAX_Para20 = int.TryParse(spinMachineData[19], out  tt) ? tt : 0;
                productionData.MAX_Para21 = int.TryParse(spinMachineData[20], out  tt) ? tt : 0;
                productionData.MAX_Para22 = int.TryParse(spinMachineData[21], out  tt) ? tt : 0;
                productionData.MAX_Para23 = int.TryParse(spinMachineData[22], out  tt) ? tt : 0;
                productionData.MAX_Para28_Spin = int.TryParse(spinMachineData[23], out  tt) ? tt : 0;
                productionData.MAX_Para28_Laser = int.TryParse(spinMachineData[24], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 转盘其他设备参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //松头设备参数
            var Cis22MachineData = sections[20].Split(',');
            try
            {
                productionData.Cis22Para1 = int.TryParse(Cis22MachineData[0], out  tt) ? tt : 0;
                productionData.Cis22Para2 = int.TryParse(Cis22MachineData[1], out  tt) ? tt : 0;
                productionData.Cis22Para3 = int.TryParse(Cis22MachineData[2], out  tt) ? tt : 0;
                productionData.Cis22Para4 = int.TryParse(Cis22MachineData[3], out  tt) ? tt : 0;
                productionData.Cis22Para5 = int.TryParse(Cis22MachineData[4], out  tt) ? tt : 0;
                productionData.Cis22Para6 = int.TryParse(Cis22MachineData[5], out  tt) ? tt : 0;
                productionData.Cis22Para7 = int.TryParse(Cis22MachineData[6], out  tt) ? tt : 0;

                productionData.Cis22Para8 = int.TryParse(Cis22MachineData[7], out  tt) ? tt : 0;
                productionData.Cis22Para9 = int.TryParse(Cis22MachineData[8], out  tt) ? tt : 0;
                productionData.Cis22Para10 = int.TryParse(Cis22MachineData[9], out  tt) ? tt : 0;
                productionData.Cis22Para11 = int.TryParse(Cis22MachineData[10], out  tt) ? tt : 0;
                productionData.Cis22Para12 = int.TryParse(Cis22MachineData[11], out  tt) ? tt : 0;
                productionData.Cis22Para13 = int.TryParse(Cis22MachineData[12], out  tt) ? tt : 0;
                productionData.Cis22Para14 = int.TryParse(Cis22MachineData[13], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 松头设备参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }


            //测点设备参数
            var Cis24MachineData = sections[21].Split(',');
            try
            {
                productionData.Cis24Para1 = int.TryParse(Cis24MachineData[0], out  tt) ? tt : 0;
                productionData.Cis24Para2 = int.TryParse(Cis24MachineData[1], out  tt) ? tt : 0;
                productionData.Cis24Para3 = int.TryParse(Cis24MachineData[2], out  tt) ? tt : 0;
                productionData.Cis24Para4 = int.TryParse(Cis24MachineData[3], out  tt) ? tt : 0;
                productionData.Cis24Para5 = int.TryParse(Cis24MachineData[4], out  tt) ? tt : 0;
                productionData.Cis24Para6 = int.TryParse(Cis24MachineData[5], out  tt) ? tt : 0;
                productionData.Cis24Para7 = int.TryParse(Cis24MachineData[6], out  tt) ? tt : 0;
                productionData.Cis24Para8 = int.TryParse(Cis24MachineData[7], out  tt) ? tt : 0;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"In {nameof(ZJ17DataConverter)}.ParseData 测点设备参数解析 occur exception:{byteConvert} , {ex},{ex?.StackTrace}");
            }

            return productionData;
        }
    }
}
