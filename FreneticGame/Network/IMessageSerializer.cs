using System;

namespace Frenetic.Network
{
    public interface IMessageSerializer
    {
        byte[] Serialize(Message msg);
        Message Deserialize(byte[] data);
    }
}
