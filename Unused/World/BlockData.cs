﻿ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Reflection.Metadata.Ecma335;
using MessagePack;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using monogameMinecraftDX.World;
 
namespace monogameMinecraftDX
{

 
   
    namespace World
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
        Door=7

    }
        [MessagePackObject]
        public struct BlockData
        {
            [Key(0)]
            public short blockID;
            [Key(1)]
            public byte optionalDataValue;
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
   
   
   
    
}
