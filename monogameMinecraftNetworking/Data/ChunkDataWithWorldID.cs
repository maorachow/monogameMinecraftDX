using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public class ChunkDataWithWorldID
    {
        [Key(0)]
        public int worldID;

        [Key(1)]
        public ChunkData chunkData;

        public ChunkDataWithWorldID(int worldID, ChunkData chunkData)
        {
            this.worldID= worldID;
            this.chunkData = chunkData;
        }
    }
}
