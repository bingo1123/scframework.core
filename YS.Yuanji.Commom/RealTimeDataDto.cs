

namespace YS.Yuanji.Commom
{
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

    public class RealTimeDataDto<T>
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
        public List<T> Details { get; set; }
    }

    public class RealTimeDataWithDeviceDto : RealTimeDataDto
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceId
        { get; set; }
    }


    public class DataItemDetailDto
    {
        /// <summary>
        /// 标签描述.
        /// </summary>
        public string Name
        { get; set; }

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
        public string DType => VisuHelper.GetObjectTypeString(this.Value);

    }


    public class WriteItemDto
    {
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        /// </summary>
        public object Target { get; set; }
    }


    public class WriteDataDto
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 指令类型
        /// </summary>
        public string Command { get; set; }


        public WriteItemDto Value { get; set; }

        // <summary>
        /// 设备编号
        /// </summary>
        public string DeviceId { get; set; }

        // <summary>
        /// 时间戳
        /// </summary>
        public long Tstamp { get; set; }
    }

    public class ReponseWriteDataDto
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public string ErrorCodde { get; set; }


        public object ActualValue { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Tstamp { get; set; }
    }

    public class DataRealtimePb: DataItemDetailDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }


    }

}
