using MessagePack;
using monogameMinecraftDX.Core;
namespace monogameMinecraftDX
{
    namespace World
    {
    [MessagePackObject]
    public class ChunkData
    {
        [Key(0)]
        public BlockData[,,] map;
        [Key(1)]
        public Vector2Int chunkPos = new Vector2Int(0, 0);

        public ChunkData(BlockData[,,] map, Vector2Int chunkPos)
        {
            this.map = map;
            this.chunkPos = chunkPos;
        }
        public ChunkData(Vector2Int chunkPos)
        {
            //  this.map = map;
            this.chunkPos = chunkPos;
        }
    }
    }
   
}
