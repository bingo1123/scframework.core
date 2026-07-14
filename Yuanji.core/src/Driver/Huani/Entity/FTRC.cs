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
    [Description("MAX滤嘴接收信息")]
    public struct FTRC
    {
        [Description("一管显示速度")]
        public int display_filter_speed_1;         // 一管显示速度

        [Description("一管接收数量")]
        public int Num_filter_received_1;         // 一管接收数量

        [Description("一管纵向堵塞检测")]
        public bool B12E_B13E;         // 一管纵向堵塞检测

        [Description("一管横向堵塞检测")]
        public bool B11E;         // 一管横向堵塞检测

        [Description("供嘴站一准备好")]
        public bool K12E;         // 供嘴站一准备好

        [Description("管道一要料")]
        public bool filter_request_1;         // 管道一要料

        [Description("管道一门开关监控")]
        public bool S110E;         // 管道一门开关监控

        [Description("管道一堵塞清洁")]
        public bool S120E;         // 管道一堵塞清洁

        [Description("管道一截止阀")]
        public bool Y12;         // 管道一截止阀

        [Description("管道一终端排气")]
        public bool Y13;         // 管道一终端排气

        [Description("二管显示速度")]
        public int display_filter_speed_2;         // 二管显示速度

        [Description("二管接收数量")]
        public int Num_filter_received_2;         // 二管接收数量

        [Description("二管纵向堵塞检测")]
        public bool B22E_B23E;         // 二管纵向堵塞检测

        [Description("二管横向堵塞检测")]
        public bool B21E;         // 二管横向堵塞检测

        [Description("供嘴站二准备好")]
        public bool K22E;         // 供嘴站二准备好

        [Description("管道二要料")]
        public bool filter_request_2;         // 管道二要料

        [Description("管道二门开关监控")]
        public bool S210E;         // 管道二门开关监控

        [Description("管道二堵塞清洁")]
        public bool S220E;         // 管道二堵塞清洁

        [Description("管道二截止阀")]
        public bool Y22;         // 管道二截止阀

        [Description("管道二终端排气")]
        public bool Y23;         // 管道二终端排气

        [Description("三管显示速度")]
        public int display_filter_speed_3;         // 三管显示速度

        [Description("三管接收数量")]
        public int Num_filter_received_3;         // 三管接收数量

        [Description("三管纵向堵塞检测")]
        public bool B32E_B33E;         // 三管纵向堵塞检测

        [Description("三管横向堵塞检测")]
        public bool B31E;         // 三管横向堵塞检测

        [Description("供嘴站三准备好")]
        public bool K32E;         // 供嘴站三准备好

        [Description("管道三要料")]
        public bool filter_request_3;         // 管道三要料

        [Description("管道三门开关监控")]
        public bool S310E;         // 管道三门开关监控

        [Description("管道三堵塞清洁")]
        public bool S320E;         // 管道三堵塞清洁

        [Description("管道三截止阀")]
        public bool Y32;         // 管道三截止阀

        [Description("管道三终端排气")]
        public bool Y33;         // 管道三终端排气

        [Description("四管显示速度")]
        public int display_filter_speed_4;         // 四管显示速度

        [Description("四管接收数量")]
        public int Num_filter_received_4;         // 四管接收数量

        [Description("四管纵向堵塞检测")]
        public bool B42E_B43E;         // 四管纵向堵塞检测

        [Description("四管横向堵塞检测")]
        public bool B41E;         // 四管横向堵塞检测

        [Description("供嘴站四准备好")]
        public bool K42E;         // 供嘴站四准备好

        [Description("管道四要料")]
        public bool filter_request_4;         // 管道四要料

        [Description("管道四门开关监控")]
        public bool S410E;         // 管道四门开关监控

        [Description("管道四堵塞清洁")]
        public bool S420E;         // 管道四堵塞清洁

        [Description("管道四截止阀")]
        public bool Y42;         // 管道四截止阀

        [Description("管道四终端排气")]
        public bool Y43;         // 管道四终端排气

        [Description("入口端管道一监控")]
        public bool B10E;         // 入口端管道一监控

        [Description("入口端管道二监控")]
        public bool B20E;         // 入口端管道二监控

        [Description("入口端管道三监控")]
        public bool B30E;         // 入口端管道三监控

        [Description("入口端管道四监控")]
        public bool B40E;         // 入口端管道四监控
    }
}
