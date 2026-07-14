using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace YS.Yuanji.Commom
{
    public static class VisuHelper
    {
        public static T ParseStruct<T>(byte[] data, int offset) where T : struct
        {
            // 确保目标类型是一个结构体
            if (!typeof(T).IsValueType || !typeof(T).IsLayoutSequential && !typeof(T).IsExplicitLayout)
                throw new InvalidOperationException("T must be a value type with Sequential or Explicit layout.");

            // 获取目标结构体的大小
            int size = Marshal.SizeOf<T>();
            if (offset + size > data.Length)
                throw new ArgumentException("The provided byte array does not contain enough data to parse the struct.");

            // 分配非托管内存并复制数据
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                // 将字节数组的数据拷贝到非托管内存中
                Marshal.Copy(data, offset, ptr, size);

                // 从非托管内存中解析为目标结构体
                return Marshal.PtrToStructure<T>(ptr)!;
            }
            finally
            {
                // 释放非托管内存
                Marshal.FreeHGlobal(ptr);
            }
        }


        public static string ToJson<T>(T obj, bool isShowFildName = false) where T : struct
        {
            var jsonObject = new Dictionary<string, object>();
            var rootDescription = GetDescription(typeof(T));
            string rootName = rootDescription ?? typeof(T).Name;
            jsonObject[rootName] = new Dictionary<string, object>();

            var fieldList = new Dictionary<string, object>();
            foreach (var field in typeof(T).GetFields())
            {
                string? description = isShowFildName ? field.Name : GetDescription(field);
                if (description != null)
                {
                    var fieldValue = field.GetValue(obj);
                    if (fieldValue is Array array)
                    {
                        var arrayList = new List<Dictionary<string, object>>();
                        foreach (var item in array)
                        {
                            arrayList.Add(ProcessStruct(item));
                        }
                        fieldList[description] = arrayList;
                    }
                    else if (fieldValue is ValueType || fieldValue == null || !(fieldValue is Nullable))
                    {
                        fieldList[description] = fieldValue;
                    }
                    else
                    {
                        fieldList[description] = ProcessStruct(fieldValue);
                    }
                }

            }
            jsonObject[rootName] = fieldList;
            return JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
        }

        private static Dictionary<string, object> ProcessStruct(object obj, bool isShowFildName = false)
        {
            var structDescription = new Dictionary<string, object>();

            foreach (var field in obj.GetType().GetFields())
            {
                string? description = isShowFildName ? field.Name : GetDescription(field);
                if (description != null)
                {
                    var fieldValue = field.GetValue(obj);
                    structDescription[description] = fieldValue == null ? "null" : fieldValue;
                }
            }
            return structDescription;
        }

        private static string? GetDescription(MemberInfo member)
        {
            var descriptionAttribute = member.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description;
        }

        /// <summary>
        /// 内容转换
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="contentLength">内容长度,自动补零</param>
        /// <returns></returns>
        public static byte[] ContentConvert(string content, int contentLength)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(content);
            byte[] result = new byte[contentLength];
            Array.Copy(bytes, result, Math.Min(bytes.Length, contentLength));
            return result;
        }

        public static byte[] SymbAddressConvertForP18(string symbAdr)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(symbAdr);
            byte[] result = new byte[4];//P18的协议，符号地址的长度为4 ，地址超出四个字节的都有问题
            Array.Copy(bytes, result, Math.Min(bytes.Length, 4));
            return result;
        }

        public static byte[] SymbAddressConvert(string symbAdr)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(symbAdr);
            byte[] result = new byte[64];
            Array.Copy(bytes, result, Math.Min(bytes.Length, 64));
            return result;
        }

        public static byte[] BrandNameConvert(string brandName = "")
        {
            byte[] result = new byte[40];//品牌参数占40个字节
            if (!string.IsNullOrEmpty(brandName))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(brandName);
                Array.Copy(bytes, result, Math.Min(bytes.Length, 40));
            }
            else
            {
                var res = Encoding.ASCII.GetBytes("\0");
                result[0] = res[0];
            }
            return result;
        }

        public static string ParseSymbAdrNameP18(byte[] response)
        {
            return Encoding.ASCII.GetString(response, 4, 4).Replace("\0", string.Empty).Trim();
        }

        public static string ParseSymbAdrName(byte[] response)
        {
            //Todo 多symb形式兼容
            return Encoding.ASCII.GetString(response, 5, 64).Replace("\0", string.Empty).Trim();
        }

        public static byte[] GetLength(int length)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(length & 0xFF);//获取低八位的值
            bytes[1] = (byte)((length >> 8) & 0xFF);//获取高八位的值
            return bytes;
        }



        public static string P18Uint32ToDateTime(uint seconds)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

            DateTime resultDateTime = baseDate.AddSeconds(seconds).AddHours(-4);

            DateTime localDateTime = resultDateTime.ToUniversalTime();
            return resultDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }


        public static string ZJ118Uint32ToDateTime(uint seconds)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

            DateTime resultDateTime = baseDate.AddSeconds(seconds).AddHours(-4);

            DateTime localDateTime = resultDateTime.ToUniversalTime();
            return resultDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }


        public static string M5Uint32ToDateTime(uint seconds)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

            DateTime resultDateTime = baseDate.AddSeconds(seconds).AddHours(10);// 这里加10 可能是huani M5 使用得是德国时区时间，所以才加得ExtendTime

            DateTime localDateTime = resultDateTime.ToUniversalTime();
            return localDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string M5ExtendDatatimeToHHmmss(DATETIME? extendDateTime)
        {
            return $"{extendDateTime.Value.Hour}:{extendDateTime.Value.Minute}:{extendDateTime.Value.Second}";
        }

        public static string M5ExtendDatatimeToDateTime(DATETIME? extendDateTime)
        {
            return $"{extendDateTime.Value.Year}-{extendDateTime.Value.Month}-{extendDateTime.Value.Day} {extendDateTime.Value.Hour}:{extendDateTime.Value.Minute}:{extendDateTime.Value.Second}";
        }

        public static string GetP18StopCodeForSHIS(uint number)
        {
            var bytes = BitConverter.GetBytes(number);
            return $"{bytes[1].ToString("X2")}{bytes[0].ToString("x2")}";
        }

        public static string GetM5StopCodeForSHIS(uint munber)
        {
            var bytes = BitConverter.GetBytes(munber);
            return $"{bytes[3].ToString("x3")}.{bytes[2].ToString("D3")}.{bytes[0].ToString("D4")}";
        }


        public static int HHmmssToTotalSeconds(string HHmmss)
        {
            return ToFxsZB48HHmmss(HHmmss);
        }

        public static int HHmmssToTotalSecondsZJ17(string HHmmss)
        {
            return ZJ17HHmmss(HHmmss);
        }

        public static string ToFxsZB48DateTime(string datetime)
        {
            try
            {
                if (double.TryParse(datetime, out double d))
                {
                    DateTime date = DateTime.FromOADate(d);
                    return date.ToString(MqttConst.DateTimeFormat);
                }
                else
                {
                    return datetime;
                }

            }
            catch (Exception ex)
            {
                return DateTime.Now.ToString(MqttConst.DateTimeFormat);
            }
        }


        public static string ToFxsZB48Time(string datetime)
        {
            try
            {
                if (double.TryParse(datetime, out double d))
                {
                    DateTime date = DateTime.FromOADate(d);
                    return date.ToString(MqttConst.HHmmssFormat);
                }
                else
                {
                    return datetime;
                }

            }
            catch (Exception ex)
            {
                return DateTime.Now.ToString(MqttConst.HHmmssFormat);
            }
        }

        public static int ToFxsZB48HHmmss(string hhmmss)
        {
            try
            {
                if (double.TryParse(hhmmss, out double d))
                {
                    TimeSpan time = TimeSpan.FromDays(d);

                    return time.Hours * 3600 + time.Minutes * 60 + time.Seconds;
                }
                else if (hhmmss.Contains(":"))
                {
                    string[] parts = hhmmss.Split(':');
                    if (parts.Length == 3)
                    {
                        int hours = int.Parse(parts[0]);
                        int minutes = int.Parse(parts[1]);
                        int seconds = int.Parse(parts[2]);
                        return hours * 3600 + minutes * 60 + seconds;
                    }

                    if (parts.Length == 2)
                    {
                        int minutes = int.Parse(parts[0]);
                        int seconds = int.Parse(parts[1]);
                        return minutes * 60 + seconds;
                    }
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"Error parsing time: {ex.Message}");
            }

            // 如果不是有效的时间格式，返回默认值
            if (int.TryParse(hhmmss, out int result))
                return result;
            else
                return 0;
        }

        public static int ZJ17HHmmss(string hhmmss)
        {
            try
            {
                if (double.TryParse(hhmmss, out double d))
                {
                    TimeSpan time = TimeSpan.FromDays(d);

                    return time.Hours * 3600 + time.Minutes * 60 + time.Seconds;
                }
                else if (hhmmss.Contains(":"))
                {
                    string[] parts = hhmmss.Split(':');
                    if (parts.Length == 3)
                    {
                        int hours = int.Parse(parts[0]);
                        int minutes = int.Parse(parts[1]);
                        int seconds = int.Parse(parts[2]);
                        return hours * 3600 + minutes * 60 + seconds;
                    }

                    if (parts.Length == 2)
                    {
                        int hours = int.Parse(parts[0]);
                        int minutes = int.Parse(parts[1]);
                        return hours * 60 * 60 + minutes * 60;
                    }
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"Error parsing time: {ex.Message}");
            }

            // 如果不是有效的时间格式，返回默认值
            if (int.TryParse(hhmmss, out int result))
                return result;
            else
                return 0;
        }

        public static Dictionary<string, object> GetStructFields<T>(T structInstance) where T : struct
        {
            var fieldValues = new Dictionary<string, object>();

            // 获取类型信息
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // 遍历字段
            foreach (var field in fields)
            {
                var fieldName = field.Name; // 字段名
                var fieldValue = field.GetValue(structInstance); // 当前值
                fieldValues.Add(fieldName, fieldValue);
            }

            return fieldValues;
        }

        public static byte ConvertToByte(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            if (input.StartsWith("0x"))
            {
                input = input.Substring(2);
            }

            if (Regex.IsMatch(input, @"^[A-Fa-f]"))
                return byte.Parse(input, NumberStyles.HexNumber);

            return byte.Parse(input, NumberStyles.HexNumber);
        }
        public static byte ParseByte(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            input = input.Trim();

            // 处理0x前缀的十六进制
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                input = input[2..];

            // 尝试十六进制
            if (byte.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hexResult))
                return hexResult;

            // 尝试十进制
            if (byte.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var decResult))
                return decResult;

            return 0;
        }

        /// <summary>
        /// 从指定路径读取文件内容并反序列化为T对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="filePath">文件路径</param>
        /// <returns>反序列化后的T对象</returns>
        public static T? ReadFileToObject<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("文件不存在", filePath);

            string content = File.ReadAllText(filePath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(content);
        }

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
}
