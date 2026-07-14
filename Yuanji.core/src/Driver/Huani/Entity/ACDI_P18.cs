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
    /// 显示在MLP故障信息字段中的当前信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前事件信息（停机/故障/操作）")]
    [Serializable]
    public struct ACDI_P18
    {
        // 警告/停机消息
        [MessageType("警告/停机消息")]
        [Description("节点ID")]
        public ushort faNode;            // 节点ID

        [MessageType("警告/停机消息")]
        [Description("功能编号")]
        public ushort faFunction;        // 功能编号

        [MessageType("警告/停机消息")]
        [Description("消息编号")]
        public ushort faNo;              // 消息编号

        [MessageType("警告/停机消息")]
        [Description("文本索引")]
        public ushort faTid;             // 文本索引

        [MessageType("警告/停机消息")]
        [Description("帮助索引")]
        public ushort faHid;             // 帮助索引

        [MessageType("警告/停机消息")]
        [Description("文本颜色")]
        public ushort faForeColor;       // 文本颜色

        [MessageType("警告/停机消息")]
        [Description("背景颜色")]
        public ushort faBackColor;       // 背景颜色

        // 操作员消息
        [MessageType("操作员消息")]
        [Description("节点ID")]
        public ushort opNode;            // 节点ID

        [MessageType("操作员消息")]
        [Description("功能编号")]
        public ushort opFunction;        // 功能编号

        [MessageType("操作员消息")]
        [Description("消息编号")]
        public ushort opNo;              // 消息编号

        [MessageType("操作员消息")]
        [Description("文本索引")]
        public ushort opTid;             // 文本索引

        [MessageType("操作员消息")]
        [Description("帮助索引")]
        public ushort opHid;             // 帮助索引

        [MessageType("操作员消息")]
        [Description("文本颜色")]
        public ushort opForeColor;       // 文本颜色

        [MessageType("操作员消息")]
        [Description("背景颜色")]
        public ushort opBackColor;       // 背景颜色

        // 系统消息
        [MessageType("系统消息")]
        [Description("节点ID")]
        public ushort sysNode;           // 节点 ID

        [MessageType("系统消息")]
        [Description("功能编号")]
        public ushort sysFunction;       // 功能编号

        [MessageType("系统消息")]
        [Description("消息编号")]
        public ushort sysNo;             // 消息编号

        [MessageType("系统消息")]
        [Description("文本索引")]
        public ushort sysTid;            // 文本索引

        [MessageType("系统消息")]
        [Description("帮助索引")]
        public ushort sysHid;            // 帮助索引

        [MessageType("系统消息")]
        [Description("文本颜色")]
        public ushort sysForeColor;      // 文本颜色

        [MessageType("系统消息")]
        [Description("背景颜色")]
        public ushort sysBackColor;      // 背景颜色
    }

}
