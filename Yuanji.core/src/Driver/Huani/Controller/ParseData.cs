using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Controller;
internal class ParseData
{

	/// <summary>
	/// 实现将返回的数据转换为对象
	/// </summary>
	/// <typeparam name="T">结构体类型</typeparam>
	/// <param name="response">响应的数据类型</param>
	/// <param name="obj">结构体对象</param>
	/// <returns></returns>
	public static T ParSeDataInfo<T>(byte[] response, T obj) where T : struct  
	{
		T result ;
		GCHandle handle = GCHandle.Alloc(response, GCHandleType.Pinned);//实现内存分配
		try
		{
			IntPtr ptr = handle.AddrOfPinnedObject();
			result = (T)Marshal.PtrToStructure(ptr, typeof(T));
		}
		finally
		{
			handle.Free();//释放内存
		}
		return result;
	}

}
