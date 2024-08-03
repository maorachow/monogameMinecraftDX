using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public class HurtEntityRequestData
    {
        [Key(0)]
        public string entityID;

        [Key(1)]
        public float hurtValue;

        [Key(2)]
        public float sourcePosX;

        [Key(3)]
        public float sourcePosY;
        [Key(4)]
        public float sourcePosZ;
        public HurtEntityRequestData(string entityID, float hurtValue, float sourcePosX, float sourcePosY, float sourcePosZ)
        {
            this.entityID = entityID;
            this.hurtValue = hurtValue;
            this.sourcePosX = sourcePosX;
            this.sourcePosY = sourcePosY;
            this.sourcePosZ = sourcePosZ;
        }
    }
}
