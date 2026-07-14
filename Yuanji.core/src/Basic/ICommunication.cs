using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Yuanji.core.src.Basic
{
    public interface ICommunication
    {
        bool Connect();


        bool IsConnected();

        void Disconnect();

        byte[] SendCommand(byte[] data);

    }
}
