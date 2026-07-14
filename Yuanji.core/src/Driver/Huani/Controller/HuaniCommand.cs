using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text;
using Yuanji.core.src.Driver.Huani.Entity;
using YS.Yuanji.Log;
using YS.Yuanji.Commom;


namespace  Yuanji.core.src.Driver.Huani.Controller
{

    public class HuaniCommand
    {
        public static byte[] PackCommandBySymbArray(byte Node, byte Function, string[] symbAdrs, ProtocolTypeEnum protocolType)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0);
            cmd.Add(0);
            cmd.Add(17);
            cmd.AddRange(PackSymbAdr(Node, Function, symbAdrs, protocolType));
            byte[] res = VisuHelper.GetLength(cmd.Count);
            cmd[0] = res[0];
            cmd[1] = res[1];
            return cmd.ToArray();
        }

        public static (byte[], string[] symbnums) PackCommand(byte nodeAddress, byte functionAddress, VisuCommandNameEnum nameEnum, ProtocolTypeEnum typeEnum, string symbAdr = "", byte brand = 0, string brandName = "", int numberParam = 0, List<Pvalue> paramValues = null)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0);
            cmd.Add(0);
            cmd.Add((byte)nameEnum);
            string[] symbs = null;
            switch (nameEnum)
            {
                default:
                    {
                        string[] symbAdrs;
                        if (symbAdr.Contains(';'))
                        {
                            symbAdrs = symbAdr.Split(';');
                            symbs = new string[symbAdrs.Length];
                            Array.Copy(symbAdrs, 0, symbs, 0, symbAdrs.Length);
                        }
                        else
                        {
                            symbAdrs = new string[1] { symbAdr };
                            symbs = new string[1] { symbAdr };
                        }
                        cmd.AddRange(PackSymbAdr(nodeAddress, functionAddress, symbAdrs, typeEnum));
                        break;
                    }
                case VisuCommandNameEnum.ReadBrandParameter:
                    cmd.AddRange(PackBrandParam(brandName, typeEnum, numberParam));
                    break;
                case VisuCommandNameEnum.ReadMachineParameter:
                    cmd.AddRange(PackMachineParam(nodeAddress, functionAddress, numberParam, typeEnum));
                    break;
                case VisuCommandNameEnum.ReadParameter:
                    cmd.AddRange(PackMachineParam(nodeAddress, functionAddress, numberParam, typeEnum));
                    break;
                case VisuCommandNameEnum.ReadMessageParameter:
                    cmd.AddRange(PackMachineParam(nodeAddress, functionAddress, numberParam, typeEnum));
                    break;
                case VisuCommandNameEnum.WriteMachineParameter:
                    cmd.AddRange(PackMachineParamValue(paramValues, typeEnum));
                    break;
                case VisuCommandNameEnum.WriteBrandParameter:
                    cmd.AddRange(PackBrandParamValue(brand, brandName, typeEnum, numberParam, paramValues));
                    break;
            }
            byte[] res = VisuHelper.GetLength(cmd.Count);
            cmd[0] = res[0];
            cmd[1] = res[1];
            return (cmd.ToArray(), symbnums: symbs);
        }

        private static byte[] PackSymbAdr(byte nodeAddress, byte functionAddress, string[] symbAdr, ProtocolTypeEnum protocolType)
        {
            List<byte> command = new List<byte>();
            foreach (string symb in symbAdr)
            {
                if (protocolType == ProtocolTypeEnum.M5 || protocolType == ProtocolTypeEnum.BOBME)
                {
                    command.Add(nodeAddress);
                    command.Add(functionAddress);
                    command.AddRange(VisuHelper.SymbAddressConvert(symb));
                }
                else
                {
                    command.Add(functionAddress);
                    command.AddRange(VisuHelper.SymbAddressConvertForP18(symb));
                }
                command.Add(0);
                command.Add(0);
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
            }
            return command.ToArray();
        }

        public static byte[] PackCommandBySymbArrayTemp(byte Node, byte Function, string[] symbAdrs, ProtocolTypeEnum protocolType, byte leng = 29)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0);
            cmd.Add(0);
            cmd.Add(17);
            cmd.AddRange(PackSymbAdrTemp(Node, Function, symbAdrs, protocolType, leng));
            byte[] res = VisuHelper.GetLength(cmd.Count);
            cmd[0] = res[0];
            cmd[1] = res[1];
            return cmd.ToArray();
        }

        private static byte[] PackSymbAdrTemp(byte nodeAddress, byte functionAddress, string[] symbAdr, ProtocolTypeEnum protocolType, byte leng)
        {
            List<byte> command = new List<byte>();
            foreach (string symb in symbAdr)
            {
                if (protocolType == ProtocolTypeEnum.M5 || protocolType == ProtocolTypeEnum.BOBME)
                {
                    command.Add(nodeAddress);
                    command.Add(functionAddress);
                    command.AddRange(VisuHelper.SymbAddressConvert(symb));
                }
                else
                {
                    command.Add(functionAddress);
                    command.AddRange(VisuHelper.SymbAddressConvertForP18(symb));
                }
                command.Add(0);
                command.Add(0);
                command.Add(leng);
                command.Add(0);
            }
            return command.ToArray();
        }

        private static byte[] PackBrandParam(string brandName, ProtocolTypeEnum type, int numberPara = 0)
        {
            List<byte> command = new List<byte>();
            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
            {
                command.AddRange(VisuHelper.BrandNameConvert(brandName));
                if (numberPara == 0)
                {
                    command.Add(byte.MaxValue);
                    command.Add(byte.MaxValue);
                    command.Add(byte.MaxValue);
                    command.Add(byte.MaxValue);
                }
                else
                {
                    command.AddRange(BitConverter.GetBytes(numberPara));
                }
            }
            else
            {
                command.Add(Convert.ToByte((brandName == "") ? ((object)0) : brandName));
                if (numberPara == 0)
                {
                    command.Add(byte.MaxValue);
                    command.Add(byte.MaxValue);
                }
                else
                {
                    command.AddRange(BitConverter.GetBytes((short)numberPara));
                }
            }
            return command.ToArray();
        }

        private static byte[] PackMachineParam(byte nodeAddress, byte functionAddress, int numberParam, ProtocolTypeEnum type)
        {
            List<byte> command = new List<byte>();
            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
            {
                command.Add(nodeAddress);
                command.Add(functionAddress);
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
            }
            else
            {
                command.Add(functionAddress);
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
            }
            return command.ToArray();
        }

        private static byte[] PackBrandParamValue(byte brand, string brandName, ProtocolTypeEnum type, int numberPara = 0, List<Pvalue> pvalues = null)
        {
            List<byte> command = new List<byte>();
            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME || type == ProtocolTypeEnum.ZJ118)
            {
                command.Add(brand);
                command.AddRange(VisuHelper.BrandNameConvert(brandName));
                command.AddRange(GenerateResponseFromPvalues(pvalues, type));
            }
            else
            {
                command.Add(Convert.ToByte((brandName == "") ? ((object)0) : brandName));
                if (numberPara == 0)
                {
                    command.Add(byte.MaxValue);
                    command.Add(byte.MaxValue);
                }
                else
                {
                    command.AddRange(BitConverter.GetBytes((short)numberPara));
                }
            }
            return command.ToArray();
        }

        private static byte[] PackMachineParamValue(List<Pvalue> list, ProtocolTypeEnum type)
        {
            List<byte> command = new List<byte>();
            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
            {
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
                command.Add(byte.MaxValue);
            }
            else
            {
                command.AddRange(GenerateResponseFromPvalues(list, type));
            }
            return command.ToArray();
        }

        public static byte[] PackFxsZB48ProductionDataCmd(FockeTypeEnum FockeType)
        {
            return PackFxsZB48Command(FockeType, FxsZB48CommandEnum.ProductionData);
        }

        public static byte[] PackFxsZB48EfficiencyCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.CalculationOfEfficiency);
        }

        public static byte[] PackFxsZB48MachineStatusCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.CurrentMachineStatus);
        }

        public static byte[] PackFxsZB48MachineParamCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.MachineParameter);
        }

        public static byte[] PackFxsZB48TimesCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.Times);
        }

        public static byte[] PackFxsZB48SpeedCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.MaxSpeedAndCurrentSpeed);
        }

        public static byte[] PackFxsZB48SpeedTimeCostCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.SpeedTimeCost);
        }

        public static byte[] PackFxsZB48WarningInfosCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.WarningInfos);
        }

        public static byte[] PackFxsZB48RejectInfosCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.RejectInfos);
        }

        public static byte[] PackFxsZB48BasicInfoCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.BasicInfo);
        }

        public static byte[] PackFxsZB48PresetInfosCmd(FockeTypeEnum fockeType)
        {
            return PackFxsZB48Command(fockeType, FxsZB48CommandEnum.PresetInfos);
        }

        public static byte[] PackFxsZB48Command(FockeTypeEnum FockeType, FxsZB48CommandEnum commandType)
        {
            string commandTemplete = $"GetData {FockeType}_1d!{FxsZB48Command.GetCommandDataType(commandType)}";
            return Encoding.ASCII.GetBytes(commandTemplete);
        }

        public static byte[] PackFxsZB48Command(FockeTypeEnum FockeType, string commandType)
        {
            string commandTemplete = $"GetData {FockeType}_1d!{commandType}";
            return Encoding.ASCII.GetBytes(commandTemplete);
        }

        public static string[] ParseFxsZB48Response(byte[] response, [CallerMemberName] string? propertyName = null)
        {
            LogController.Instance.DebugfLog(propertyName + " 收到数据<=" + Encoding.ASCII.GetString(response));
            string str = Encoding.ASCII.GetString(response).Replace("\r", "").Replace("\n", "")
                .TrimStart()
                .TrimEnd()
                .Trim();
            return (from s in str.Split('\t')
                    where !string.IsNullOrWhiteSpace(s) && !s.Equals("\t")
                    select s).ToArray();
        }

        public static List<(int, string, int)> ParseTripleDataFxsZB48Response(byte[] response, [CallerMemberName] string? propertyName = null)
        {
            List<(int, string, int)> tripleValue = new List<(int, string, int)>();
            try
            {
                LogController.Instance.DebugfLog(propertyName + " 收到数据<=" + Encoding.ASCII.GetString(response));
                string str = Encoding.ASCII.GetString(response).Replace("\r", "").Replace("\n", "")
                    .TrimStart()
                    .TrimEnd()
                    .Trim();
                string[] strings = (from s in str.Split('\t')
                                    where !string.IsNullOrWhiteSpace(s) && !s.Equals("\t")
                                    select s).ToArray();
                int valueCount = strings.Length / 3;
                for (int i = 0; i < valueCount; i++)
                {
                    tripleValue.Add((Convert.ToInt32(strings[i * 3]), strings[i * 3 + 1], Convert.ToInt32(strings[i * 3 + 2])));
                }
            }
            catch (Exception value)
            {
                LogController.Instance.Error($"Caller:{propertyName} occur exception:{value}");
            }
            return tripleValue;
        }

        public static List<List<string>> ParseMutTripleDataFxsZB48Response(byte[] response, [CallerMemberName] string? propertyName = null)
        {
            List<List<string>> tripleValue = new List<List<string>>();
            try
            {
                LogController.Instance.DebugfLog(propertyName + " 收到数据<=" + Encoding.ASCII.GetString(response));
                var lines = Encoding.ASCII.GetString(response)
                    .Trim().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var ite = new List<string>();
                    var strings = (from s in line.Split('\t')
                                   where !string.IsNullOrWhiteSpace(s)
                                   select s.TrimEnd()).ToList();
                    tripleValue.Add(strings);
                }

            }
            catch (Exception value)
            {
                LogController.Instance.Error($"Caller:{propertyName} occur exception:{value}");
            }
            return tripleValue;
        }

        public static uint ParseM5ShiftDataUnit(byte[] response, string[] symbs = null)
        {
            int offset = 73;
            return CheckIfResponsIsValid(response, "", "ParseM5ShiftDataUnit") ? BitConverter.ToUInt32(response, offset) : 0u;
        }

        public static float ParseM5ShiftDataFloat(byte[] response, string[] symbs = null)
        {
            int offset = 73;
            return CheckIfResponsIsValid(response, "", "ParseM5ShiftDataFloat") ? BitConverter.ToSingle(response, offset) : 0f;
        }

        public static short ParseM5ShiftDataShort(byte[] response, string[] symbs = null)
        {
            int offset = 73;
            return (short)(CheckIfResponsIsValid(response, "", "ParseM5ShiftDataShort") ? BitConverter.ToInt16(response, offset) : 0);
        }

        public static object ParseResponse(byte[] response, ProtocolTypeEnum protocolType, string[] symbs = null)
        {
            if (!CheckIfResponsIsValid(response, "", "ParseResponse"))
            {
                return null;
            }
            switch (response[2])
            {
                case 17:
                    {
                        object[] res = new object[symbs.Length];
                        for (int i = 0; i < symbs.Length; i++)
                        {
                            Enum.TryParse<SymbAdrEnum>(symbs[i], out var symbAdr);
                            try
                            {
                                object[] array = res;
                                int num = i;
                                if (1 == 0)
                                {
                                }
                                object obj = symbAdr switch
                                {
                                    SymbAdrEnum.ProductionCounter => ProductionCounterParser.Parse(response),
                                    SymbAdrEnum.ACFA => ACFAParser.Parse(response, protocolType),
                                    SymbAdrEnum.ACDI => (protocolType == ProtocolTypeEnum.M5 || protocolType == ProtocolTypeEnum.BOBME) ? ((object)VisuHelper.ParseStruct<ACDI>(response, 73)) : ((object)VisuHelper.ParseStruct<ACDI_P18>(response, 12)),
                                    SymbAdrEnum.SANA => SANAParser.Parse(response, protocolType),
                                    SymbAdrEnum.SHIS => SHISParser.Parse(response, protocolType),
                                    SymbAdrEnum.OMSC => OMSCParser.Parse(response, protocolType),
                                    SymbAdrEnum.SHFT => VisuHelper.ParseStruct<SHFT>(response, 73),
                                    SymbAdrEnum.BRKT => BRKTParser.Parse(response, protocolType),
                                    SymbAdrEnum.PROC => PROCParser.Parse(response, protocolType),
                                    SymbAdrEnum.STAT => VisuHelper.ParseStruct<STAT>(response, 12),
                                    SymbAdrEnum.SP02 => SP02Parser.Parse(response, protocolType),
                                    SymbAdrEnum.WASP => VisuHelper.ParseStruct<WASP>(response, 12),
                                    SymbAdrEnum.SHFD => VisuHelper.ParseStruct<SHFD_P18>(response, 12),
                                    SymbAdrEnum.ACST => VisuHelper.ParseStruct<ACST>(response, 12),
                                    SymbAdrEnum.OPMT => VisuHelper.ParseStruct<OPMT>(response, 12),
                                    _ => null,
                                };
                                if (1 == 0)
                                {
                                }
                                array[num] = obj;
                            }
                            catch (Exception value)
                            {
                                LogController.Instance.Error($"in ParseSymbDataToEntity occur exception:{value}");
                            }
                        }
                        return res;
                    }
                case 33:
                    Console.WriteLine("处理读取品牌参数响应");
                    return null;
                default:
                    Console.WriteLine("未知的帧类型");
                    return null;
            }
        }

        public static bool CheckIfResponsIsValid(byte[] response, string symbName = "", [CallerMemberName] string? name = null)
        {
            if (response == null || response.Length < 3)
            {
                return false;
            }
            ushort frameLength = BitConverter.ToUInt16(response, 0);
            byte frameType = response[2];
            (bool, string) resCheck = CheckResponseIsSuccess(frameType);
            if (!resCheck.Item1)
            {
                LogController.Instance.Error($"caller:{name},symbName{symbName}, 收到响应帧error, message: {resCheck.Item2}");
                return false;
            }
            return true;
        }

        public static (bool, string) CheckResponseIsSuccess(byte responseCode)
        {
            string message = string.Empty;
            if (Enum.IsDefined(typeof(VisuCommandNameEnum), responseCode))
            {
                return (true, message);
            }
            switch (responseCode)
            {
                case 2:
                    message = "Invalid message type";
                    break;
                case 4:
                    message = "Parameter out of its value range";
                    break;
                case 6:
                    message = "Frame length faulty";
                    break;
                case 8:
                    message = "Command not executed";
                    break;
                case 10:
                    message = "Unknown fault";
                    break;
                case 12:
                    message = "Parameter is write protected";
                    break;
                case 16:
                    message = "No such device";
                    break;
                case 18:
                    message = "Device not available";
                    break;
                case 20:
                    message = "Name of symbolic address not found";
                    break;
                case 22:
                    message = "No such offset";
                    break;
                case 24:
                    message = "Entry too long";
                    break;
                case 32:
                    message = "Unknown brand number";
                    break;
                case 36:
                    message = "Unknown parameter number";
                    break;
                case 38:
                    message = "Unknown parameter format";
                    break;
                case 40:
                    message = "Parameter was rejected by the machine";
                    break;
            }
            return (false, message);
        }

        public static List<Pvalue> ParseBrandPvalue(byte[] response, ProtocolTypeEnum type)
        {
            if (!CheckIfResponsIsValid(response, "", "ParseBrandPvalue"))
            {
                return null;
            }
            int brandIndex = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 3 : 4);
            int MachineLength = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 40 : 40);
            byte[] value = new byte[response.Length - brandIndex - MachineLength];
            Array.Copy(response, brandIndex + MachineLength, value, 0, response.Length - brandIndex - MachineLength);
            return PasrePvalue(value, type, isBrand: true);
        }

        public static List<Pvalue> ParseDeviceParamerPvalue(byte[] response, ProtocolTypeEnum type)
        {
            if (!CheckIfResponsIsValid(response, "", "ParseDeviceParamerPvalue"))
            {
                return null;
            }
            int brandIndex = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 6 : 4);
            byte[] value = new byte[response.Length - brandIndex];
            Array.Copy(response, brandIndex, value, 0, response.Length - brandIndex);
            return PasrePvalue(value, type, isBrand: true);
        }

        public static List<Pvalue> ParseMachinePvalue(byte[] response, ProtocolTypeEnum type)
        {
            if (!CheckIfResponsIsValid(response, "", "ParseMachinePvalue"))
            {
                return null;
            }
            int brandIndex = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 3 : 4);
            int brandNameLength = 40;
            byte[] value = new byte[response.Length - brandIndex - brandNameLength];
            Array.Copy(response, brandIndex + brandNameLength, value, 0, response.Length - brandIndex - brandNameLength);
            return PasrePvalue(value, type, isBrand: true);
        }

        public static List<Pvalue> PasrePvalue(byte[] response, ProtocolTypeEnum type, bool isBrand = false, int firstIndex = 0)
        {
            if (!isBrand && !CheckIfResponsIsValid(response, "", "PasrePvalue"))
            {
                return null;
            }
            int index = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 5 : 4);
            if (isBrand)
            {
                index = firstIndex;
            }
            int frameHeaderLength = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 5 : 3);
            byte[] value = new byte[response.Length - index];
            Array.Copy(response, index, value, 0, response.Length - index);
            int remainBytes = value.Length;
            int offset = index;
            List<Pvalue> lise = new List<Pvalue>();
            try
            {
                while (remainBytes > frameHeaderLength)
                {
                    Pvalue pvalue = ((type != ProtocolTypeEnum.M5 && type != ProtocolTypeEnum.BOBME) ? new Pvalue
                    {
                        nodeAddress = response[offset],
                        functionNumber = response[offset + 1],
                        number = BitConverter.ToInt16(response, offset),
                        ParaNumerName = response[offset + 1].ToString("X2") + response[offset].ToString("X2"),
                        format = response[offset + 2]
                    } : new Pvalue
                    {
                        nodeAddress = response[offset],
                        functionNumber = response[offset + 1],
                        number = BitConverter.ToInt16(response, offset + 2),
                        ParaNumerName = $"{response[offset + 3].ToString("D3")}.{response[offset + 2].ToString("D3")}.{BitConverter.ToUInt16(new byte[2]{response[offset],response[offset + 1]}).ToString("D4")}",
                        format = response[offset + 4]
                    });
                    if (pvalue.format > 19)
                    {
                        break;
                    }
                    switch (pvalue.format)
                    {
                        case 0:
                            pvalue.ParameterValue = ((response[offset + frameHeaderLength] != 0) ? true : false);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueBool;
                            break;
                        case 1:
                            pvalue.ParameterValue = response[offset + frameHeaderLength];
                            pvalue.pvalueFormat = PvalueFormatEnum.valueByte;
                            break;
                        case 2:
                            pvalue.ParameterValue = Convert.ToSByte(response[offset + frameHeaderLength]);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueSByte;
                            break;
                        case 3:
                            pvalue.ParameterValue = BitConverter.ToUInt16(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueUInt16;
                            break;
                        case 4:
                            pvalue.ParameterValue = BitConverter.ToInt16(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueInt16;
                            break;
                        case 5:
                            pvalue.ParameterValue = BitConverter.ToUInt32(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueUInt32;
                            break;
                        case 8:
                            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                            {
                                pvalue.ParameterValue = $"{response[offset + frameHeaderLength + 3].ToString("D3")}.{response[offset + frameHeaderLength + 2].ToString("D3")}.{BitConverter.ToUInt16(new byte[2]{response[offset + frameHeaderLength],response[offset + frameHeaderLength + 1]}).ToString("D4")}";
                                pvalue.pvalueFormat = PvalueFormatEnum.valueInt32StringTID;
                            }
                            else
                            {
                                pvalue.ParameterValue = response[offset + frameHeaderLength + 1].ToString("X2") + response[offset + frameHeaderLength].ToString("X2");
                                pvalue.pvalueFormat = PvalueFormatEnum.valueInt16StringTID;
                            }
                            break;
                        case 6:
                            pvalue.ParameterValue = BitConverter.ToInt32(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueInt32;
                            break;
                        case 7:
                            pvalue.ParameterValue = BitConverter.ToSingle(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueFloat;
                            break;
                        case 9:
                            pvalue.pvalueFormat = PvalueFormatEnum.ValueButton;
                            break;
                        case 10:
                            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                            {
                                pvalue.ParameterValue = $"StartTime-EndTime:{response[offset + frameHeaderLength]}:{response[offset + frameHeaderLength + 1]}-{response[offset + frameHeaderLength + 2]}:{response[offset + frameHeaderLength + 3]}";
                                pvalue.pvalueFormat = PvalueFormatEnum.valueStartEndTime;
                            }
                            else
                            {
                                pvalue.ParameterValue = BitConverter.ToInt64(response, offset + frameHeaderLength);
                                pvalue.pvalueFormat = PvalueFormatEnum.value64StartEndTime;
                            }
                            break;
                        case 12:
                            pvalue.ParameterValue = $"{BitConverter.ToInt16(new byte[2]{response[offset + frameHeaderLength + 2],response[offset + frameHeaderLength + 3]}).ToString("D4")}-{response[offset + frameHeaderLength + 1].ToString("D2")}-{response[offset + frameHeaderLength].ToString("D2")}";
                            pvalue.pvalueFormat = PvalueFormatEnum.ValueDate;
                            break;
                        case 13:
                            pvalue.ParameterValue = $"{BitConverter.ToInt16(new byte[2]{response[offset + frameHeaderLength + 2],response[offset + frameHeaderLength + 3]}).ToString("D2")}:{response[offset + frameHeaderLength + 1].ToString("D2")}:{response[offset + frameHeaderLength].ToString("D2")}";
                            pvalue.pvalueFormat = PvalueFormatEnum.ValueTime;
                            break;
                        case 11:
                            {
                                int count = 0;
                                List<byte> strbytes = new List<byte>();
                                for (int i = offset + frameHeaderLength; i < response.Length; i++)
                                {
                                    count++;
                                    if (response[i] != 0)
                                    {
                                        strbytes.Add(response[i]);
                                        continue;
                                    }
                                    offset += count;
                                    offset += frameHeaderLength;
                                    remainBytes -= count;
                                    remainBytes -= frameHeaderLength;
                                    break;
                                }
                                pvalue.ParameterValue = Encoding.ASCII.GetString(strbytes.ToArray()).Replace("\0", "");
                                pvalue.pvalueFormat = PvalueFormatEnum.valueString;
                                break;
                            }
                    }
                    switch (pvalue.pvalueFormat)
                    {
                        case PvalueFormatEnum.valueBool:
                        case PvalueFormatEnum.valueByte:
                        case PvalueFormatEnum.valueSByte:
                            offset += frameHeaderLength + 1;
                            remainBytes -= frameHeaderLength + 1;
                            break;
                        case PvalueFormatEnum.valueUInt16:
                        case PvalueFormatEnum.valueInt16:
                        case PvalueFormatEnum.valueInt16StringTID:
                            offset += frameHeaderLength + 2;
                            remainBytes -= frameHeaderLength + 2;
                            break;
                        case PvalueFormatEnum.valueUInt32:
                        case PvalueFormatEnum.valueInt32:
                        case PvalueFormatEnum.valueFloat:
                        case PvalueFormatEnum.ValueDate:
                        case PvalueFormatEnum.ValueTime:
                        case PvalueFormatEnum.valueInt32StringTID:
                        case PvalueFormatEnum.valueStartEndTime:
                            offset += frameHeaderLength + 4;
                            remainBytes -= frameHeaderLength + 4;
                            break;
                        case PvalueFormatEnum.value64StartEndTime:
                            offset += frameHeaderLength + 8;
                            remainBytes -= frameHeaderLength + 8;
                            break;
                        default:
                            offset += frameHeaderLength;
                            remainBytes -= frameHeaderLength;
                            break;
                        case PvalueFormatEnum.valueString:
                            break;
                    }
                    lise.Add(pvalue);
                }
            }
            catch (Exception value2)
            {
                LogController.Instance.Error($"huaniCommand PasrePvalue occur exception,response:{BitConverter.ToString(response)}, ex:{value2}");
            }
            return lise;
        }

        public static List<Pvalue> PasreDevicePvalue(byte[] response, ProtocolTypeEnum type, bool isBrand = false, int firstIndex = 0)
        {
            if (!isBrand && !CheckIfResponsIsValid(response, "", "PasreDevicePvalue"))
            {
                return null;
            }
            int index = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 5 : 4);
            if (isBrand)
            {
                index = firstIndex;
            }
            int frameHeaderLength = ((type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME) ? 5 : 3);
            byte[] value = new byte[response.Length - index];
            Array.Copy(response, index, value, 0, response.Length - index);
            int remainBytes = value.Length;
            int offset = index;
            List<Pvalue> lise = new List<Pvalue>();
            try
            {
                while (remainBytes > frameHeaderLength)
                {
                    Pvalue pvalue = ((type != ProtocolTypeEnum.M5 && type != ProtocolTypeEnum.BOBME) ? new Pvalue
                    {
                        nodeAddress = response[offset],
                        functionNumber = response[offset + 1],
                        number = BitConverter.ToInt16(response, offset),
                        ParaNumerName = response[offset + 1].ToString("X2") + response[offset].ToString("X2"),
                        format = response[offset + 2]
                    } : new Pvalue
                    {
                        nodeAddress = response[offset + 3],
                        functionNumber = response[offset + 2],
                        number = BitConverter.ToInt16(response, offset),
                        ParaNumerName = $"{response[offset + 3].ToString("D3")}.{response[offset + 2].ToString("D3")}.{BitConverter.ToUInt16(new byte[2]{ response[offset],response[offset + 1]}).ToString("D4")}",
                        format = response[offset + 4]
                    });
                    switch (pvalue.format)
                    {
                        case 0:
                            pvalue.ParameterValue = ((response[offset + frameHeaderLength] != 0) ? true : false);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueBool;
                            break;
                        case 1:
                            pvalue.ParameterValue = response[offset + frameHeaderLength];
                            pvalue.pvalueFormat = PvalueFormatEnum.valueByte;
                            break;
                        case 2:
                            pvalue.ParameterValue = Convert.ToSByte(response[offset + frameHeaderLength]);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueSByte;
                            break;
                        case 3:
                            pvalue.ParameterValue = BitConverter.ToUInt16(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueUInt16;
                            break;
                        case 4:
                            pvalue.ParameterValue = BitConverter.ToInt16(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueInt16;
                            break;
                        case 5:
                            pvalue.ParameterValue = BitConverter.ToUInt32(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueUInt32;
                            break;
                        case 8:
                            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                            {
                                pvalue.ParameterValue = $"{response[offset + frameHeaderLength + 3].ToString("D3")}.{response[offset + frameHeaderLength + 2].ToString("D3")}.{BitConverter.ToUInt16(new byte[2]{response[offset + frameHeaderLength],response[offset + frameHeaderLength + 1]}).ToString("D4")}";
                                pvalue.pvalueFormat = PvalueFormatEnum.valueInt32StringTID;
                            }
                            else
                            {
                                pvalue.ParameterValue = response[offset + frameHeaderLength + 1].ToString("X2") + response[offset + frameHeaderLength].ToString("X2");
                                pvalue.pvalueFormat = PvalueFormatEnum.valueInt16StringTID;
                            }
                            break;
                        case 6:
                            pvalue.ParameterValue = BitConverter.ToInt32(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueInt32;
                            break;
                        case 7:
                            pvalue.ParameterValue = BitConverter.ToSingle(response, offset + frameHeaderLength);
                            pvalue.pvalueFormat = PvalueFormatEnum.valueFloat;
                            break;
                        case 10:
                            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                            {
                                pvalue.ParameterValue = $"StartTime-EndTime:{response[offset + frameHeaderLength]}:{response[offset + frameHeaderLength + 1]}-{response[offset + frameHeaderLength + 2]}:{response[offset + frameHeaderLength + 3]}";
                                pvalue.pvalueFormat = PvalueFormatEnum.valueStartEndTime;
                            }
                            else
                            {
                                pvalue.ParameterValue = BitConverter.ToInt64(response, offset + frameHeaderLength);
                                pvalue.pvalueFormat = PvalueFormatEnum.value64StartEndTime;
                            }
                            break;
                        case 12:
                            pvalue.ParameterValue = $"{BitConverter.ToInt16(new byte[2]{response[offset + frameHeaderLength + 2],response[offset + frameHeaderLength + 3]}).ToString("D4")}-{response[offset + frameHeaderLength + 1].ToString("D2")}-{response[offset + frameHeaderLength].ToString("D2")}";
                            pvalue.pvalueFormat = PvalueFormatEnum.ValueDate;
                            break;
                        case 13:
                            pvalue.ParameterValue = $"{BitConverter.ToInt16(new byte[2]{response[offset + frameHeaderLength + 2],response[offset + frameHeaderLength + 3]}).ToString("D2")}:{response[offset + frameHeaderLength + 1].ToString("D2")}:{response[offset + frameHeaderLength].ToString("D2")}";
                            pvalue.pvalueFormat = PvalueFormatEnum.ValueTime;
                            break;
                        case 11:
                            {
                                int count = 0;
                                List<byte> strbytes = new List<byte>();
                                for (int i = offset + frameHeaderLength; i < response.Length; i++)
                                {
                                    count++;
                                    if (response[i] != 0)
                                    {
                                        strbytes.Add(response[i]);
                                        continue;
                                    }
                                    offset += count;
                                    offset += frameHeaderLength;
                                    remainBytes -= count;
                                    remainBytes -= frameHeaderLength;
                                    break;
                                }
                                pvalue.ParameterValue = Encoding.ASCII.GetString(strbytes.ToArray()).Replace("\0", "");
                                pvalue.pvalueFormat = PvalueFormatEnum.valueString;
                                break;
                            }
                    }
                    switch (pvalue.pvalueFormat)
                    {
                        case PvalueFormatEnum.valueBool:
                        case PvalueFormatEnum.valueByte:
                        case PvalueFormatEnum.valueSByte:
                            offset += frameHeaderLength + 1;
                            remainBytes -= frameHeaderLength + 1;
                            break;
                        case PvalueFormatEnum.valueUInt16:
                        case PvalueFormatEnum.valueInt16:
                        case PvalueFormatEnum.valueInt16StringTID:
                            offset += frameHeaderLength + 2;
                            remainBytes -= frameHeaderLength + 2;
                            break;
                        case PvalueFormatEnum.valueUInt32:
                        case PvalueFormatEnum.valueInt32:
                        case PvalueFormatEnum.valueFloat:
                        case PvalueFormatEnum.ValueDate:
                        case PvalueFormatEnum.ValueTime:
                        case PvalueFormatEnum.valueInt32StringTID:
                        case PvalueFormatEnum.valueStartEndTime:
                            offset += frameHeaderLength + 4;
                            remainBytes -= frameHeaderLength + 4;
                            break;
                        case PvalueFormatEnum.value64StartEndTime:
                            offset += frameHeaderLength + 8;
                            remainBytes -= frameHeaderLength + 8;
                            break;
                    }
                    lise.Add(pvalue);
                }
            }
            catch (Exception value2)
            {
                LogController.Instance.Error($"huaniCommand PasrePvalue occur exception,response:{BitConverter.ToString(response)}, ex:{value2}");
            }
            return lise;
        }

        public static byte[] GenerateResponseFromPvalues(List<Pvalue> pvalues, ProtocolTypeEnum type, bool isBrand = false)
        {
            if (pvalues == null || pvalues.Count == 0)
            {
                return new byte[0];
            }
            List<byte> response = new List<byte>();
            if (isBrand)
            {
                if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                {
                    response.AddRange(new byte[5]);
                }
                else
                {
                    response.AddRange(new byte[4]);
                }
            }
            try
            {
                foreach (Pvalue pvalue in pvalues)
                {
                    if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                    {
                        response.Add(pvalue.nodeAddress);
                        response.Add(pvalue.functionNumber);
                        byte[] numberBytes = BitConverter.GetBytes(pvalue.number);
                        response.AddRange(numberBytes);
                        response.Add(pvalue.format);
                    }
                    else
                    {
                        response.Add(pvalue.nodeAddress);
                        response.Add(pvalue.functionNumber);
                        response.Add(pvalue.format);
                    }
                    switch (pvalue.format)
                    {
                        case 0:
                            response.Add((byte)(Convert.ToBoolean(pvalue.ParameterValue) ? 1 : 0));
                            break;
                        case 1:
                            response.Add(Convert.ToByte(pvalue.ParameterValue));
                            break;
                        case 2:
                            response.Add((byte)Convert.ToSByte(pvalue.ParameterValue));
                            break;
                        case 3:
                            response.AddRange(BitConverter.GetBytes(Convert.ToUInt16(pvalue.ParameterValue)));
                            break;
                        case 4:
                            response.AddRange(BitConverter.GetBytes(Convert.ToInt16(pvalue.ParameterValue)));
                            break;
                        case 5:
                            response.AddRange(BitConverter.GetBytes(Convert.ToUInt32(pvalue.ParameterValue)));
                            break;
                        case 6:
                            response.AddRange(BitConverter.GetBytes(Convert.ToInt32(pvalue.ParameterValue)));
                            break;
                        case 7:
                            response.AddRange(BitConverter.GetBytes(Convert.ToSingle(pvalue.ParameterValue)));
                            break;
                        case 8:
                            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                            {
                                string[] parts = ((string)pvalue.ParameterValue).Split('.');
                                if (parts.Length >= 3)
                                {
                                    response.Add(byte.Parse(parts[2].Substring(2, 2)));
                                    response.Add(byte.Parse(parts[2].Substring(0, 2)));
                                    response.Add(byte.Parse(parts[1]));
                                    response.Add(byte.Parse(parts[0]));
                                }
                            }
                            else
                            {
                                string tid = (string)pvalue.ParameterValue;
                                if (tid.Length >= 4)
                                {
                                    response.Add(Convert.ToByte(tid.Substring(2, 2), 16));
                                    response.Add(Convert.ToByte(tid.Substring(0, 2), 16));
                                }
                            }
                            break;
                        case 10:
                            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                            {
                                string[] times = ((string)pvalue.ParameterValue).Replace("StartTime-EndTime:", "").Split('-');
                                if (times.Length >= 2)
                                {
                                    string[] start = times[0].Split(':');
                                    string[] end = times[1].Split(':');
                                    if (start.Length >= 2 && end.Length >= 2)
                                    {
                                        response.Add(byte.Parse(start[0]));
                                        response.Add(byte.Parse(start[1]));
                                        response.Add(byte.Parse(end[0]));
                                        response.Add(byte.Parse(end[1]));
                                    }
                                }
                            }
                            else
                            {
                                response.AddRange(BitConverter.GetBytes((long)pvalue.ParameterValue));
                            }
                            break;
                        case 12:
                            {
                                string[] dateParts = ((string)pvalue.ParameterValue).Split('-');
                                if (dateParts.Length >= 3)
                                {
                                    response.Add(byte.Parse(dateParts[2]));
                                    response.Add(byte.Parse(dateParts[1]));
                                    byte[] yearBytes = BitConverter.GetBytes(short.Parse(dateParts[0]));
                                    response.AddRange(yearBytes);
                                }
                                break;
                            }
                        case 13:
                            {
                                string[] timeParts = ((string)pvalue.ParameterValue).Split(':');
                                if (timeParts.Length >= 3)
                                {
                                    response.Add(byte.Parse(timeParts[2]));
                                    response.Add(byte.Parse(timeParts[1]));
                                    byte[] hourBytes = BitConverter.GetBytes(short.Parse(timeParts[0]));
                                    response.AddRange(hourBytes);
                                }
                                break;
                            }
                        case 11:
                            {
                                byte[] stringBytes = Encoding.ASCII.GetBytes((string)pvalue.ParameterValue);
                                response.AddRange(stringBytes);
                                response.Add(0);
                                break;
                            }
                    }
                }
            }
            catch (Exception value)
            {
                LogController.Instance.Error($"GenerateResponseFromPvalues occur exception: {value}");
                return new byte[0];
            }
            return response.ToArray();
        }

        public static string ParseFxsZB48ResponseToJson(byte[] response, FxsZB48CommandEnum commandType)
        {
            string resStr = Encoding.GetEncoding("GBK").GetString(response);
            string[] strings = resStr.Split('\t');
            switch (commandType)
            {
                case FxsZB48CommandEnum.ProductionData:
                    {
                        if (strings.Length < 11)
                        {
                            return string.Empty;
                        }
                        FxsZB48ProductionData productionData = new FxsZB48ProductionData();
                        productionData.BrandName = strings[0];
                        productionData.BrandCode = strings[1];
                        productionData.ProductStartTime = strings[2];
                        productionData.ProductTime = strings[3];
                        productionData.MachineCycles = int.Parse(strings[4]);
                        productionData.GoodProducts = int.Parse(strings[5]);
                        productionData.WasteProducts = int.Parse(strings[6]);
                        productionData.CigFault = int.Parse(strings[7]);
                        productionData.FPIbypass = int.Parse(strings[8]);
                        productionData.BrandCount = int.Parse(strings[9]);
                        productionData.Dummy = int.Parse(strings[10]);
                        return JsonConvert.SerializeObject(productionData);
                    }
                case FxsZB48CommandEnum.CalculationOfEfficiency:
                case FxsZB48CommandEnum.MachineParameter:
                case FxsZB48CommandEnum.Times:
                case FxsZB48CommandEnum.MaxSpeedAndCurrentSpeed:
                    {
                        Dictionary<string, string> dicData = new Dictionary<string, string>();
                        for (int j = 0; j < strings.Length; j += 2)
                        {
                            dicData.Add(strings[j], strings[j + 1]);
                        }
                        return JsonConvert.SerializeObject(dicData);
                    }
                case FxsZB48CommandEnum.CurrentMachineStatus:
                case FxsZB48CommandEnum.WarningInfos:
                case FxsZB48CommandEnum.RejectInfos:
                    {
                        List<FxsZB48ListData> listDatas = new List<FxsZB48ListData>();
                        for (int i = 0; i < strings.Length; i += 4)
                        {
                            FxsZB48ListData data = new FxsZB48ListData();
                            data.Name = strings[i];
                            data.Number = int.Parse(strings[i + 1]);
                            data.Duration = strings[i + 2];
                            data.Frequency = int.Parse(strings[i + 3]);
                            listDatas.Add(data);
                        }
                        return JsonConvert.SerializeObject(listDatas);
                    }
                default:
                    return string.Empty;
            }
        }

        public static ZJ17ProductionData GetZJ17ProductionData(byte[] input, ProtocolTypeEnum type = ProtocolTypeEnum.ZJ117)
        {
            if(input == null || input.Length < 2)
            {
                return null;
            }

            string receivedData = Encoding.GetEncoding("GBK").GetString(input);
            LogController.Instance.DebugfLog("ZJ17:收到数据<= " + receivedData);
            ZJ17ProductionData parseData = ZJ17DataConverter.ParseData(receivedData);
            return parseData;
        }
    }
}
