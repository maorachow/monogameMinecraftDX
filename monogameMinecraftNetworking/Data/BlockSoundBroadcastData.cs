using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public class BlockSoundBroadcastData
    {
        [Key(0)]
        public float posX;
        [Key(1)]
        public float posY;
        [Key(2)]
        public float posZ;

        [Key(3)]
        public short blockID;

        public BlockSoundBroadcastData(float posX, float posY, float posZ, short blockID)
        {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.blockID = blockID;
        }
    }
}
