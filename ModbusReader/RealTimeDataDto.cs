using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RealTimeDataDto
{
    /// <summary>
    /// 设备Id
    /// </summary>
    public string MachineId
    { get; set; }


    /// <summary>
    /// 时间戳
    /// </summary>
    public long Ts
    { get; set; }
    public List<DataItemDetailDto> Details { get; set; }
}

public class DataItemDetailDto
{
    /// <summary>
    /// 标签代码--PlcNet(1)，MStatus(1
    /// </summary>
    public string Code
    { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public object Value
    { get; set; }

    /// <summary>
    /// 数据质量
    /// </summary>
    public bool IsGood
    { get; set; }

    /// <summary>
    /// 数据类型.
    /// </summary>
    public string DType => Heleper.GetObjectTypeString(this.Value);
}

public class Heleper
{
    /// <summary>
    /// 获取对象类型字符串
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetObjectTypeString(object obj)
    {
        if (obj == null)
            return "null";

        // 获取实际运行时类型
        Type type = obj.GetType();

        // 返回类型的名称
        return type.Name;
    }
}