using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YS.Yuanji.Log;


namespace YS.Yuanji.Commom
{
    internal class RealTimeDataDtoHelp
    {

        public static RealTimeDataDto Create(string machineId,List<object> list)
        {
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = machineId;
            try
            {
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        var fields = item.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            var fieldName = $"{machineId}.{field.Name}"; // 字段名
                            var fieldValue = field.GetValue(item); // 当前值
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LogController.Instance.Log($"RealTimeDataDtoHelp Create .{e.ToString()}");
            }

            return  realTimeDataDto;

        }
    }
}
