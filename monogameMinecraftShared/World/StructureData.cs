using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftShared.Core;

namespace monogameMinecraftShared.World
{
    [MessagePackObject]
    public struct StructureData : ICloneable
    {
        [Key(0)]
        public BlockData[,,] blockDatas;

        public StructureData(BlockData[,,] data)
        {
            blockDatas = data;
        }

        public StructureBoundingBox GetBoundingBox(Vector3Int origin)
        {
            return new StructureBoundingBox
            {
                min = origin,
                max = origin + new Vector3Int(blockDatas.GetLength(0), blockDatas.GetLength(1), blockDatas.GetLength(2))
            };
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [MessagePackObject]
    public struct GeneratingStructureData
    {
        [Key(0)]
        public BlockData[,,] data;

        [Key(1)]
        public StructureGeneratingParam genParams;

        public GeneratingStructureData(BlockData[,,] data, StructureGeneratingParam genParams)
        {
            this.data = data;
            this.genParams = genParams;
        }

    }


}
