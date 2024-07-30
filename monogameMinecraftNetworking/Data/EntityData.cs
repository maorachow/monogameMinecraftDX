using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public class EntityData
    {
        [Key(0)]
        public int typeid;
        [Key(1)]
        public float posX;
        [Key(2)]
        public float posY;
        [Key(3)]
        public float posZ;
        [Key(4)]
        public float rotX;
        [Key(5)]
        public float rotY;
        [Key(6)]
        public float rotZ;
        [Key(7)]
        public string entityID;
        [Key(8)]
        public float entityHealth;
        [Key(9)]
        public bool isEntityHurt;

        public EntityData(int typeid, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, string entityID, float entityHealth, bool isEntityHurt)
        {
            this.typeid = typeid;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotX = rotX;
            this.rotY = rotY;
            this.rotZ = rotZ;
            this.entityID = entityID;
            this.entityHealth = entityHealth;
            this.isEntityHurt = isEntityHurt;
        }
    }
}
