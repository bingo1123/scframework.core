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
[Description("地址 OPMT 包含作模式的时间数据")]
public struct OPMT
{ 
	/// <summary>
	/// 上一次的opm生产时间
	/// </summary>
	public uint OpmCleanupTime;                //Time in OpmProduction since last 
	/// <summary>
	/// 激活OP模式时间
	/// </summary>
	public uint OpModeTime;                    // Time in active OP mode 
	/// <summary>
	/// 回退模式时间
	/// </summary>
	public uint OpModeTimeExt;                 // Time in back mode 
	/// <summary>
	/// 前一次的OP模式
	/// </summary>
	public uint OpBackMode;                    // Last OP mode 
    /// <summary>
	/// OP模式启动时间
	/// </summary>
	public uint OpmStartTime;                  // Start time of active OP mode 
	/// <summary>
	/// 在SANA下的OP模式下的时间计数
	/// </summary>
	public uint OpmStopTime_SANA;              // OP mode time counted in SANA  
	/// <summary>
	/// 在SHIS 下的OP模式下的时间计数
	/// </summary>
	public uint OpmStopTime_SHIS;              // OP mode time counted in SHIS 
	/// <summary>
	/// 激活停止的开始时间
	/// </summary>
	public uint StpStartTime;                  // Start time of active stop 
	//停止分析的计时
	public uint StpStopTime_SANA;              // Counted time in stop analysis 
	public uint StpStopTime_SHIS;
}
//根据字节数组直接转结构体，让其自动实现，这样调试完成一个后，后面的基本上不用手动添加，现在这种方式如果数据长度不一样又要进行修改 

#region OPMT
public class OPMTParse
{
	public static (OPMT,string) Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.M5)
	{
		OPMT opmt=new OPMT();
		string err = "";
		try
		{
			//数据解析的开始位置
			int offset = (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)?  HuaniConst.M5ReadDataOffset : HuaniConst.P18ReadDataOffset; 
			#region 读取数据解析  首先获取结构体的包含的字节数量，以及读取的开始位置，这样就能够获取到数据  
			byte[] desResponse = new byte[Unsafe.SizeOf<OPMT>()];//首先定义目标数组
			Array.Copy(response, offset, desResponse, 0, desResponse.Length);//获取所需的数组数据 
			opmt=ParseData.ParSeDataInfo<OPMT>(desResponse,new OPMT());//实现数据解析
			#endregion
		}
		catch(Exception ex)
		{
			err = ex.Message;
			System.Diagnostics.Debug.WriteLine("OPMT Parse Failure");
		}
		return (opmt,err);
	}
}
#endregion
