using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public struct EntityData
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
        public int entityInWorldID;

        [Key(10)]
        public bool isEntityHurt;

        [Key(11)]
        public bool isEntityDying;

        [Key(12)]
        public byte[] optionalData;

        public EntityData(int typeid, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, string entityID, float entityHealth, int entityInWorldID,bool isEntityHurt,bool isEntityDying, byte[] optionalData)
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
            this.entityInWorldID = entityInWorldID;
            this.isEntityHurt=isEntityHurt;
            this.isEntityDying=isEntityDying;
            this.optionalData = optionalData;
        }
    }
    [MessagePackObject]
    public struct Float3Data
    {
        [Key(0)]
        public float x;
        [Key(1)]
        public float y;
        [Key(2)]
        public float z;

        public Float3Data(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static byte[] ToBytes(Float3Data data)
        {
            return MessagePackSerializer.Serialize(data);
        }

        public static Float3Data FromBytes(byte[] data)
        {
            try
            {
                Float3Data data1 = MessagePackSerializer.Deserialize<Float3Data>(data);
                //    Debug.WriteLine(data1.x+" "+data1.y + " " + data1.z + " " + data1.w);
                return data1;
            }
            catch
            {
                return new Float3Data(0, 0, 0);
            }

        }
    }
    [MessagePackObject]
    public struct Float4Data
    {
        [Key(0)]
        public float x;
        [Key(1)]
        public float y;
        [Key(2)]
        public float z;
        [Key(3)]
        public float w;
        public Float4Data(float x, float y, float z,float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w= w;
        }

        public static byte[] ToBytes(Float4Data data)
        {
            return MessagePackSerializer.Serialize(data);
        }

        public static Float4Data FromBytes(byte[] data)
        {
            try
            {
                Float4Data data1 = MessagePackSerializer.Deserialize<Float4Data>(data);
            //    Debug.WriteLine(data1.x+" "+data1.y + " " + data1.z + " " + data1.w);
                return data1;
            }
            catch
            {
                return new Float4Data(0, 0, 0, 1);
            }
         
        }
    }
}
