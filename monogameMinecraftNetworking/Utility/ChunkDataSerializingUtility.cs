using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.World;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Utility
{
    public class ChunkDataSerializingUtility
    {
        public static MessagePackSerializerOptions lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);
        public static byte[] SerializeChunk(ServerSideChunk chunk)
        {
            if (chunk.map == null)
            {
                return null;
            }

            byte[] ret = MessagePackSerializer.Serialize(chunk.ChunkToChunkData(),lz4Options);
            return ret;
        }
        public static ChunkData DeserializeChunk(byte[] chunkData)
        {
            if (chunkData == null)
            {
                return null;
            }

            ChunkData ret = MessagePackSerializer.Deserialize<ChunkData>(chunkData, lz4Options);
            return ret;
        }
    }
}
