using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class MessageTypeAttribute : Attribute
    {
        public string MessageType { get; }

        public MessageTypeAttribute(string messageType)
        {
            MessageType = messageType;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class IsUsedTypeAttribute : Attribute
    {
        public bool IsUsed { get; }

        public IsUsedTypeAttribute(bool isUsed)
        {
            IsUsed = isUsed;
        }
    }
}
