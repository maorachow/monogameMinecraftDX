﻿using monogameMinecraft;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MessagePack;
using System.IO;
using System.Diagnostics;
using SharpDX.MediaFoundation.DirectX;
namespace monogameMinecraftDX
{
    public class VoxelWorld
    {
        public int worldGenType = 0;
        public int worldID = 0;
        public string curWorldSaveName = "default.bin";
        public static List<VoxelWorld> voxelWorlds = new List<VoxelWorld>{ 
            new VoxelWorld("world.bin",0,0),
            new VoxelWorld("worldender.bin",2,1)};
        public static bool isWorldChanged;
        public static VoxelWorld currentWorld = voxelWorlds[0];
        public FastNoise noiseGenerator = new FastNoise();
        public FastNoise biomeNoiseGenerator = new FastNoise();
        public FastNoise frequentNoiseGenerator = new FastNoise();

        public ConcurrentDictionary<Vector2Int,Chunk> chunks=new ConcurrentDictionary<Vector2Int,Chunk>();  
        public Dictionary<Vector2Int,ChunkData> chunkDataReadFromDisk=new Dictionary<Vector2Int,ChunkData>();


        public object updateWorldThreadLock = new object();
        public object deleteChunkThreadLock = new object();
       
        public VoxelWorld(string curWorldSaveName,int worldGenType, int worldID)
        {
            this.worldGenType = worldGenType;
            this.worldID = worldID;
            this.curWorldSaveName = curWorldSaveName;
        }



        public Chunk GetChunk(Vector2Int chunkPos)
        {
            if (chunks == null)
            {
                return null;
            }
            if (chunks.ContainsKey(chunkPos))
            {
                Chunk tmp = chunks[chunkPos];
                return tmp;
            }
            else
            {
                return null;
            }

        }



        public void UpdateWorldThread(GamePlayer player, MinecraftGame game)
        {
            BoundingFrustum frustum;
            while (true)
            {
                lock (updateWorldThreadLock)
                {
                    lock (deleteChunkThreadLock)
                    {
                     
                        if (VoxelWorld.currentWorld.worldID != worldID)
                        {
                            Debug.WriteLine("world changed");
                            return;
                        }
                   //     Debug.WriteLine("update world ID:" + worldID);
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
                            for (float x = player.playerPos.X - GameOptions.renderDistance; x < player.playerPos.X + GameOptions.renderDistance; x += Chunk.chunkWidth)
                            {
                                for (float z = player.playerPos.Z - GameOptions.renderDistance; z < player.playerPos.Z + GameOptions.renderDistance; z += Chunk.chunkWidth)
                                {
                                    //   Thread.Sleep(1);
                                    Vector2Int chunkPos = ChunkManager.Vec3ToChunkPos(new Vector3(x, 0, z));

                                    if (GetChunk(chunkPos) == null && !chunks.ContainsKey(chunkPos))
                                    {
                                        BoundingBox chunkBoundingBox = new BoundingBox(new Vector3(chunkPos.x, 0, chunkPos.y), new Vector3(chunkPos.x + Chunk.chunkWidth, Chunk.chunkHeight, chunkPos.y + Chunk.chunkWidth));
                                        if (frustum.Intersects(chunkBoundingBox))
                                        {
                                            Chunk c = new Chunk(chunkPos, game.GraphicsDevice,this);
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
        }



        public void TryDeleteChunksThread(GamePlayer player, MinecraftGame game)
        {
            while (true)
            {
                Thread.Sleep(500);
                lock (deleteChunkThreadLock)
                {
                    if(VoxelWorld.currentWorld.worldID!=worldID)
                    {

                        Debug.WriteLine("world changed delchunk thread");
                        return;
                    }
                  //  Debug.WriteLine("delete chunks ID:" + worldID);
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
                    foreach (Chunk c in chunks.Values)
                    {

                        if ((MathF.Abs(c.chunkPos.x - player.playerPos.X) > (GameOptions.renderDistance + Chunk.chunkWidth) || MathF.Abs(c.chunkPos.y - player.playerPos.Z) > (GameOptions.renderDistance + Chunk.chunkWidth))
                            && (c.isReadyToRender == true && c.isTaskCompleted == true) && c.usedByOthersCount <= 0
                            /*    && (c.Value.leftChunk==null||(c.Value.leftChunk!=null&&c.Value.leftChunk.isTaskCompleted == true))
                                && (c.Value.rightChunk == null || (c.Value.rightChunk != null && c.Value.rightChunk.isTaskCompleted == true))
                                && (c.Value.frontChunk == null || (c.Value.frontChunk != null && c.Value.frontChunk.isTaskCompleted == true))
                                && (c.Value.backChunk == null || (c.Value.backChunk != null && c.Value.backChunk.isTaskCompleted == true))*/
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


                    foreach (Chunk c in chunks.Values)
                    {
                        //  c.semaphore.Wait();
                        if (c.isUnused == true)
                        {
                            c.unusedSeconds += 0.5f;

                            if (c.unusedSeconds > 5f)
                            {

                                c.Dispose();
                                chunks.TryRemove(new KeyValuePair<Vector2Int, Chunk>(c.chunkPos, c));

                            }

                            //     ChunkManager.chunks.TryRemove(new KeyValuePair<Vector2Int,Chunk>(c.chunkPos,c));
                            //  c.Dispose();


                        }

                        //   c.semaphore.Release();

                    }
                }



            }
        }

        public static string gameWorldDataPath = AppDomain.CurrentDomain.BaseDirectory;
        public bool isWorldDataSaved;
        public static bool isJsonReadFromDisk { get; set; }
        public void SaveWorldData()
        {
            Debug.WriteLine(curWorldSaveName);
            FileStream fs;
            if (File.Exists(gameWorldDataPath + "unityMinecraftServerData/GameData/"+curWorldSaveName))
            {
                fs = new FileStream(gameWorldDataPath + "unityMinecraftServerData/GameData/" + curWorldSaveName, FileMode.Truncate, FileAccess.Write);//Truncate模式打开文件可以清空。
            }
            else
            {
                fs = new FileStream(gameWorldDataPath + "unityMinecraftServerData/GameData/" + curWorldSaveName, FileMode.Create, FileAccess.Write);
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
            File.WriteAllBytes(gameWorldDataPath + "unityMinecraftServerData/GameData/" + curWorldSaveName, allWorldData);
            isWorldDataSaved = true;
        }
        public Thread updateWorldThread;
        public Thread tryRemoveChunksThread;
        public Action actionOnSwitchedWorld;
        public void InitWorld(MinecraftGame game)
        {
            Debug.WriteLine("current world ID:" + worldID);

            
            int playerInWorldID = 0;
            if (isWorldChanged == true)
            {
                playerInWorldID =GamePlayer.ReadPlayerData(game.gamePlayer,game,true);
            }
            else
            {
                playerInWorldID = GamePlayer.ReadPlayerData(game.gamePlayer, game);

                if (playerInWorldID != worldID)
                {
                    SwitchToWorldWithoutSaving(playerInWorldID,game);
                   // return;
                }
            }



            // InitChunkPool();

            chunks = new ConcurrentDictionary<Vector2Int, Chunk>();
           
            ReadJson();
           

            EntityManager.ReadEntityData();
            EntityBeh.SpawnEntityFromData(game);

            //     isGoingToQuitWorld = false;
            //       updateAllChunkLoadersThread = Task.Run(() => VoxelWorld.currentWorld.TryUpdateAllChunkLoadersThread());

            //   t2.Start();
            //       TryDeleteChunksThread = Task.Run(() => VoxelWorld.currentWorld.TryReleaseChunkThread());
            //   t3.Start();
            //    tryUpdateChunksThread = Task.Run(() => VoxelWorld.currentWorld.TryUpdateChunkThread());


            updateWorldThread = new Thread(() => UpdateWorldThread(game.gamePlayer, game));
            updateWorldThread.IsBackground = true;
            updateWorldThread.Start();
            tryRemoveChunksThread = new Thread(() => TryDeleteChunksThread(game.gamePlayer, game));
            tryRemoveChunksThread.IsBackground = true;
            tryRemoveChunksThread.Start();
            game.gamePlayer.curChunk= null; 

            if (actionOnSwitchedWorld != null)
            {
                actionOnSwitchedWorld();
                actionOnSwitchedWorld = null;
            }


        }
       
        public void DestroyAllChunks()
        {
           
            lock (updateWorldThreadLock)
            {
                foreach (var c in chunks)
                {

                    c.Value.Dispose();
                   Chunk _;
                    chunks.Remove<Vector2Int, Chunk>(c.Key,out _ );

                } 
                chunks.Clear();
            chunkDataReadFromDisk.Clear();
           
            }
           
        }


        public void ReadJson()
        {
            chunkDataReadFromDisk=new Dictionary<Vector2Int, ChunkData>();
            //   gameWorldDataPath = WorldManager.gameWorldDataPath;
            Debug.WriteLine("read:"+curWorldSaveName);
            if (!Directory.Exists(gameWorldDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(gameWorldDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(gameWorldDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(gameWorldDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(gameWorldDataPath + "unityMinecraftServerData" + "/GameData/"+curWorldSaveName))
            {
                FileStream fs = File.Create(gameWorldDataPath + "unityMinecraftServerData" + "/GameData/" + curWorldSaveName);
                fs.Close();
            }

            byte[] worldData = File.ReadAllBytes(gameWorldDataPath + "unityMinecraftServerData/GameData/" + curWorldSaveName);
            /*  List<ChunkData> tmpList = new List<ChunkData>();
              foreach (string s in worldData)
              {
                  ChunkData tmp = JsonConvert.DeserializeObject<ChunkData>(s);
                  tmpList.Add(tmp);
              }
              foreach (ChunkData w in tmpList)
              {
                  chunkDataReadFromDisk.Add(new Vector2Int(w.chunkPos.x, w.chunkPos.y), w);
              }*/
            if (worldData.Length > 0)
            {
                chunkDataReadFromDisk = MessagePackSerializer.Deserialize<Dictionary<Vector2Int, ChunkData>>(worldData);
            }

            isJsonReadFromDisk = true;
        }

        public static void SwitchToWorldWithoutSaving(int worldIndex,MinecraftGame game)
        {
            if (worldIndex >= voxelWorlds.Count)
            {
                Debug.WriteLine("invalid index");
                return;
            }
            isWorldChanged = true;
            currentWorld = voxelWorlds[worldIndex];
            currentWorld.InitWorld(game);
        }



        public void SaveAndQuitWorld(MinecraftGame game)
        {

            // PlayerMove player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
          GamePlayer.SavePlayerData(game.gamePlayer,false);

            EntityManager.SaveWorldEntityData();
      
            SaveWorldData();

            DestroyAllChunks();
            //     chunks.Clear();
            //    isGoingToQuitWorld = true;

        }
        public static void SwitchToWorld(int worldIndex,MinecraftGame game)
        {
            if (worldIndex >= voxelWorlds.Count)
            {
                Debug.WriteLine("invalid index");
                return;
            }

            isWorldChanged = true;
            currentWorld.SaveAndQuitWorld(game);

            currentWorld = voxelWorlds[worldIndex];
            currentWorld.InitWorld(game);
            //    currentWorld.InitWorld();
        }

    }
}