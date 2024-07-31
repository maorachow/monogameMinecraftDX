using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Data
{
    public enum ChunkUpdateDataTypes:byte
    {
        PlaceBlockUpdate=(byte)0,
        BreakingBlockUpdate = (byte)1,
        DoorInteractingUpdate= (byte)2,

    }
    [MessagePackObject]
    public struct ChunkUpdateData
    {
        [Key(0)]
        public byte dataType;
        [Key(1)]
        public int posX;
        [Key(2)]
        public int posY;
        [Key(3)]
        public int posZ;
        [Key(4)]
        public BlockData optionalData1;
        [Key(5)]
        public BlockData optionalData2;

        [Key(6)]
        public int worldID;
        public ChunkUpdateData(byte dataType, int posX, int posY, int posZ, BlockData optionalData1,
            BlockData optionalData2,int worldID)
        {
            this.optionalData1=optionalData1;
            this.optionalData2=optionalData2;
            this.dataType=dataType;
            this.posX=posX;
            this.posY = posY;
            this.posZ=posZ;
            this.worldID=worldID;
        }

    }
}
