using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Controller
{
    public enum FxsZB48CommandEnum
    {
        ProductionData,

        CalculationOfEfficiency,

        CurrentMachineStatus,

        MachineParameter,

        Times,

        MaxSpeedAndCurrentSpeed,

        WarningInfos,

        RejectInfos,

        SpeedTimeCost,

        BasicInfo,

        PresetInfos,
    }

    public class FxsZB48Command
    {
        public static string GetCommandDataType(FxsZB48CommandEnum type)//数据命令
        {
            string command = type switch
            {
                FxsZB48CommandEnum.ProductionData => "A30-J30",
                FxsZB48CommandEnum.CalculationOfEfficiency => "Q2-Q25",
                FxsZB48CommandEnum.CurrentMachineStatus => "B41-D600",

                FxsZB48CommandEnum.MachineParameter => "B1-B8",
                FxsZB48CommandEnum.Times => "B13-B19",
                FxsZB48CommandEnum.MaxSpeedAndCurrentSpeed => "E22-E23",
                FxsZB48CommandEnum.WarningInfos => "H41-J300",
                FxsZB48CommandEnum.RejectInfos => "M41-O300",
                FxsZB48CommandEnum.SpeedTimeCost => "E13-E20",
                FxsZB48CommandEnum.BasicInfo => "B1-B8",
                FxsZB48CommandEnum.PresetInfos => "Q41-S600",
                _ => string.Empty,
            };
            return command;
        }
    }
}
