using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public class BlockModifyData
    {
        [Key(0)]
        public int x;
        [Key(1)]
        public int y;
        [Key(2)]
        public int z;
        [Key(3)]
        public BlockData convertType;
        [Key(4)]
        public BlockData originalType;
        public BlockModifyData(int x, int y, int z, BlockData convertType, BlockData originalType)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.convertType = convertType;
            this.originalType = originalType;
        }
    }
}
