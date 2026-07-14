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
    /// Node Status List
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("节点状态列表")]
    [Obsolete]
    public struct SLVL
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        [Description("每个节点的状态信息")]
        public uint[] Nodes;    // 每个节点的状态信息

        [Description("设定值列表")]
        public ushort Configuration; //PROTOS1-8 Setpoint list 

        [Description("实际值列表")]
        public ushort LifeList; //PROTOS1-8 Actual user list
    }

    #region SLVL 节点状态列表
    //public class SLVLParser : ISymbEntityParser<SLVL>
    //{
    //    public SLVL Parse(byte[] response, HuaniProtocolTypeEnum typeEnum = HuaniProtocolTypeEnum.M5)
    //    {
    //        SLVL slvl = new SLVL();
    //        slvl.Nodes = new uint[256];

    //        for (int i = 0; i < 256; i++)
    //        {
    //            slvl.Nodes[i] = BitConverter.ToUInt32(response, i * 4);
    //        }

    //        return slvl;
    //    }
    //}
    #endregion

    // Setpoint and actual node list 
    // Bit code per node:
    /*
        #define SLVL_NodeSetpoint 0 // Bit = 0: Setpoint node connected
        #define SLVL_NodeConnected 1 // Bit = 1: Actual value node connected
        #define SLVL_ParInconsist 2 // Bit = 2: Node parameters inconsistent
        #define SLVL_DataTransfer 3 // Bit = 3: Data transfer permitted
        #define SLVL_OPCConnecting 4 // Bit = 4: OPC connection is being established
        #define SLVL_OPCConnected 5 // Bit = 5: Connected to the OPC server
        #define SLVL_NodeAvailable 6 // Bit = 6: Node configuration exists
        #define SLVL_ReadingNodeConfiguration 7 // Bit = 7: Node configuration is being read
    */
}
