using MessagePack;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using monogameMinecraftShared.Animations;
using monogameMinecraftShared.Pathfinding;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using monogameMinecraftShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Client;
using monogameMinecraftNetworking.Pathfinding;
using monogameMinecraftNetworking.World;
using EntityData=monogameMinecraftNetworking.Data.EntityData;
namespace monogameMinecraftNetworking.Updateables
{
    public class ServerSideEntityManager
    {
        public static string gameWorldEntityDataPath = AppDomain.CurrentDomain.BaseDirectory;
        public static Random randomGenerator = new Random();
        public static ServerSidePathfindingManager pathfindingManager;
        public static void UpdateAllEntity(float deltaTime)
        {
            for (int i = 0; i < worldEntities.Count; i++)
            {
                worldEntities[i].OnUpdate(deltaTime);
              
            }
        }
        public static readonly float maxDelayedTime = 0.5f;
        public static float delayedTime = 0f;
        public static void FixedUpdateAllEntity(float deltaTime)
        {
            delayedTime += deltaTime;
            if (delayedTime > maxDelayedTime)
            {
                delayedTime = 0f;
                for (int i = 0; i < worldEntities.Count; i++)
                {

                    worldEntities[i].OnFixedUpdate(maxDelayedTime);
                }

              
            }
        }
        public static void TrySpawnNewZombie(IMultiplayerServer game, float deltaTime)
        {
            if (randomGenerator.NextSingle() >= 1 - deltaTime * 0.15f && worldEntities.Count < 35)
            {
                foreach (var client in game.remoteClients)
                {
                    if (client.isUserDataLoaded == true)
                    {
                        Vector2 randSpawnPos = new Vector2(client.curUserData.posX + (randomGenerator.NextSingle() - 0.5f) * 60f, client.curUserData.posZ+ (randomGenerator.NextSingle() - 0.5f) * 60f);
                        Vector3 spawnPos = new Vector3(randSpawnPos.X, ServerSideChunkHelper.GetChunkLandingPoint(randSpawnPos.X, randSpawnPos.Y,client.curUserData.curWorldID), randSpawnPos.Y);
                        SpawnNewEntity(spawnPos + new Vector3(0f, 1f, 0f), 0f, 0f, 0f, 0, game, client.curUserData.curWorldID);
                    }
                }
             

            }
        }

        public static void ReadEntityData()
        {
            worldEntities.Clear();
            //   gameWorldDataPath = WorldManager.gameWorldDataPath;

            if (!Directory.Exists(gameWorldEntityDataPath + "unityMinecraftServerData"))
            {
                Directory.CreateDirectory(gameWorldEntityDataPath + "unityMinecraftServerData");

            }
            if (!Directory.Exists(gameWorldEntityDataPath + "unityMinecraftServerData/GameData"))
            {
                Directory.CreateDirectory(gameWorldEntityDataPath + "unityMinecraftServerData/GameData");
            }

            if (!File.Exists(gameWorldEntityDataPath + "unityMinecraftServerData" + "/GameData/worldentities.json"))
            {
                FileStream fs = File.Create(gameWorldEntityDataPath + "unityMinecraftServerData" + "/GameData/worldentities.json");
                fs.Close();
            }

            byte[] worldData = File.ReadAllBytes(gameWorldEntityDataPath + "unityMinecraftServerData/GameData/worldentities.json");
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
                entityDataReadFromDisk = MessagePackSerializer.Deserialize<List<EntityData>>(worldData);
            }


        }
        public static void SaveWorldEntityData()
        {

            FileStream fs;
            if (File.Exists(gameWorldEntityDataPath + "unityMinecraftServerData/GameData/worldentities.json"))
            {
                fs = new FileStream(gameWorldEntityDataPath + "unityMinecraftServerData/GameData/worldentities.json", FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(gameWorldEntityDataPath + "unityMinecraftServerData/GameData/worldentities.json", FileMode.Create, FileAccess.Write);
            }
            fs.Close();

            foreach (ServerSideEntityBeh e in worldEntities)
            {
                e.SaveSingleEntity();
            }
            //   Debug.Log(entityDataReadFromDisk.Count);
            /*  foreach(EntityData ed in entityDataReadFromDisk){
               string tmpData=JsonSerializer.ToJsonString(ed);
               File.AppendAllText(gameWorldEntityDataPath+"unityMinecraftData/GameData/worldentities.json",tmpData+"\n");
              }*/
            byte[] tmpData = MessagePackSerializer.Serialize(entityDataReadFromDisk);
            File.WriteAllBytes(gameWorldEntityDataPath + "unityMinecraftServerData/GameData/worldentities.json", tmpData);

        }

        public static void StopAllThreads()
        {
            pathfindingManager.QuitThread();
        }

        public static void SpawnEntityFromData(IMultiplayerServer server)
        {
            foreach (var etd in entityDataReadFromDisk)
            {
                 
                    switch (etd.typeid)
                    {
                        case 0:
                            ServerSideZombieEntityBeh tmp = new ServerSideZombieEntityBeh(new Vector3(etd.posX, etd.posY, etd.posZ), etd.rotX, etd.rotY, etd.rotZ, etd.entityID, etd.entityHealth, false,etd.entityInWorldID, server);

                            break;
                        default:
                            break;
                    }

                

            }
        }

        public static List<EntityData> entityDataReadFromDisk = new List<EntityData>();
        public static List<ServerSideEntityBeh> worldEntities = new List<ServerSideEntityBeh>();
        public static List<EntityData> allEntityDatas=new  List<EntityData>();

        public static void UpdateCurEntityData()
        {
            allEntityDatas.Clear();
            foreach (var entity in worldEntities)
            {
                allEntityDatas.Add(entity.ToEntityData());
            }
        }
      
        public static void InitEntityList()
        {
            worldEntities = new List<ServerSideEntityBeh>();
            pathfindingManager = new ServerSidePathfindingManager();
            pathfindingManager.Initialize();
        }

 
      /*  public static void LoadEntitySounds(ContentManager cm)
        {
            entitySounds.TryAdd("0hurt", cm.Load<SoundEffect>("sounds/zombiehurt"));
            entitySounds.TryAdd("0idle", cm.Load<SoundEffect>("sounds/zombiesay"));
        }*/


        public static void SpawnNewEntity(Vector3 position, float rotationX, float rotationY, float rotationZ, int typeID,IMultiplayerServer server,int worldID)
        {
            switch (typeID)
            {
                case 0:
                    ServerSideZombieEntityBeh tmp = new ServerSideZombieEntityBeh(new Vector3(position.X, position.Y, position.Z), rotationX, rotationY, rotationZ, Guid.NewGuid().ToString("N"),20f, false,worldID, server);
                    //   ZombieEntityBeh tmp = new ZombieEntityBeh(position, rotationX, rotationY, rotationZ, Guid.NewGuid().ToString("N"), 20f, false, game);

                    break;
                default:
                    break;
            }

        }


        public static void HurtEntity(string entityID, float hurtValue, Vector3 sourcePos)
        {
            ServerSideEntityBeh ServerSideEntityBeh;
            int index = worldEntities.FindIndex((e) => { return entityID == e.entityID; });
            if (index != -1)
            {
                ServerSideEntityBeh = worldEntities[index];
            }
            else
            {
                return;
            }
            if (ServerSideEntityBeh.isEntityHurt == true)
            {
                return;
            }
        /*    if (entitySounds.ContainsKey(ServerSideEntityBeh.typeID + "hurt"))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, ServerSideEntityBeh.position, entitySounds[ServerSideEntityBeh.typeID + "hurt"], 20f);
            }*/

            ServerSideEntityBeh.entityHealth -= hurtValue;
            ServerSideEntityBeh.entityHurtCD = 0.2f;
            ServerSideEntityBeh.entityMotionVec = Vector3.Normalize(ServerSideEntityBeh.position - sourcePos) * 15f;
        }
    }

    
}

