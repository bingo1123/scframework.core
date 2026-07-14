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
    /// 当前激活的停机信息<br/>
    /// M5 协议使用的结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前激活的停机信息")]
    [Serializable]
    public struct ACDI
    {
        [Description("消息编号")]
        public uint No;         // 消息编号

        [Description("消息的文本ID编号")]
        public uint Tid;        // 消息的文本ID编号

        [Description("消息优先级")]
        public uint Priority;   // 消息优先级
    }
}
