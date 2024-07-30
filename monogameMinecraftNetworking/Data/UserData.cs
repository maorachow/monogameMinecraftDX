using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public class UserData
    {
        [Key(0)]
        public float posX;
        [Key(1)]
        public float posY;
        [Key(2)]
        public float posZ;
        [Key(3)]
        public float rotX;
        [Key(4)]
        public float rotY;
        [Key(5)]
        public float rotZ;
        [Key(6)]
        public string userName;
        [Key(7)]
        public bool isAttacking;

        public UserData(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, string userName, bool isAttacking)
        {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.rotX = rotX;
            this.rotY = rotY;
            this.rotZ = rotZ;
            this.userName = userName;
            this.isAttacking = isAttacking;
        }
    }
}
