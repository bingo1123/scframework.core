using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Basic
{
    public class CommunicationBase : ICommunication
    {
        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public bool IsConnected()
        {
            throw new NotImplementedException();
        }

        public byte[] SendCommand(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
