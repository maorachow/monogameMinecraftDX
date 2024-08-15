using System.Collections.Generic;
using MessagePack;
using Microsoft.Xna.Framework;

namespace monogameMinecraftShared.World
{

 
   
    
    public enum BlockShape
    {
        Solid=0,
        CrossModel=1,
        Torch=2,
        Slabs=3,
        Stairs=4,
        Water=5,
        Fence=6,
        Door=7,
        WallAttachment=8,
        SolidTransparent=9

    }
        [MessagePackObject]
        public struct BlockData
        {
            [Key(0)]
            public short blockID;//2*byte
            [Key(1)]
            public byte optionalDataValue;//1*byte
            public BlockData(short blockID)
            {
                this.blockID = blockID;
                this.optionalDataValue = 0;
            }
            public BlockData(short blockID, byte optionalDataValue)
            {
                this.blockID = blockID;
                this.optionalDataValue = optionalDataValue;
            }
            public static explicit operator BlockData(short data) => new BlockData(data);
            public static implicit operator BlockData(int data) => new BlockData((short)data);
            public static implicit operator short(BlockData data) => data.blockID;

        }

        public struct BlockInfo
    {

        public List<Vector2> uvCorners;
        public List<Vector2> uvSizes;
        
        public BlockShape shape;
        public BlockInfo(List<Vector2> uvCorners, List<Vector2> uvSizes, BlockShape shape)
        {
            this.uvCorners = uvCorners;
            this.uvSizes = uvSizes;
            this.shape = shape;
        }
        public BlockInfo(List<Vector2> uvCorners, List<Vector2> uvSizes)
        {
            this.uvCorners = uvCorners;
            this.uvSizes = uvSizes;
            this.shape = BlockShape.Solid;
        }
    }
    }
   
   
   
    

