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
        UserDataUpdate = (byte)138,
        UserDataBroadcast = (byte)135,
        UserDataRequest= (byte)137,
        WorldData =(byte)131,
        ChunkDataRequest= (byte)132,//ChunkDataRequestData
        UserLoginReturn= (byte)136,
        WorldGenParamsRequest= (byte)139,
        WorldGenParamsData = (byte)140,
        BlockModifyData=(byte)141,
        ChunkUpdateData=(byte)142,
        EntityDataBroadcast=(byte)143
    }
}
