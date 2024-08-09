using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftShared;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.World;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace monogameMinecraftNetworking.World
{
    public class ServerSideVoxelWorld
    {
        public int worldGenType = 0;
        public int worldID = 0;
        public string curWorldSaveName = "default.bin";
        public static List<ServerSideVoxelWorld> voxelWorlds = new List<ServerSideVoxelWorld>{
            new ServerSideVoxelWorld("world.bin",0,0),
            new ServerSideVoxelWorld("worldender.bin",2,1)};


        public WorldGenParamsData genParamsData = new WorldGenParamsData();
        public FastNoise noiseGenerator = new FastNoise();
        public FastNoise biomeNoiseGenerator = new FastNoise();
        public FastNoise frequentNoiseGenerator = new FastNoise();

        public ConcurrentDictionary<Vector2Int, ServerSideChunk> chunks = new ConcurrentDictionary<Vector2Int, ServerSideChunk>();
        public Dictionary<Vector2Int, ChunkData> chunkDataReadFromDisk = new Dictionary<Vector2Int, ChunkData>();
        public bool isThreadsStopping;

        public ServerSideWorldUpdater worldUpdater;
        public bool isWorldDataSaved;
        public List<GeneratingStructureData> worldStructures = new List<GeneratingStructureData>();

        public object chunkBuildingQueueLock=new object();
        public Queue<(RemoteClient client, Vector2Int chunkPos)> chunkBuildingQueue=new Queue<(RemoteClient client, Vector2Int chunkPos)>();


        public Thread updateWorldThread;
        public Thread tryRemoveChunksThread;

        public object updateWorldThreadLock = new object();
        public object deleteChunkThreadLock = new object();
        public static IMultiplayerServer serverInstance;
        public ServerSideVoxelWorld(string curWorldSaveName, int worldGenType, int worldID)
        {
            this.worldGenType = worldGenType;
            this.worldID = worldID;
            this.curWorldSaveName = curWorldSaveName;
            this.worldUpdater = new ServerSideWorldUpdater(this);
        }

        public ServerSideChunk GetChunk(Vector2Int chunkPos)
        {
            if (chunks == null)
            {
                return null;
            }
            if (chunks.ContainsKey(chunkPos))
            {
                ServerSideChunk tmp = chunks[chunkPos];
                return tmp;
            }
            else
            {
                return null;
            }

        }

        public static string gameWorldDataPath
        {
            get
            {
                if (OperatingSystem.IsWindows())
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }

                else
                {
                    return "";
                }
            }
        }

        public void SaveWorldData()
        {
            Console.WriteLine(curWorldSaveName);
           
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
            foreach (KeyValuePair<Vector2Int, ServerSideChunk> c in chunks)
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

        public void DestroyAllChunks()
        {
            lock (updateWorldThreadLock)
            {
                lock (deleteChunkThreadLock)
                {
                    foreach (var c in chunks)
                    {

                        c.Value.Dispose();
                        ServerSideChunk chunk;
                        chunks.Remove<Vector2Int, ServerSideChunk>(c.Key, out chunk);

                    }
                    chunks.Clear();
                    chunkDataReadFromDisk.Clear();

                }
            }
            GC.Collect();


        }

        public void ShutDown()
        {
            SaveWorldData();
            StopAllThreads();
            worldUpdater.StopAllThreads();
            DestroyAllChunks();
        }
        public void ReadJson()
        {
            worldStructures.Clear();
            chunkDataReadFromDisk = new Dictionary<Vector2Int, ChunkData>();
            //   gameWorldDataPath = WorldManager.gameWorldDataPath;
            Console.WriteLine("read:" + curWorldSaveName);
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
            Console.WriteLine("saved chunks:" + chunkDataReadFromDisk.Count);
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

        public void SetupNoiseGenerators(float bngf,float ngf,float fngf)
        {
            genParamsData = new WorldGenParamsData(worldGenType, bngf, ngf, fngf,worldID);
            noiseGenerator.SetFrequency(genParamsData.noiseGeneratorFrequency);

            frequentNoiseGenerator.SetFrequency(genParamsData.frequentNoiseGeneratorFrequency);
            biomeNoiseGenerator.SetFrequency(genParamsData.biomeNoiseGeneratorFrequency);
        }
        public void InitWorld(IMultiplayerServer server)
        {

            Console.WriteLine("initializing world ID:" + worldID);
        
            SetupNoiseGenerators(0.002f, 0.01f, 0.01f);

            


            // InitChunkPool();

            chunks = new ConcurrentDictionary<Vector2Int, ServerSideChunk>();

            ReadJson();
        //    structureOperationsManager.ReadStructureDatas();

          //  EntityManager.ReadEntityData();
         //   EntityManager.SpawnEntityFromData(game);

            //     isGoingToQuitWorld = false;
            //       updateAllChunkLoadersThread = Task.Run(() => VoxelWorld.currentWorld.TryUpdateAllChunkLoadersThread());

            //   t2.Start();
            //       TryDeleteChunksThread = Task.Run(() => VoxelWorld.currentWorld.TryReleaseChunkThread());
            //   t3.Start();
            //    tryUpdateChunksThread = Task.Run(() => VoxelWorld.currentWorld.TryUpdateChunkThread());

            isThreadsStopping = false;
            updateWorldThread = new Thread(() => UpdateWorldThread(server));
            updateWorldThread.IsBackground = true;
            updateWorldThread.Start();
            tryRemoveChunksThread = new Thread(() => TryDeleteChunksThread(server));
            tryRemoveChunksThread.IsBackground = true;
            tryRemoveChunksThread.Start();
        
            worldUpdater.Init();
           


        }

        public void UpdateWorldThread(IMultiplayerServer server)
        {
            while (true)
            {
                Thread.Sleep(50);
                if (isThreadsStopping == true)
                {
                    Console.WriteLine("quit update world thread:"+Thread.CurrentThread.ManagedThreadId);
                    return;
                }
                lock (updateWorldThreadLock)
                {
                    lock (chunkBuildingQueueLock)
                    {
                    if (chunkBuildingQueue.Count > 0)
                    {
                        (RemoteClient remoteClient,Vector2Int chunkPos) item = chunkBuildingQueue.Dequeue();
                        
                        if (item.chunkPos.x % Chunk.chunkWidth != 0 || item.chunkPos.y % Chunk.chunkWidth != 0)
                        {
                            continue;
                        }

                        if (GetChunk(item.chunkPos) == null && !chunks.ContainsKey(item.chunkPos))
                        {
                           
                            //         Chunk c = new Chunk(chunkPos, game.GraphicsDevice, this);
                            //      tempChunkUpdatingQueue.Enqueue(chunkPos, Math.Abs(chunkPos.x - (int)player.position.X) + Math.Abs(chunkPos.y - (int)player.position.Z));
                            ServerSideChunk c = new ServerSideChunk(item.chunkPos, this);
                            // goto endUpdateWorld;
                            //    break;
                            NetworkingUtility.SendToClient(item.remoteClient,new MessageProtocol((byte)MessageCommandType.WorldData,ChunkDataSerializingUtility.SerializeChunkWithWorldID(c,worldID)));
                          
                        }
                        else
                        {
                            ServerSideChunk c = GetChunk(item.chunkPos);
                               
                            NetworkingUtility.SendToClient(item.remoteClient, new MessageProtocol((byte)MessageCommandType.WorldData, ChunkDataSerializingUtility.SerializeChunkWithWorldID(c, worldID)));
                        }
                    }
                    }
                    
                }
                
            }
           
        }

        public void TryDeleteChunksThread(IMultiplayerServer server)
        {
            while (true)
            {
                Thread.Sleep(50);
                if (isThreadsStopping == true)
                {
                    Console.WriteLine("quit delete chunks thread:" + Thread.CurrentThread.ManagedThreadId);
                    return;
                }
                lock (updateWorldThreadLock)
                {
                    lock (chunkBuildingQueueLock)
                    {
                        foreach (var c in chunks.Values)
                        {
                            bool isChunkRemoving=true;
                            
                            foreach (var userData in server.allUserDatas)
                            {
                                if (userData.curWorldID!=worldID)
                                {
                                    continue;
                                }
                                Vector3 userPos = new Vector3(userData.posX, userData.posY, userData.posZ);
                                if (c.isMapGenCompleted == true &&
                                    (MathF.Abs(c.chunkPos.x - userPos.X) < 128 + Chunk.chunkWidth &&
                                     MathF.Abs(c.chunkPos.y - userPos.Z) <128 + Chunk.chunkWidth))
                                {
                                  isChunkRemoving=false;

                                }
                            }

                            if (isChunkRemoving == true&&c.isMapGenCompleted==true)
                            {
                                c.SaveSingleChunk();
                                c.isUnused = true;
                            //    c.Dispose();
                      //      Console.WriteLine("remove chunk");
                                chunks.TryRemove(c.chunkPos, out _);
                            }
                        }
                    }

                }

            }
        }
        public void StopAllThreads()
        {
            isThreadsStopping = true;
            tryRemoveChunksThread?.Join();
            updateWorldThread?.Join();
            worldUpdater.StopAllThreads();
        }
        public bool isJsonReadFromDisk { get; set; }
    }
}
