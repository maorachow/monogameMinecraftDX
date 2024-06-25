using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace monogameMinecraftDX.World
{
    [MessagePackObject]
    public struct StructureData
    {
        [Key(0)]
        public BlockData[,,] blockDatas;

        public StructureData(BlockData[,,] data)
        {
            this.blockDatas = data;
        }

    }
    
}
