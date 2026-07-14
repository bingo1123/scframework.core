using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.PLC.ModbusServer
{
    public class ModbusModel
    {
        public ushort Address { get; set; }
        public RegisterType RegisterType { get; set; }

        public DataType DataType { get; set; }

        public ushort Length { get; set; }

        public ByteOrder ByteOrder { get; set; }

        }


   public enum RegisterType
    {
        Coil =0,
        DiscreteInput =1,
        HoldingRegister=4,
        InputRegister =3
    }

    public enum DataType
    {
        Float,
        Byte,
        Int32,
        UInt32,
        Int16,
        UInt16,
        Double,
        Word,
        DWord // 新增DWORD类型
    }

    public enum ByteOrder { BigEndian, LittleEndian , BigEndianSwap , LittleEndianSwap }
}
