using System.ComponentModel;
using System.Runtime.InteropServices;
namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 4,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SRM废品比例信息")]
    public struct SRM_SPOT
    {
        [Description("前道后排区域1软点数")]
        public ushort Zone1SP0_v;         // 前道后排区域1软点数

        [Description("前道后排区域2软点数")]
        public ushort Zone2SP0_v;         // 前道后排区域2软点数

        [Description("前道后排区域3软点数")]
        public ushort Zone3SP0_v;         // 前道后排区域3软点数

        [Description("前道后排区域1硬点数")]
        public ushort Zone1HP0_v;         // 前道后排区域1硬点数

        [Description("前道后排区域2硬点数")]
        public ushort Zone2HP0_v;         // 前道后排区域2硬点数

        [Description("前道后排区域3硬点数")]
        public ushort Zone3HP0_v;         // 前道后排区域3硬点数

        [Description("前道前排区域1软点数")]
        public ushort Zone1SP1_v;         // 前道前排区域1软点数

        [Description("前道前排区域2软点数")]
        public ushort Zone2SP1_v;         // 前道前排区域2软点数

        [Description("前道前排区域3软点数")]
        public ushort Zone3SP1_v;         // 前道前排区域3软点数

        [Description("前道前排区域1硬点数")]
        public ushort Zone1HP1_v;         // 前道前排区域1硬点数

        [Description("前道前排区域2硬点数")]
        public ushort Zone2HP1_v;         // 前道前排区域2硬点数

        [Description("前道前排区域3硬点数")]
        public ushort Zone3HP1_v;         // 前道前排区域3硬点数

        [Description("后道后排区域1软点数")]
        public ushort Zone1SP0_h;         // 后道后排区域1软点数

        [Description("后道后排区域2软点数")]
        public ushort Zone2SP0_h;         // 后道后排区域2软点数

        [Description("后道后排区域3软点数")]
        public ushort Zone3SP0_h;         // 后道后排区域3软点数

        [Description("后道后排区域1硬点数")]
        public ushort Zone1HP0_h;         // 后道后排区域1硬点数

        [Description("后道后排区域2硬点数")]
        public ushort Zone2HP0_h;         // 后道后排区域2硬点数

        [Description("后道后排区域3硬点数")]
        public ushort Zone3HP0_h;         // 后道后排区域3硬点数

        [Description("后道前排区域1软点数")]
        public ushort Zone1SP1_h;         // 后道前排区域1软点数

        [Description("后道前排区域2软点数")]
        public ushort Zone2SP1_h;         // 后道前排区域2软点数

        [Description("后道前排区域3软点数")]
        public ushort Zone3SP1_h;         // 后道前排区域3软点数

        [Description("后道前排区域1硬点数")]
        public ushort Zone1HP1_h;         // 后道前排区域1硬点数

        [Description("后道前排区域2硬点数")]
        public ushort Zone2HP1_h;         // 后道前排区域2硬点数

        [Description("后道前排区域3硬点数")]
        public ushort Zone3HP1_h;         // 后道前排区域3硬点数
    }
}
