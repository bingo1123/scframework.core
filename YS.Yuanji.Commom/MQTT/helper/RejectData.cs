using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom.helper
{
    public class RejectData
    {
        /// <summary>
        /// 终端采集时间
        /// </summary>
        public DateTime TestTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 采集状态(0为正常，1为未连接到设备，2为数据格式异常，3为取牌号或班次信息时出错，9为采集过程中出现未知的异常)
        /// </summary>
        public int? State { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public bool ErrMsg { get; set; }
        /// <summary>
        /// 外观检验设备编号对应【PB_QL_OnlineDevice】表的【Code】字段
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 生产日期(可为空)
        /// </summary>
        public DateTime? ProduceDate { get; set; }
        /// <summary>
        /// 牌号(可为空)
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 班组(可为空)
        /// </summary>
        public string ClassId { get; set; }
        /// <summary>
        /// 班次(可为空)
        /// </summary>
        public string ClassTimeId { get; set; }
        /// <summary>
        /// 总剔除数（设备不包含该项输出时返回空）
        /// </summary>
        public int? KickCount { get; set; }
        /// <summary>
        /// 总检测数（设备不包含该项输出时返回空）
        /// </summary>
        public int? CheckCount { get; set; }
        /// <summary>
        /// 设备全局参数
        /// </summary>
        public Dictionary<string, string> CfgParams { get; set; }
        /// <summary>
        /// 设备相机参数（外层字典Key为相机编码，内层字典Key为相机的参数编码）（如设备没有则不采集）
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> CameraCfgParams { get; set; }
        /// <summary>
        /// 缺陷明细列表
        /// </summary>
        public List<RejectDetailData> DefectList { get; set; }
    }
    /// <summary>
    /// 剔除缺陷明细
    /// </summary>
    public class RejectDetailData
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        public string MachineId { get; set; }
        /// <summary>
        /// 工位编码
        /// </summary>
        public string PartCode { get; set; }
        /// <summary>
        /// 相机编码
        /// </summary>
        public string CameraCode { get; set; }
        /// <summary>
        /// 组件模块编码
        /// </summary>
        public string ModuleCode { get; set; } = string.Empty;

        /// <summary>
        /// 终端的缺陷编码
        /// </summary>
        public string RejectCode { get; set; }
        /// <summary>
        /// 缺陷数量(一个采集周期内检测的缺陷数量)
        /// </summary>
        public int DefectNum { get; set; }
        /// <summary>
        /// 缺陷总量(当前牌号、当前班的累计缺陷数量)
        /// </summary>
        public int DefectCount { get; set; }

        /// <summary>
        /// 图片文件名（传到Minio后的文件名）
        /// </summary>
        public string PicFile { get; set; }
    }
}
