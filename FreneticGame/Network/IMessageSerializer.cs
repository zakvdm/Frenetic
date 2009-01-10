using System;

namespace Frenetic
{
    public interface IMessageSerializer
    {
        byte[] Serialize(Message msg);
        Message Deserialize(byte[] data);
    }
}
