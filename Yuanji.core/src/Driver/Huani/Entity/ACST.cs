using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;

using  Yuanji.core.src.Driver.Huani.Controller;

namespace  Yuanji.core.src.Driver.Huani.Entity;

/// <summary>
/// 激活停止信息<br/>
/// zj116 协议使用的结构体
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Description("激活停止信息")]
public struct ACST
{
	/// <summary>
	/// 消息数量
	/// </summary>
	public ushort No { get; set; }
	/// <summary>
	/// 消息文本Id
	/// </summary>
	public ushort Tid { get; set; }
}

#region ACST
public class ACSTParser
{
	public static ACST Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.M5)
	{
		ACST acst = new ACST();
		int offset;//获取偏移量
		if(type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
			offset = HuaniConst.M5ReadDataOffset;
		else
			offset = HuaniConst.P18ReadDataOffset;  
		#region 读取数据解析
		acst.No= BitConverter.ToUInt16(response, offset);
		acst.Tid =  BitConverter.ToUInt16(response, offset+2);
		#endregion
		return acst;
	}
}
#endregion
