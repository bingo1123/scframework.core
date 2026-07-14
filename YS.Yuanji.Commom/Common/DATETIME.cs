using System.Runtime.InteropServices;

namespace YS.Yuanji.Commom
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DATETIME
    {
        public byte Day;
        public byte Month;
        public ushort Year;
        public byte Hour;
        public byte Minute;
        public byte Second;
    }
}
