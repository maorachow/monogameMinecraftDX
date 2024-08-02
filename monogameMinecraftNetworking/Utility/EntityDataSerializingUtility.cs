using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftShared.Updateables;
using EntityData = monogameMinecraftNetworking.Data.EntityData;
namespace monogameMinecraftNetworking.Utility
{
    public class EntityDataSerializingUtility
    {
        public static MessagePackSerializerOptions lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
        public static byte[] SerializeAllEntityDatas(List<EntityData> allEntityDatas)
        {
            byte[] returnVal = MessagePackSerializer.Serialize(allEntityDatas, lz4Options);
            return returnVal;
        }

        public static List<EntityData> DeserializeEntityDatas(byte[] entityDatas)
        {
           List<EntityData> returnVal = MessagePackSerializer.Deserialize<List<EntityData>>(entityDatas, lz4Options);
            return returnVal;
        }
    }
}
