using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftShared.Core;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public struct ChunkDataRequestData
    {
        [Key(0)]
        public Vector2Int chunkPos;

        [Key(1)]
        public int worldID;

        public ChunkDataRequestData(Vector2Int chunkPos, int worldID)
        {
            this.worldID=worldID;
            this.chunkPos = chunkPos;
        }
    }
}
