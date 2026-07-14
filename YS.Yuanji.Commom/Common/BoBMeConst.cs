using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    public class BoBMeConst
    {
        public readonly static string[] BobProductData = new string[]
       {
            "ShiftData.Date",
            "ShiftData.ShiftStart",
            "ShiftData.ShiftTime",
            "ShiftData.RunTime",
            "ShiftData.StopTime",
            "ShiftData.Stop.Cnt",
            "ShiftData.EffMachineT",
            "ShiftData.EffProductionT",
            "ShiftData.TotalBobbins",
       };

        public readonly static string[] BobHMIData = new string[]
        {
            "SpeedOfBOB",
            "DiameterRightBobbi",
            "DiameterLeftBobbin",
            "Bobinchangecounter",
        };
    }
}
