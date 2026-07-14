using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Commom.Common
{
    public class ConvertDataType
    {
        /// <summary>
        /// 根据字符串返回对应的Type类型（增强版）
        /// </summary>
        /// <param name="typeName">类型名称字符串</param>
        /// <param name="throwOnError">找不到类型时是否抛出异常</param>
        /// <returns>对应的Type类型</returns>
        public static Type GetSystemType(string typeName, bool throwOnError = false)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                if (throwOnError)
                    throw new ArgumentException("类型名称不能为空", nameof(typeName));
                return typeof(object);
            }

            // 转换为小写进行比较
            string lowerTypeName = typeName.ToLowerInvariant().Trim();

            try
            {
                // 基本数据类型映射
                Type result = lowerTypeName switch
                {
                    "int" or "int32" => typeof(int),
                    "long" or "int64" => typeof(long),
                    "short" or "int16" or "word" => typeof(short),
                    "byte" => typeof(byte),
                    "sbyte" => typeof(sbyte),
                    "uint" or "uint32" or "dword" => typeof(uint),
                    "ulong" or "uint64" => typeof(ulong),
                    "ushort" or "uint16" or "uword" => typeof(ushort),
                    "float" or "real" => typeof(float),
                    "double"  => typeof(double),
                    "decimal" => typeof(decimal),
                    "bool" or "boolean" => typeof(bool),
                    "char" => typeof(char),
                    "string" => typeof(string),
                    "datetime" => typeof(DateTime),
                    "datetimeoffset" => typeof(DateTimeOffset),
                    "timespan" => typeof(TimeSpan),
                    "guid" => typeof(Guid),
                    "object" => typeof(object),
                    "void" => typeof(void),
                    _ => null
                };

                if (result != null)
                    return result;

                // 处理数组类型
                if (lowerTypeName.EndsWith("[]"))
                {
                    string elementType = typeName.Substring(0, typeName.Length - 2);
                    Type elementTypeObj = GetSystemType(elementType, false);
                    return elementTypeObj.MakeArrayType();
                }

                // 处理泛型类型（简单处理List<T>）
                if (lowerTypeName.StartsWith("list<") && lowerTypeName.EndsWith(">"))
                {
                    string innerType = typeName.Substring(5, typeName.Length - 6);
                    Type innerTypeObj = GetSystemType(innerType, false);
                    return typeof(List<>).MakeGenericType(innerTypeObj);
                }

                // 尝试通过Type.GetType获取类型
                Type type = Type.GetType(typeName);
                if (type != null)
                    return type;

                // 尝试在常用命名空间中查找
                string[] namespaces = {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.IO",
            "System.Net",
            "System.Threading.Tasks"
        };

                foreach (string ns in namespaces)
                {
                    type = Type.GetType($"{ns}.{typeName}");
                    if (type != null)
                        return type;
                }

                // 检查当前加载的程序集
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(typeName);
                    if (type != null)
                        return type;

                    // 尝试带命名空间的查找
                    foreach (string ns in namespaces)
                    {
                        type = assembly.GetType($"{ns}.{typeName}");
                        if (type != null)
                            return type;
                    }
                }
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw new InvalidOperationException($"无法解析类型: {typeName}", ex);
            }

            if (throwOnError)
                throw new InvalidOperationException($"无法找到类型: {typeName}");

            return typeof(object);
        }
        /// <summary>
        /// 将对象转换为指定类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的对象</returns>
        public static object ConvertToType(object value, Type targetType)
        {
            // 处理空值情况
            if (value == null || value == DBNull.Value)
            {
                if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                {
                    // 对于不可空的值类型，返回默认值
                    return Activator.CreateInstance(targetType);
                }
                return null;
            }

            // 如果类型已经匹配，直接返回
            if (targetType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            try
            {
                // 处理可空类型
                Type underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    if (string.IsNullOrEmpty(value.ToString()))
                        return null;

                    // 递归调用转换基础类型
                    return ConvertToType(value, underlyingType);
                }

                // 处理字符串类型
                if (targetType == typeof(string))
                {
                    return value.ToString();
                }

                // 处理枚举类型
                if (targetType.IsEnum)
                {
                    if (value is string stringValue)
                    {
                        return Enum.Parse(targetType, stringValue, true);
                    }
                    return Enum.ToObject(targetType, value);
                }

                // 处理布尔类型
                if (targetType == typeof(bool))
                {
                    if (value is string stringValue)
                    {
                        if (bool.TryParse(stringValue, out bool result))
                            return result;

                        // 处理常见的布尔值表示
                        switch (stringValue.ToLowerInvariant())
                        {
                            case "1":
                            case "true":
                            case "yes":
                            case "on":
                                return true;
                            case "0":
                            case "false":
                            case "no":
                            case "off":
                                return false;
                            default:
                                return false;
                        }
                    }
                    return Convert.ToBoolean(value);
                }

                // 处理数值类型
                if (targetType == typeof(int))
                {
                    if (value is string stringValue && int.TryParse(stringValue, out int result))
                        return result;
                    return Convert.ToInt32(value);
                }

                if (targetType == typeof(long))
                {
                    if (value is string stringValue && long.TryParse(stringValue, out long result))
                        return result;
                    return Convert.ToInt64(value);
                }

                if (targetType == typeof(double))
                {
                    if (value is string stringValue && double.TryParse(stringValue, out double result))
                        return result;
                    return Convert.ToDouble(value);
                }

                if (targetType == typeof(float))
                {
                    if (value is string stringValue && float.TryParse(stringValue, out float result))
                        return result;
                    return Convert.ToSingle(value);
                }

                if (targetType == typeof(decimal))
                {
                    if (value is string stringValue && decimal.TryParse(stringValue, out decimal result))
                        return result;
                    return Convert.ToDecimal(value);
                }

                if (targetType == typeof(short))
                {
                    if (value is string stringValue && short.TryParse(stringValue, out short result))
                        return result;
                    return Convert.ToInt16(value);
                }

                if (targetType == typeof(byte))
                {
                    if (value is string stringValue && byte.TryParse(stringValue, out byte result))
                        return result;
                    return Convert.ToByte(value);
                }

                // 处理日期时间类型
                if (targetType == typeof(DateTime))
                {
                    if (value is string stringValue)
                    {
                        if (DateTime.TryParse(stringValue, out DateTime result))
                            return result;
                        throw new FormatException($"无法将字符串 '{stringValue}' 转换为 DateTime");
                    }
                    return Convert.ToDateTime(value);
                }

                if (targetType == typeof(DateTimeOffset))
                {
                    if (value is string stringValue)
                    {
                        if (DateTimeOffset.TryParse(stringValue, out DateTimeOffset result))
                            return result;
                        throw new FormatException($"无法将字符串 '{stringValue}' 转换为 DateTimeOffset");
                    }
                    return (DateTimeOffset)Convert.ChangeType(value, targetType);
                }

                if (targetType == typeof(TimeSpan))
                {
                    if (value is string stringValue)
                    {
                        if (TimeSpan.TryParse(stringValue, out TimeSpan result))
                            return result;
                        throw new FormatException($"无法将字符串 '{stringValue}' 转换为 TimeSpan");
                    }
                    return (TimeSpan)Convert.ChangeType(value, targetType);
                }

                // 处理Guid类型
                if (targetType == typeof(Guid))
                {
                    if (value is string stringValue)
                    {
                        if (Guid.TryParse(stringValue, out Guid result))
                            return result;
                        throw new FormatException($"无法将字符串 '{stringValue}' 转换为 Guid");
                    }
                    return Guid.Parse(value.ToString());
                }

                // 使用Convert.ChangeType进行通用转换
                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"无法将值 '{value}' (类型: {value.GetType()}) 转换为类型 '{targetType.Name}'", ex);
            }
        }

        /// <summary>
        /// 将对象转换为指定类型（泛型版本）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">要转换的对象</param>
        /// <returns>转换后的对象</returns>
        public static T ConvertToType<T>(object value)
        {
            if (value == null)
            {
                if (default(T) == null)
                    return default(T);
                else
                    throw new ArgumentNullException(nameof(value), "无法将null转换为不可空的值类型");
            }

            Type targetType = typeof(T);
            object convertedValue = ConvertToType(value, targetType);
            return (T)convertedValue;
        }

        /// <summary>
        /// 根据类型名称将对象转换为指定类型
        /// </summary>
        /// <param name="value">要转换的对象</param>
        /// <param name="typeName">目标类型名称</param>
        /// <returns>转换后的对象</returns>
        public static object ConvertToTypeName(object value, string typeName)
        {
            Type targetType = GetSystemType(typeName);
            return ConvertToType(value, targetType);
        }

        /// <summary>
        /// 根据类型名称将对象转换为指定类型（泛型版本）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">要转换的对象</param>
        /// <param name="typeName">目标类型名称</param>
        /// <returns>转换后的对象</returns>
        public static T ConvertToTypeName<T>(object value, string typeName)
        {
            object result = ConvertToTypeName(value, typeName);
            return (T)ConvertToType(result, typeof(T));
        }
    }
}
