using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Protocol
{
    public enum MessageCommandType:byte
    {
        UserLogin= (byte)129,
        UserLogout= (byte)130, 
        UserDataBroadcast = (byte)135,
        WorldData =(byte)131,
        ChunkDataRequest= (byte)132,//ChunkDataRequestData
        UserLoginReturn= (byte)136
    }
}
