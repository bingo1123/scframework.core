using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;

namespace  Yuanji.core.src.Driver.Huani.Controller;
public class HuniCommunication
{

	#region 属性定义
	/// <summary>
	/// 地址
	/// </summary>
	public string IpAddress { get; set; }
	/// <summary>
	/// 端口号
	/// </summary>
	public int Port { get; set; }
	/// <summary>
	/// 连接对象
	/// </summary>
	private TcpClient client;
	/// <summary>
	/// 流对象
	/// </summary>
	private NetworkStream stream;

	#endregion
	#region 方法定义
	public string Connect(string ipAddress, int port)
	{
		string err = "";
		if(client == null || !client.Connected)
		{
			client.Connect(ipAddress, port);
			if(!client.Connected)
			{
				err = "连接失败";
			}
		}
		return err;
	}

	public (string, Dictionary<string, object>) Read(CommunicationInfo communicationInfo)
	{
		if(client != null && client.Connected)
		{
			//实现数据读取
			Dictionary<string, object> dic = new Dictionary<string, object>();
			#region 构建命令
			List<byte> cmds = new List<byte>();
			//构建读取参数命令
			cmds.Add(0X00);
			cmds.Add(0X00);
			cmds.Add((byte)communicationInfo.FType);//功能类型

			foreach(var  symb in communicationInfo.DataItems)
			{
				if(communicationInfo.MType == MachineType.M5 || communicationInfo.MType == MachineType.BOBME)
				{
					//if (Enum.IsDefined(typeof(SymbAdrEnum), symb))
					{
						cmds.Add((byte)communicationInfo.NodeId);//nodeAddress 
						//if (symb == SymbAdrEnum.ProductionCounter.ToString()
						//    || symb.Contains(SymbAdrEnum.ShiftData.ToString()))
						//    command.Add(2);//ProductionCounter ,ShiftData functionAddress为2
						//else

						cmds.Add((byte)communicationInfo.FuncId);//其他的basic symbAddress functionAddress 为1
					}
					cmds.AddRange(VisuHelper.SymbAddressConvert(symb.ParName));//bobme地址符号为节点地址+功能码+符号地址
				}
				else
				{
					cmds.Add((byte)communicationInfo.FuncId);
					cmds.AddRange(VisuHelper.SymbAddressConvertForP18(symb.ParName));
				}
				cmds.Add(0x00);
				cmds.Add(0x00);
				cmds.Add(0xFF);
				cmds.Add(0xFF);
			}  
			//添加符号地址，每个符号地址以\0结尾  
			#endregion
			#region 发送命令
			#endregion
			#region 接受数据
			//接手数据，需要判定接受的数据是不是发送命令返回的数据以及返回的数据长度是否一致，如果不一致则需要提示
			#endregion
			#region 解析数据

			#endregion
			return ("", dic);
		}
		else
		{
			return (client == null ? "请先创建连接" : "连接掉线", null);
		}


	}
	#endregion
}
public class CommunicationInfo
{
	public int NodeId { get; set; }

	public int FuncId { get; set; }

	public List<Item> DataItems { get; set; } = new List<Item>();//数据项

	public MachineType MType { get; set; }//机台类型

	public FunType FType { get; set; }//功能类型
}
public class Item
{
	public int StartIndex { get; set; }//索引
	public string DataType { get; set; }//数据类型 
	public string ParName { get; set; }//参数名称
}
//不同的机型读取参数发的命令，以及解析的也不一样，所以需要根据
public enum MachineType
{
	M5,

	/// <summary>
	/// Protos 1-8 对应ZJ116
	/// </summary>
	P18,

	ZJ119,

	ZJ117,

	BOBME,
}

public enum FunType
{
	ReadData = 0x11,//读取数据

	ReadBrandParameter = 0x21,//读取品牌参数

	ReadMachineParameter = 0x31,//读取机台参数

	/// <summary>
	/// 在ZJ16机型中的功能是ReadMLParameter
	/// </summary>
	ReadParameter = 0x51,//读取参数

	ReadMessageParameter = 0x61,//读取信息参数

	/// <summary>
	/// PROTOS 1–8, 对应ZJ16机型
	/// </summary>
	//ReadShiftDataOnlyFor1to8 = 0x83,

}

