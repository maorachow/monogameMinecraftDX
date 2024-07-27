using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace monogameMinecraftShared.World
{
    public enum StructureGeneratingType
    {
        Random = 0,
        FixedSpacing = 1
    }

    [MessagePackObject]
    public struct StructureGeneratingParam
    {
        [Key(0)]
        public StructureGeneratingType type;
        [Key(1)]
        public float additionalParam1;
        [Key(2)]
        public float additionalParam2;

        [Key(3)]
        public short[] ignorePlacingBlockTypes;

        public StructureGeneratingParam(StructureGeneratingType type, float additionalParam1, float additionalParam2, short[] ignorePlacingBlockTypes)
        {
            this.type = type;
            this.additionalParam1 = additionalParam1;
            this.additionalParam2 = additionalParam2;
            this.ignorePlacingBlockTypes = ignorePlacingBlockTypes;
        }
    }
}
