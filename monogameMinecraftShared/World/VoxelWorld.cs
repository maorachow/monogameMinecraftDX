using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
 

namespace monogameMinecraftShared.World
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

        public ConcurrentDictionary<Vector2Int, Chunk> chunks = new ConcurrentDictionary<Vector2Int, Chunk>();

        public ConcurrentDictionary<Vector2Int, IRenderableChunkBuffers> _renderingChunks =
            new ConcurrentDictionary<Vector2Int, IRenderableChunkBuffers>();
        public ConcurrentDictionary<Vector2Int, IRenderableChunkBuffers> renderingChunks
        {
            get
            {
                foreach (var kvp in chunks)
                {
                    if (!_renderingChunks.ContainsKey(kvp.Key))
                    {
                        _renderingChunks.TryAdd(kvp.Key, (IRenderableChunkBuffers)kvp.Value);
                    }
                }
                foreach (var kvp in _renderingChunks)
                {
                    if (!chunks.ContainsKey(kvp.Key))
                    {
                        _renderingChunks.TryRemove(kvp.Key,out IRenderableChunkBuffers _);
                    }
                }

                return _renderingChunks;
            }
        }

        public Dictionary<Vector2Int, ChunkData> chunkDataReadFromDisk = new Dictionary<Vector2Int, ChunkData>();

        public List<GeneratingStructureData> worldStructures= new List<GeneratingStructureData>();

        public object updateWorldThreadLock = new object();
        public object deleteChunkThreadLock = new object();

        public WorldUpdater worldUpdater;
        public StructureOperationsManager structureOperationsManager;
        public VoxelWorld(string curWorldSaveName, int worldGenType, int worldID)
        {
            this.worldGenType = worldGenType;
            this.worldID = worldID;
            this.curWorldSaveName = curWorldSaveName;
            worldUpdater = new WorldUpdater(this);
            structureOperationsManager= new StructureOperationsManager(this);
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


        private PriorityQueue<Vector2Int, int> tempChunkUpdatingQueue=new PriorityQueue<Vector2Int, int>();

        public void UpdateWorldThread(IGamePlayer player, MinecraftGameBase game)
        {
            BoundingFrustum frustum;
            try
            {
                while (true)
                {
                    Thread.Sleep(500);
                    Debug.WriteLine("update world thread:" + isThreadsStopping);
                    if (VoxelWorld.currentWorld.worldID != worldID)
                    {
                        Debug.WriteLine("world changed");
                        return;
                    }
                       
                    //     Debug.WriteLine("update world ID:" + worldID);
                    if (isThreadsStopping == true)
                    {
                        Debug.WriteLine("quit updateworld thread");
                        return;
                    }


                    if (chunks == null)
                    {
                        return;
                    }

                    lock (updateWorldThreadLock)
                    {
                        lock (deleteChunkThreadLock)
                        {
                            //       Debug.WriteLine("update world thread running");
                        
                            //         Debug.WriteLine("building chunks count:"+buildingChunksCount);
                            tempChunkUpdatingQueue.Clear();
                             
                            if (player is GamePlayer player1&& player1.isChunkNeededUpdate == true)
                            {
                                //    Debug.WriteLine("update");

                                frustum = new BoundingFrustum(player.cam.viewMatrix * player.cam.projectionMatrix);
                                for (float x = player.position.X - GameOptions.renderDistance; x < player.position.X + GameOptions.renderDistance; x += Chunk.chunkWidth)
                                {
                                    for (float z = player.position.Z - GameOptions.renderDistance; z < player.position.Z + GameOptions.renderDistance; z += Chunk.chunkWidth)
                                    {
                                        //   Thread.Sleep(1);
                                        Vector2Int chunkPos = ChunkHelper.Vec3ToChunkPos(new Vector3(x, 0, z));

                                        if (GetChunk(chunkPos) == null && !chunks.ContainsKey(chunkPos))
                                        {
                                            BoundingBox chunkBoundingBox = new BoundingBox(new Vector3(chunkPos.x, 0, chunkPos.y), new Vector3(chunkPos.x + Chunk.chunkWidth, Chunk.chunkHeight, chunkPos.y + Chunk.chunkWidth));
                                            if (frustum.Intersects(chunkBoundingBox))
                                            {
                                                //         Chunk c = new Chunk(chunkPos, game.GraphicsDevice, this);
                                                tempChunkUpdatingQueue.Enqueue(chunkPos,Math.Abs(chunkPos.x-(int)player.position.X)+Math.Abs(chunkPos.y-(int)player.position.Z));
                                                // goto endUpdateWorld;
                                                //    break;
                                            }
                                            else continue;


                                        }
                                        else continue;

                                    }
                                }

                                while (tempChunkUpdatingQueue.Count > 0)
                                {
                                    Chunk c = new Chunk(tempChunkUpdatingQueue.Dequeue(), game.GraphicsDevice, this);
                                }

                                // endUpdateWorld:;

                                player1.isChunkNeededUpdate = false;

                            }
                            else
                            {
                                continue;
                            }
                    
                        }
       

                    }
                 
                

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("thread aborted update world thread");
                return;
            }
           
        }



        public void TryDeleteChunksThread(IGamePlayer player, MinecraftGameBase game)
        {
            try
            {
            while (true)
            {
                Thread.Sleep(500); 
                Debug.WriteLine(isThreadsStopping);
                if (VoxelWorld.currentWorld.worldID != worldID)
                {

                    Debug.WriteLine("world changed delchunk thread");
                    return;
                }
                //  Debug.WriteLine("delete chunks ID:" + worldID);
                 
                if (game.status == GameStatus.Quiting || game.status == GameStatus.Menu || isThreadsStopping == true)
                {
                    Debug.WriteLine("quit delchunk thread");
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

                lock ((deleteChunkThreadLock))
                {
                foreach (Chunk c in chunks.Values)
                    {

                        if ((MathF.Abs(c.chunkPos.x - player.position.X) > (GameOptions.renderDistance + Chunk.chunkWidth) || MathF.Abs(c.chunkPos.y - player.position.Z) > (GameOptions.renderDistance + Chunk.chunkWidth))
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
            catch (Exception e)
            {
                Debug.WriteLine("thread aborted");
                return;
            }
          
        }

        // = AppDomain.CurrentDomain.BaseDirectory
        public static MinecraftGameBase gameInstance;
        public static string gameWorldDataPath { get
        {
            if (OperatingSystem.IsWindows())
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }else if (OperatingSystem.IsAndroid())
            {
                return gameInstance.optionalPlatformDataPath;
            }
            else
            {
                return "";
            }
        }
        }
        public bool isWorldDataSaved;
        public static bool isJsonReadFromDisk { get; set; }
        public void SaveWorldData()
        {
            Debug.WriteLine(curWorldSaveName);
            FileStream fs;
            if (File.Exists(gameWorldDataPath + "unityMinecraftServerData/GameData/" + curWorldSaveName))
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
        public void InitWorld(MinecraftGameBase game)
        {
            gameInstance = game;
            Debug.WriteLine("current world ID:" + worldID);


            int playerInWorldID = 0;
            if (isWorldChanged == true)
            {
                if (game.gamePlayerR.gamePlayer is GamePlayer player1)
                {
                    playerInWorldID = GamePlayer.ReadPlayerData(player1, game, true);
                }
              
            }
            else
            {
                if (game.gamePlayerR.gamePlayer is GamePlayer player1)
                {
                    playerInWorldID = GamePlayer.ReadPlayerData(player1, game);
                }

                if (playerInWorldID != worldID)
                {
                    SwitchToWorldWithoutSaving(playerInWorldID, game);
                    // return;
                }
            }



            // InitChunkPool();

            chunks = new ConcurrentDictionary<Vector2Int, Chunk>();

            ReadJson();
            structureOperationsManager.ReadStructureDatas();

            EntityManager.ReadEntityData();
            EntityManager.SpawnEntityFromData(game);

            //     isGoingToQuitWorld = false;
            //       updateAllChunkLoadersThread = Task.Run(() => VoxelWorld.currentWorld.TryUpdateAllChunkLoadersThread());

            //   t2.Start();
            //       TryDeleteChunksThread = Task.Run(() => VoxelWorld.currentWorld.TryReleaseChunkThread());
            //   t3.Start();
            //    tryUpdateChunksThread = Task.Run(() => VoxelWorld.currentWorld.TryUpdateChunkThread());

            isThreadsStopping = false;
            updateWorldThread = new Thread(() => UpdateWorldThread(game.gamePlayerR.gamePlayer, game));
           
            updateWorldThread.IsBackground = true;
            updateWorldThread.Start();
            tryRemoveChunksThread = new Thread(() => TryDeleteChunksThread(game.gamePlayerR.gamePlayer, game));
            tryRemoveChunksThread.IsBackground = true;
            tryRemoveChunksThread.Start();
            if (game.gamePlayerR.gamePlayer is GamePlayer gamePlayer1)
            {
                gamePlayer1.curChunk = null;
            }
        
            worldUpdater.Init(game);
            if (actionOnSwitchedWorld != null)
            {
                actionOnSwitchedWorld();
                actionOnSwitchedWorld = null;
            }


        }

        public void FrameUpdate(float deltaTime)
        {
            worldUpdater.MainThreadUpdate(deltaTime);
        }
        public void DestroyAllChunks()
        {
            lock (updateWorldThreadLock)
            {
                lock (deleteChunkThreadLock)
                {
                    foreach (var c in chunks)
                    {

                        c.Value.Dispose();
                        Chunk _;
                        chunks.Remove<Vector2Int, Chunk>(c.Key, out _);

                    }
                    chunks.Clear();
                    chunkDataReadFromDisk.Clear();

                }
            }
            GC.Collect();


        }
        public bool isThreadsStopping = false;
        public void StopAllThreads()
        {
            isThreadsStopping = true;

         
          //  updateWorldThread.Abort();
          //  tryRemoveChunksThread.Abort();
        
            worldUpdater.StopAllThreads();
        }


        public void ReadJson()
        {
            worldStructures.Clear();
                chunkDataReadFromDisk = new Dictionary<Vector2Int, ChunkData>();
            //   gameWorldDataPath = WorldManager.gameWorldDataPath;
            Debug.WriteLine("read:" + curWorldSaveName);
            if (!Directory.Exists(gameWorldDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(gameWorldDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(gameWorldDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(gameWorldDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(gameWorldDataPath + "unityMinecraftServerData" + "/GameData/" + curWorldSaveName))
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
                if (worldGenType == 0)
                {
                    GeneratingStructureData? structureData =
                        StructureManager.LoadGeneratingStructure(Directory.GetCurrentDirectory() +
                                                                 "/customstructures/defaultstructure.bin");
                    if (structureData.HasValue == true)
                    {
                        worldStructures.Add(structureData.Value);
                    }
                }
         
           
        }

        public static void SwitchToWorldWithoutSaving(int worldIndex, MinecraftGameBase game)
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



        public void SaveAndQuitWorld(MinecraftGameBase game)
        {

            // PlayerMove player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
            if (game.gamePlayerR.gamePlayer is GamePlayer player1)
            {
                GamePlayer.SavePlayerData(player1, false);
            }
           

            EntityManager.SaveWorldEntityData();

            isThreadsStopping = true;

            StopAllThreads();
            Debug.WriteLine("main thread is threads stopping:"+isThreadsStopping);
            SaveWorldData();
            structureOperationsManager.SaveAllStructures();
        Task.Run(() => DestroyAllChunks()) ;
          //       chunks.Clear();
         //      isGoingToQuitWorld = true;

        }
        public static void SwitchToWorld(int worldIndex, MinecraftGameBase game)
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

