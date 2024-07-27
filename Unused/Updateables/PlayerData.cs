using MessagePack;
namespace monogameMinecraftDX.Updateables
{
    [MessagePackObject]
    public struct PlayerData
    {
        [Key(0)]
        public float posX;
        [Key(1)]
        public float posY;
        [Key(2)]
        public float posZ;
        [Key(3)]
        public short[] inventoryData;
        [Key(4)]
        public int playerInWorldID;
        public PlayerData(float posX, float posY, float posZ, short[] inventoryData, int playerInWorldID)
        {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
            this.inventoryData = inventoryData;
            this.playerInWorldID = playerInWorldID;
        }
    }
}
