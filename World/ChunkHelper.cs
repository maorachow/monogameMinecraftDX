using Microsoft.Xna.Framework;
using System;
using monogameMinecraftDX.Core;
using monogameMinecraftDX.Utility;
using monogameMinecraftDX.Physics;
namespace monogameMinecraftDX
{
    namespace World
    {
        public class ChunkHelper
    {
        //   public static Dictionary<Vector2Int, ChunkData> chunkDataReadFromDisk = new Dictionary<Vector2Int, ChunkData>();
        //    public static ConcurrentDictionary<Vector2Int, Chunk> chunks = new ConcurrentDictionary<Vector2Int, Chunk>();
        //  public static int buildingChunksCount = 0;
        //  public static object updateWorldThreadLock=new object();
        //  public static object deleteChunkThreadLock = new object();
        public static Chunk GetChunk(Vector2Int pos) => VoxelWorld.currentWorld.GetChunk(pos);
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
        public static bool isJsonReadFromDisk { get; set; }
        public static bool isWorldDataSaved { get; private set; }
        public static string gameWorldDataPath = AppDomain.CurrentDomain.BaseDirectory;


        /*     public static void SaveWorldData()
             {

                 FileStream fs;
                 if (File.Exists(gameWorldDataPath + "unityMinecraftServerData/GameData/world.json"))
                 {
                     fs = new FileStream(gameWorldDataPath + "unityMinecraftServerData/GameData/world.json", FileMode.Truncate, FileAccess.Write);//Truncate模式打开文件可以清空。
                 }
                 else
                 {
                     fs = new FileStream(gameWorldDataPath + "unityMinecraftServerData/GameData/world.json", FileMode.Create, FileAccess.Write);
                 }
                 fs.Close();
                 foreach (KeyValuePair<Vector2Int, Chunk> c in chunks)
                 {
                     // int[] worldDataMap=ThreeDMapToWorldData(c.Value.map);
                     //   int x=(int)c.Value.transform.position.x;
                     //  int z=(int)c.Value.transform.position.z;
                     //   WorldData wd=new WorldData();
                     //   wd.map=worldDataMap;
                     //   wd.posX=x;
                     //   wd.posZ=z;
                     //   string tmpData=JsonMapper.ToJson(wd);
                     //   File.AppendAllText(Application.dataPath+"/GameData/world.json",tmpData+"\n");
                     c.Value.SaveSingleChunk();
                 }

                 //    foreach (KeyValuePair<Vector2Int, ChunkData> wd in chunkDataReadFromDisk)
                 //   {
                 //      string tmpData = JsonConvert.SerializeObject(wd.Value);

                 //    }
                 byte[] allWorldData = MessagePackSerializer.Serialize(chunkDataReadFromDisk);
                 File.WriteAllBytes(gameWorldDataPath + "unityMinecraftServerData/GameData/world.json", allWorldData);
                 isWorldDataSaved = true;
             }*/
        public static bool CheckIsPosInChunk(Vector3 pos, Chunk c)
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

        public static bool CheckIsPosInChunkBorder(Vector3 pos, Chunk c)
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

        /*   public static void UpdateWorldThread( GamePlayer player,MinecraftGame game)
           {
               BoundingFrustum frustum;
               while (true)
               {
                   lock (updateWorldThreadLock)
                   {
                       lock (deleteChunkThreadLock)
                       {
                   if (game.status == GameStatus.Quiting || game.status == GameStatus.Menu)
                   {
                       return;
                   }

                   Thread.Sleep(500);
                   if (chunks == null)
                   {
                       return;
                   }
          //         Debug.WriteLine("building chunks count:"+buildingChunksCount);
                   if (player.isChunkNeededUpdate == true)
                   {
                   //    Debug.WriteLine("update");

                    frustum = new BoundingFrustum(player.cam.viewMatrix * player.cam.projectionMatrix);
                   for(float x = player.position.X - GameOptions.renderDistance; x < player.position.X + GameOptions.renderDistance; x += Chunk.chunkWidth)
                   {
                       for (float z = player.position.Z - GameOptions.renderDistance; z < player.position.Z + GameOptions.renderDistance; z += Chunk.chunkWidth)
                       {
                            //   Thread.Sleep(1);
                               Vector2Int chunkPos = Vec3ToChunkPos(new Vector3(x, 0, z));

                           if (GetChunk(chunkPos) == null&& !chunks.ContainsKey(chunkPos)) {
                               BoundingBox chunkBoundingBox = new BoundingBox(new Vector3(chunkPos.x, 0, chunkPos.y), new Vector3(chunkPos.x + Chunk.chunkWidth, Chunk.chunkHeight, chunkPos.y + Chunk.chunkWidth));
                               if (frustum.Intersects(chunkBoundingBox))
                               {
                                   Chunk c = new Chunk(chunkPos,  game.GraphicsDevice);
                                  // goto endUpdateWorld;
                                   //    break;
                               }
                               else continue;


                           }
                           else continue;

                       }
                   }

              // endUpdateWorld:;

                   player.isChunkNeededUpdate = false;

                   }
                       }

                   }

               }
           }*/
        /*   public static void TryDeleteChunksThread( GamePlayer player,MinecraftGame game)
           {
               while (true)
               { 
                   Thread.Sleep(500);
                   lock (deleteChunkThreadLock)
                   {
                   if (game.status == GameStatus.Quiting || game.status == GameStatus.Menu)
                       {
                           return;
                       }

                       if (ChunkRenderer.isBusy == true)
                       {
                           continue;
                       }
                       if (chunks == null)
                       {
                           return;
                       }
                       foreach (Chunk c in ChunkManager.chunks.Values)
                       {

                           if ((MathF.Abs(c.chunkPos.x - player.position.X) > (GameOptions.renderDistance + Chunk.chunkWidth) || MathF.Abs(c.chunkPos.y - player.position.Z) > (GameOptions.renderDistance + Chunk.chunkWidth))
                               && (c.isReadyToRender == true && c.isTaskCompleted == true) && c.usedByOthersCount <= 0
                               /*    && (c.Value.leftChunk==null||(c.Value.leftChunk!=null&&c.Value.leftChunk.isTaskCompleted == true))
                                   && (c.Value.rightChunk == null || (c.Value.rightChunk != null && c.Value.rightChunk.isTaskCompleted == true))
                                   && (c.Value.frontChunk == null || (c.Value.frontChunk != null && c.Value.frontChunk.isTaskCompleted == true))
                                   && (c.Value.backChunk == null || (c.Value.backChunk != null && c.Value.backChunk.isTaskCompleted == true))
                               )
                           {
                               // Chunk c2;

                               c.isReadyToRender = false;
                               c.SaveSingleChunk();

                               c.isUnused = true;

                               //     ChunkManager.chunks.TryRemove(new KeyValuePair<Vector2Int,Chunk>(c.chunkPos,c));
                               //  c.Dispose();


                           }



                       }


                       foreach (Chunk c in ChunkManager.chunks.Values)
                       {
                         //  c.semaphore.Wait();
                           if (c.isUnused == true)
                           {
                               c.unusedSeconds += 0.5f;

                               if (c.unusedSeconds > 5f)
                               {

                                   c.Dispose();
                                   ChunkManager.chunks.TryRemove(new KeyValuePair<Vector2Int, Chunk>(c.chunkPos, c));

                               }

                               //     ChunkManager.chunks.TryRemove(new KeyValuePair<Vector2Int,Chunk>(c.chunkPos,c));
                               //  c.Dispose();


                           }

                        //   c.semaphore.Release();

                       }
                   }



               }
           }*/
        public static void RebuildAllChunks()
        {
            lock (VoxelWorld.currentWorld.updateWorldThreadLock)
            {
                lock (VoxelWorld.currentWorld.deleteChunkThreadLock)
                {
                    foreach (Chunk c in VoxelWorld.currentWorld.chunks.Values)
                    {
                        //  c.semaphore.Wait();
                        if (c.disposed == false && c.isReadyToRender == true)
                        {
                            c.BuildChunk();
                        }


                        //   c.semaphore.Release();

                    }
                }
            }
            GC.Collect();

        }
        public static short GetBlock(Vector3 pos)
        {
            Vector3Int intPos = Vector3Int.FloorToIntVec3(pos);
            Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(pos));

            if (chunkNeededUpdate == null || chunkNeededUpdate.isMapGenCompleted == false || chunkNeededUpdate.isUnused == true)
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
            public static short GetBlock(Vector3Int pos)
            {
                
                Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x,pos.y,pos.z)));

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
            public static BlockData GetBlockData(Vector3 pos)
        {
            Vector3Int intPos = Vector3Int.FloorToIntVec3(pos);
            Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(pos));

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
            public static BlockData GetBlockData(Vector3Int pos)
            {
               
                Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));

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
            public static short RaycastFirstBlockID(Physics.Ray ray, float distance)
        {
            for (float i = 0; i < distance; i += 0.1f)
            {
                Vector3 blockPoint = ray.origin + ray.direction * i;
                short blockID = (short)ChunkHelper.GetBlock(blockPoint);
                if (blockID != 0)
                {
                    return blockID;
                }
            }
            return 0;
        }
        public static Vector3 RaycastFirstPosition(Physics.Ray ray, float distance)
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
        }
        public void BreakBlockAtPoint(Vector3 blockPoint)
        {



            SetBlockWithUpdate(blockPoint, (short)0);

        }


        public static int GetChunkLandingPoint(float x, float z)
        {
            Vector2Int intPos = new Vector2Int((int)x, (int)z);
            Chunk locChunk = GetChunk(Vec3ToChunkPos(new Vector3(x, 0, z)));
            if (locChunk == null || locChunk.isMapGenCompleted == false)
            {

                return 100;
            }
            Vector2Int chunkSpacePos = intPos - locChunk.chunkPos;
            chunkSpacePos.x = MathHelper.Clamp(chunkSpacePos.x, 0, Chunk.chunkWidth - 1);
            chunkSpacePos.y = MathHelper.Clamp(chunkSpacePos.y, 0, Chunk.chunkWidth - 1);
            for (int i = 200; i > 0; i--)
            {
                if (locChunk.map[chunkSpacePos.x, i - 1, chunkSpacePos.y] != 0)
                {
                    return i;
                }
            }

            return 100;

        }
        public static void SetBlockWithUpdate(Vector3 pos, short blockID)
        {

            Vector3Int intPos = new Vector3Int(ChunkHelper.FloatToInt(pos.X), ChunkHelper.FloatToInt(pos.Y), ChunkHelper.FloatToInt(pos.Z));
            Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(pos));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;
            chunkNeededUpdate.BuildChunk();
            chunkNeededUpdate.isModifiedInGame = true;
            if (Chunk.blockSoundInfo.ContainsKey((int)blockID))
            {
                SoundsUtility.PlaySound(MinecraftGame.gameposition, pos, Chunk.blockSoundInfo[+blockID], 20f);
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
            }


            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }


            public static void SetBlockWithUpdate(Vector3Int pos, short blockID)
            {

                Vector3Int intPos = pos;
                Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(new Vector3(pos.x, pos.y, pos.z)));
                if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
                {
                    return;
                }
                Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
                if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
                {
                    return;
                }
                chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockID;
                chunkNeededUpdate.BuildChunk();
                chunkNeededUpdate.isModifiedInGame = true;
                if (Chunk.blockSoundInfo.ContainsKey((int)blockID))
                {
                    SoundsUtility.PlaySound(MinecraftGame.gameposition, new Vector3(pos.x+0.5f, pos.y + 0.5f, pos.z + 0.5f), Chunk.blockSoundInfo[+blockID], 20f);
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
                }


                //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
                //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
            }
            public static void SetBlockWithUpdate(Vector3 pos, BlockData blockData)
        {

            Vector3Int intPos = new Vector3Int(ChunkHelper.FloatToInt(pos.X), ChunkHelper.FloatToInt(pos.Y), ChunkHelper.FloatToInt(pos.Z));
            Chunk chunkNeededUpdate = ChunkHelper.GetChunk(ChunkHelper.Vec3ToChunkPos(pos));
            if (chunkNeededUpdate == null || chunkNeededUpdate.isReadyToRender == false)
            {
                return;
            }
            Vector3Int chunkSpacePos = intPos - new Vector3Int(chunkNeededUpdate.chunkPos.x, 0, chunkNeededUpdate.chunkPos.y);
            if (chunkSpacePos.y < 0 || chunkSpacePos.y >= Chunk.chunkHeight)
            {
                return;
            }
            chunkNeededUpdate.map[chunkSpacePos.x, chunkSpacePos.y, chunkSpacePos.z] = blockData;
            chunkNeededUpdate.BuildChunk();
            chunkNeededUpdate.isModifiedInGame = true;
            if (Chunk.blockSoundInfo.ContainsKey((int)blockData.blockID))
            {
                SoundsUtility.PlaySound(MinecraftGame.gameposition, pos, Chunk.blockSoundInfo[blockData.blockID], 20f);
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
            }


            //   BlockModifyData b = new BlockModifyData(pos.X, pos.Y, pos.Z, blockID);
            //    Program.AppendMessage(null, new MessageProtocol(133, MessagePackSerializer.Serialize(b)));
        }
        public static void BreakBlock(Vector3 pos)
        {
            short blockID = GetBlock(pos);
            if (Chunk.blockSoundInfo.ContainsKey((int)blockID))
            {
                SoundsUtility.PlaySound(MinecraftGame.gameposition, pos, Chunk.blockSoundInfo[+blockID], 20f);
            }
            SetBlockWithUpdate(pos, (short)0);
        }
            public static void BreakBlock(Vector3Int pos)
            {
                short blockID = GetBlock(pos);
                if (Chunk.blockSoundInfo.ContainsKey((int)blockID))
                {
                    SoundsUtility.PlaySound(MinecraftGame.gameposition, new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z + 0.5f), Chunk.blockSoundInfo[+blockID], 20f);
                }
                SetBlockWithUpdate(pos, (short)0);
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
   
}
