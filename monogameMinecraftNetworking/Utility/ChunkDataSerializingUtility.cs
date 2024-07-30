using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.World;

namespace monogameMinecraftNetworking.Utility
{
    public class ChunkDataSerializingUtility
    {
        public static MessagePackSerializerOptions lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
        public static byte[] SerializeChunk(ServerSideChunk chunk)
        {
            if (chunk.map == null)
            {
                return null;
            }

            byte[] ret = MessagePackSerializer.Serialize(chunk.ChunkToChunkData());
            return ret;
        }
    }
}
