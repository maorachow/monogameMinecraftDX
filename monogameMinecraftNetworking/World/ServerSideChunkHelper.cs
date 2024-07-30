using Microsoft.Xna.Framework;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Physics;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using monogameMinecraftShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace monogameMinecraftNetworking.World
{
    
    public partial class ServerSideChunkHelper
    {
        //   public static Dictionary<Vector2Int, ChunkData> chunkDataReadFromDisk = new Dictionary<Vector2Int, ChunkData>();
        //    public static ConcurrentDictionary<Vector2Int, Chunk> chunks = new ConcurrentDictionary<Vector2Int, Chunk>();
        //  public static int buildingChunksCount = 0;
        //  public static object updateWorldThreadLock=new object();
        //  public static object deleteChunkThreadLock = new object();
        public static ServerSideChunk GetChunk(Vector2Int pos,int worldID) =>ServerSideVoxelWorld.voxelWorlds[worldID]. GetChunk(pos);
        /*  {
              if (chunks == null)
              {
                  return null;
              }
              if (chunks.ContainsKey(pos))
              {
                  if (chunks[pos].isUnused == true)
                  {
                      return null;
                  }
              return chunks[pos];
              }
              else
              {
                  return null;
              }

          }*/
        public static Vector3Int Vec3ToBlockPos(Vector3 pos)
        {
            Vector3Int intPos = new Vector3Int(FloatToInt(pos.X), FloatToInt(pos.Y), FloatToInt(pos.Z));
            return intPos;
        }
        public static int FloatToInt(float f)
        {
            if (f >= 0)
            {
                return (int)f;
            }
            else
            {
                return (int)f - 1;
            }
        }
        public static int FloorFloat(float n)
        {
            int i = (int)n;
            return n >= i ? i : i - 1;
        }

        public static int CeilFloat(float n)
        {
            int i = (int)(n + 1);
            return n >= i ? i : i - 1;
        }
        public static Vector2Int Vec3ToChunkPos(Vector3 pos)
        {
            Vector3 tmp = pos;
            tmp.X = MathF.Floor(tmp.X / (float)Chunk.chunkWidth) * Chunk.chunkWidth;
            tmp.Z = MathF.Floor(tmp.Z / (float)Chunk.chunkWidth) * Chunk.chunkWidth;
            Vector2Int value = new Vector2Int((int)tmp.X, (int)tmp.Z);
            //  mainForm.LogOnTextbox(value.x+" "+value.y+"\n");
            return value;
        }
     

 
        public static bool CheckIsPosInChunk(Vector3 pos, ServerSideChunk c)
        {
            if (c == null)
            {
                return false;
            }

            Vector3 chunkSpacePos = pos - new Vector3(c.chunkPos.x, 0, c.chunkPos.y);
            if (chunkSpacePos.X >= 0 && chunkSpacePos.X < Chunk.chunkWidth && chunkSpacePos.Z >= 0 && chunkSpacePos.Z < Chunk.chunkWidth)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckIsPosInChunkBorder(Vector3 pos, ServerSideChunk c)
        {
            if (c == null)
            {
                return false;
            }

            Vector3 chunkSpacePos = pos - new Vector3(c.chunkPos.x, 0, c.chunkPos.y);
            if ((chunkSpacePos.X >= 0 && chunkSpacePos.X < 1) || (chunkSpacePos.X >= Chunk.chunkWidth - 1 && chunkSpacePos.X < Chunk.chunkWidth) || (chunkSpacePos.Z >= 0 && chunkSpacePos.Z < 1) || (chunkSpacePos.Z >= Chunk.chunkWidth - 1 && chunkSpacePos.Z < Chunk.chunkWidth))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
 
        public static short GetBlock(Vector3 pos,int worldID)
        {
            Vector3Int intPos = Vector3Int.FloorToIntVec3(pos);
            ServerSideChunk chunkNeededUpdate = GetChunk(Vec3ToChunkPos(pos), worldID);

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false||chunkNeededUpdate.isUnused==true)
            {
                return 1;
            }

            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < Chunk.chunkWidth && chunkSpacePos.y < Chunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < Chunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return 0;
            }



        }
        public static short GetBlock(Vector3Int pos, int worldID)
        {

            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)),worldID);

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
            {
                return 1;
            }

            Vector3Int chunkSpacePos = pos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < Chunk.chunkWidth && chunkSpacePos.y < Chunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < Chunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return 0;
            }



        }

        public static BlockShape? GetBlockShape(short typeID)
        {
            if (typeID == 0 || !Chunk.blockInfosNew.ContainsKey(typeID))
            {
                return null;
            }

            return Chunk.blockInfosNew[typeID].shape;
        }
        public static BlockData GetBlockData(Vector3 pos,int worldID)
        {
            Vector3Int intPos = Vector3Int.FloorToIntVec3(pos);
            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(pos), worldID);

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
            {
                return (BlockData)1;
            }

            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < Chunk.chunkWidth && chunkSpacePos.y < Chunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < Chunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return (BlockData)0;
            }



        }
        public static BlockData GetBlockData(Vector3Int pos, int worldID)
        {

            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)), worldID);

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
            {
                return (BlockData)1;
            }

            Vector3Int chunkSpacePos = pos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x >= 0 && chunkSpacePos.x < Chunk.chunkWidth && chunkSpacePos.y < Chunk.chunkHeight && chunkSpacePos.y >= 0 && chunkSpacePos.z >= 0 && chunkSpacePos.z < Chunk.chunkWidth)
            {
                return chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z];
            }
            else
            {
                return (BlockData)0;
            }



        }
      /*  public static short RaycastFirstBlockID(monogameMinecraftShared. Physics.Ray ray, float distance)
        {
            for (float i = 0; i < distance; i += 0.1f)
            {
                Vector3 blockPoint = ray.origin + ray.direction * i;
                short blockID = (short).GetBlock(blockPoint);
                if (blockID != 0)
                {
                    return blockID;
                }
            }
            return 0;
        }
        public static Vector3 RaycastFirstPosition(monogameMinecraftShared.Physics.Ray ray, float distance)
        {
            for (float i = 0; i < distance; i += 0.01f)
            {
                Vector3 blockPoint = ray.origin + ray.direction * i;
                short blockID = ChunkHelper.GetBlock(blockPoint);
                if (blockID != 0)
                {
                    return blockPoint;
                }
            }
            return new Vector3(1024, 0, 1024);
        }*/
        public void BreakBlockAtPoint(Vector3 blockPoint,int worldID)
        {



            SetBlockWithUpdate(blockPoint, (short)0,worldID);

        }


        public static int GetChunkLandingPoint(float x, float z,int worldID)
        {
            Vector2Int intPos = new Vector2Int((int)x, (int)z);
            ServerSideChunk locChunk = GetChunk(Vec3ToChunkPos(new Vector3(x, 0, z)),worldID);
            if (locChunk == null || locChunk.isMapGenCompleted == false)
            {

                return 100;
            }
            Vector2Int chunkSpacePos = intPos - locChunk.chunkPos;
            chunkSpacePos.x = MathHelper.Clamp(chunkSpacePos.x, 0, Chunk.chunkWidth - 1);
            chunkSpacePos.y = MathHelper.Clamp(chunkSpacePos.y, 0, Chunk.chunkWidth - 1);
            for (int i = Chunk.chunkHeight - 1; i > 0; i--)
            {
                if (locChunk.map[chunkSpacePos.x, i - 1, chunkSpacePos.y] != 0)
                {
                    return i;
                }
            }

            return 100;

        }
        public static int GetSingleChunkLandingPoint(ServerSideChunk c, int x, int z)
        {
            if (c == null)
            {
                return -1;
            }
            Vector2Int chunkSpacePos = new Vector2Int(x, z);
            chunkSpacePos.x = MathHelper.Clamp(chunkSpacePos.x, 0, Chunk.chunkWidth - 1);
            chunkSpacePos.y = MathHelper.Clamp(chunkSpacePos.y, 0, Chunk.chunkWidth - 1);
            for (int i = Chunk.chunkHeight - 1; i > 0; i--)
            {
                if (c.map[chunkSpacePos.x, i, chunkSpacePos.y] == 0)
                {
                    continue;
                }
                if (BlockBoundingBoxUtility.IsBlockWithBoundingBox(Chunk.blockInfosNew[c.map[chunkSpacePos.x, i, chunkSpacePos.y]].shape))
                {
                    return i + 1;
                }
            }

            return -1;

        }

        public static Vector2Int GetChunkSpacePos(Vector3 worldPos, ServerSideChunk c)
        {
            Vector2Int intPos = new Vector2Int(FloatToInt(worldPos.X), FloatToInt(worldPos.Z));

            Vector2Int chunkSpacePos = intPos - c.chunkPos;
            return chunkSpacePos;
        }
        public static int PredictChunkLandingPoint(float x, float y,int worldID)
        {
            if (ServerSideVoxelWorld.voxelWorlds[worldID].worldGenType == 0)
            {
                int biome = (int)(1f + ServerSideVoxelWorld.voxelWorlds[worldID].biomeNoiseGenerator.GetSimplex(x, y) * 3f);
                return (int)(Chunk.chunkSeaLevel + ServerSideVoxelWorld.voxelWorlds[worldID].noiseGenerator.GetSimplex(x, y) * 20f + biome * 25f);
            }
            else if (ServerSideVoxelWorld.voxelWorlds[worldID].worldGenType == 2)
            {
                for (int i = 200; i > 0; i--)
                {
                    if (ServerSideChunk.PredictBlockType3D((int)x, i, (int)y,worldID) > 0)
                    {
                        return i;
                    }
                }
                return -100;
            }
            else
            {
                return -100;
            }
        }
        public static void SetBlockWithUpdate(Vector3 pos, short blockID, int worldID)
        {

            Vector3Int intPos = new Vector3Int(ChunkHelper.FloatToInt(pos.X), ChunkHelper.FloatToInt(pos.Y), ChunkHelper.FloatToInt(pos.Z));
            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(pos), worldID);
            if (chunkNeededUpdate == null )
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;

            chunkNeededUpdate.isModifiedInGame = true;



        }


        public static void SetBlockWithUpdate(Vector3Int pos, short blockID, int worldID)
        {

            Vector3Int intPos = pos;
            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)),worldID);
            if (chunkNeededUpdate == null )
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;
          
            chunkNeededUpdate.isModifiedInGame = true;
            
        }

        public static void SetBlockWithoutUpdate(Vector3Int pos, short blockID, int worldID)
        {

            Vector3Int intPos = pos;
            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)),worldID);
            if (chunkNeededUpdate == null )
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;




            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }
        public static void SetBlockWithoutUpdate(Vector3Int pos, BlockData blockID, int worldID)
        {

            Vector3Int intPos = pos;
            ServerSideChunk chunkNeededUpdate =GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)),worldID);
            if (chunkNeededUpdate == null)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;




            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }
        public static void SetBlockWithoutUpdateWithSaving(Vector3Int pos, BlockData blockID, int worldID)
        {

            Vector3Int intPos = pos;
            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)),worldID);
            if (chunkNeededUpdate == null)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;


            chunkNeededUpdate.isModifiedInGame = true;

            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }

        public static void SendPlaceBlockOperation(Vector3Int position, BlockData data,int worldID)
        {
            ServerSideVoxelWorld.voxelWorlds[worldID].worldUpdater.queuedChunkUpdatePoints.Enqueue(new PlacingBlockOperation(position, ServerSideVoxelWorld.voxelWorlds[worldID].worldUpdater, data,worldID));
            Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(position.x, position.y, position.z)));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = position - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
          

        }

        public static void SendBreakBlockOperation(Vector3Int position,int worldID)
        {
            BlockData prevData = ChunkHelper.GetBlockData(position);
            ServerSideVoxelWorld.voxelWorlds[worldID].worldUpdater.queuedChunkUpdatePoints.Enqueue(new BreakBlockOperation(position, ServerSideVoxelWorld.voxelWorlds[worldID].worldUpdater, prevData, worldID));
            ServerSideVoxelWorld.voxelWorlds[worldID].worldUpdater.queuedChunkUpdatePoints.Enqueue(new PlacingBlockOperation(position, ServerSideVoxelWorld.voxelWorlds[worldID].worldUpdater, 0, worldID));




         /*   if (Chunk.blockSoundInfo.ContainsKey((int)prevData.blockID))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, new Vector3(position.x + 0.5f, position.y + 0.5f, position.z + 0.5f), Chunk.blockSoundInfo[prevData.blockID], 20f);
            }
            Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(position.x, position.y, position.z)));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = position - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.x == 0)
            {

                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x - Chunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunkAsync();

            }
            if (chunkSpacePos.x == Chunk.chunkWidth - 1)
            {
                // if (chunkNeededUpdate.rightChunk != null && chunkNeededUpdate.rightChunk.isMapGenCompleted == true)

                //  chunkNeededUpdate.rightChunk.BuildChunk();
                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x + Chunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunkAsync();
            }
            if (chunkSpacePos.z == 0)
            {
                //  if (chunkNeededUpdate.backChunk != null && chunkNeededUpdate.backChunk.isMapGenCompleted == true)
                //       {
                //         chunkNeededUpdate.backChunk.BuildChunk();
                //     }
                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y - Chunk.chunkWidth))?.BuildChunkAsync();
            }
            if (chunkSpacePos.z == Chunk.chunkWidth - 1)
            {
                //   if (chunkNeededUpdate.frontChunk != null && chunkNeededUpdate.frontChunk.isMapGenCompleted == true)
                //     {
                //      chunkNeededUpdate.frontChunk.BuildChunk();
                //     }

                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y + Chunk.chunkWidth))?.BuildChunkAsync();
            }*/
        }
        public static void SetBlockOptionalDataWithoutUpdate(Vector3Int pos, byte optionalData,int worldID)
        {

            Vector3Int intPos = pos;
            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)),worldID);
            if (chunkNeededUpdate == null )
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z].optionalDataValue = optionalData;




            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }
        public static void SetBlockWithUpdate(Vector3 pos, BlockData blockData, int worldID)
        {

            Vector3Int intPos = new Vector3Int(ChunkHelper.FloatToInt(pos.X), ChunkHelper.FloatToInt(pos.Y), ChunkHelper.FloatToInt(pos.Z));
            ServerSideChunk chunkNeededUpdate = GetChunk(ChunkHelper.Vec3ToChunkPos(pos),worldID);
            if (chunkNeededUpdate == null )
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockData;
        /*      chunkNeededUpdate.BuildChunk();
            chunkNeededUpdate.isModifiedInGame = true;
            if (Chunk.blockSoundInfo.ContainsKey((int)blockData.blockID))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, pos, Chunk.blockSoundInfo[blockData.blockID], 20f);
            }
          if (chunkSpacePos.x == 0)
            {

                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x - Chunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunk();

            }
            if (chunkSpacePos.x == Chunk.chunkWidth - 1)
            {
                // if (chunkNeededUpdate.rightChunk != null && chunkNeededUpdate.rightChunk.isMapGenCompleted == true)

                //  chunkNeededUpdate.rightChunk.BuildChunk();
                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x + Chunk.chunkWidth, chunkNeededUpdate.chunkPos.y))?.BuildChunk();
            }
            if (chunkSpacePos.z == 0)
            {
                //  if (chunkNeededUpdate.backChunk != null && chunkNeededUpdate.backChunk.isMapGenCompleted == true)
                //       {
                //         chunkNeededUpdate.backChunk.BuildChunk();
                //     }
                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y - Chunk.chunkWidth))?.BuildChunk();
            }
            if (chunkSpacePos.z == Chunk.chunkWidth - 1)
            {
                //   if (chunkNeededUpdate.frontChunk != null && chunkNeededUpdate.frontChunk.isMapGenCompleted == true)
                //     {
                //      chunkNeededUpdate.frontChunk.BuildChunk();
                //     }

                GetChunk(new Vector2Int(chunkNeededUpdate.chunkPos.x, chunkNeededUpdate.chunkPos.y + Chunk.chunkWidth))?.BuildChunk();
            }*/


            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }

        public static void BreakBlock(Vector3 pos,int worldID)
        {
            short blockID = GetBlock(pos, worldID);
        /*    if (Chunk.blockSoundInfo.ContainsKey((int)blockID))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, pos, Chunk.blockSoundInfo[+blockID], 20f);
            }*/
            SetBlockWithUpdate(pos, (short)0,worldID);
        }
        public static void BreakBlock(Vector3Int pos, int worldID)
        {
            short blockID = GetBlock(pos, worldID);
           /* if (Chunk.blockSoundInfo.ContainsKey((int)blockID))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z + 0.5f), Chunk.blockSoundInfo[+blockID], 20f);
            }*/
            SetBlockWithUpdate(pos, (short)0, worldID);
        }
        /*    public static void ReadJson()
            {
                chunkDataReadFromDisk.Clear();
                //   gameWorldDataPath = WorldManager.gameWorldDataPath;

                if (!Directory.Exists(gameWorldDataPath + "unityMinecraftServerData"))
                {
                    Directory.CreateDirectory(gameWorldDataPath + "unityMinecraftServerData");

                }
                if (!Directory.Exists(gameWorldDataPath + "unityMinecraftServerData/GameData"))
                {
                    Directory.CreateDirectory(gameWorldDataPath + "unityMinecraftServerData/GameData");
                }

                if (!File.Exists(gameWorldDataPath + "unityMinecraftServerData" + "/GameData/world.json"))
                {
                    FileStream fs = File.Create(gameWorldDataPath + "unityMinecraftServerData" + "/GameData/world.json");
                    fs.Close();
                }

                byte[] worldData = File.ReadAllBytes(gameWorldDataPath + "unityMinecraftServerData/GameData/world.json");
                  List<ChunkData> tmpList = new List<ChunkData>();
                  foreach (string s in worldData)
                  {
                      ChunkData tmp = JsonConvert.DeserializeObject<ChunkData>(s);
                      tmpList.Add(tmp);
                  }
                  foreach (ChunkData w in tmpList)
                  {
                      chunkDataReadFromDisk.Add(new Vector2Int(w.chunkPos.x, w.chunkPos.y), w);
                  }
                if (worldData.Length > 0)
                {
                    chunkDataReadFromDisk = MessagePackSerializer.Deserialize<Dictionary<Vector2Int, ChunkData>>(worldData);
                }

                isJsonReadFromDisk = true;
            }*/
    }
}
