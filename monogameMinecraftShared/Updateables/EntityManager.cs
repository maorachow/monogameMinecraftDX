using MessagePack;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using monogameMinecraftShared.Animations;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.Pathfinding;
using monogameMinecraftShared.World;
using monogameMinecraftShared.Core;

namespace monogameMinecraftShared.Updateables
{
    public class EntityManager
    {
        public static string gameWorldEntityDataPath = AppDomain.CurrentDomain.BaseDirectory;
        public static Random randomGenerator = new Random();
        public static PathfindingManager pathfindingManager;
        public static void UpdateAllEntity(float deltaTime)
        {
            for (int i = 0; i < worldEntities.Count; i++)
            {
                worldEntities[i].OnUpdate(deltaTime);
            }
        }
        public static readonly float maxDelayedTime = 0.2f;
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

                if (worldEntities.Count > 0)
                {
                    pathfindingManager.curDebuggingPath = ((ZombieEntityBeh)worldEntities[0])?.entityPath;
                }
            }
        }
        public static void TrySpawnNewZombie(MinecraftGameBase game, float deltaTime)
        {
            if (randomGenerator.NextSingle() >= 1 - deltaTime * 0.15f && worldEntities.Count < 35 && VoxelWorld.currentWorld.worldID == 0)
            {
                Vector2 randSpawnPos = new Vector2(game.gamePlayerR.gamePlayer.position.X + (randomGenerator.NextSingle() - 0.5f) * 60f, game.gamePlayerR.gamePlayer.position.Z + (randomGenerator.NextSingle() - 0.5f) * 60f);
                if((randSpawnPos-new Vector2(game.gamePlayerR.gamePlayer.position.X, game.gamePlayerR.gamePlayer.position.Z)).Length()<10f)
                {
                    return;
                }
                Vector3 spawnPos = new Vector3(randSpawnPos.X, ChunkHelper.GetChunkLandingPoint(randSpawnPos.X, randSpawnPos.Y), randSpawnPos.Y);
                if(ChunkHelper.GetChunk(ChunkCoordsHelper.Vec3ToChunkPos(spawnPos))==null)
                {
                    return;
                }
                SpawnNewEntity(spawnPos + new Vector3(0f, 1f, 0f), 0f, 0f, 0f, 0, game);

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

            foreach (EntityBeh e in worldEntities)
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

        public static void SpawnEntityFromData(MinecraftGameBase game)
        {
            foreach (var etd in entityDataReadFromDisk)
            {
                if (etd.entityInWorldID == VoxelWorld.currentWorld.worldID)
                {
                    switch (etd.typeid)
                    {
                        case 0:
                            ZombieEntityBeh tmp = new ZombieEntityBeh(new Vector3(etd.posX, etd.posY, etd.posZ), etd.rotX, etd.rotY, etd.rotZ, etd.entityID, etd.entityHealth, false, game);

                            break;
                        default:
                            break;
                    }

                }

            }
        }

        public static List<EntityData> entityDataReadFromDisk = new List<EntityData>();
        public static List<EntityBeh> worldEntities = new List<EntityBeh>();

        public static void InitEntityList()
        {
            worldEntities = new List<EntityBeh>();
            pathfindingManager = new PathfindingManager();
            pathfindingManager.Initialize();
        }

        [Obsolete]
        public static Animation zombieAnim = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "leftLeg",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) }
            }, 0.5f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 75f, 0f),  new Vector3(1f, 1f, 1f)) },
                { "leftLeg", new AnimationTransformation(new Vector3(0f,0.0f, 0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
            }, 0.5f)
        }, true);
        [Obsolete]
        public static Animation entityDieAnim = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "waist", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f,0f, 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.4f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "waist", new AnimationTransformation(new Vector3(0f, -0.75f, 0f), new Vector3(0f,0f, -90f), new Vector3(1f, 1f, 1f)) },
            }, 0.1f)
        }, false);
        [Obsolete]
        public static Dictionary<string, SoundEffect> entitySounds = new Dictionary<string, SoundEffect>();


        public static void LoadEntitySounds(ContentManager cm)
        {
            entitySounds.TryAdd("0hurt", cm.Load<SoundEffect>("sounds/zombiehurt"));
            entitySounds.TryAdd("0idle", cm.Load<SoundEffect>("sounds/zombiesay"));
        }


        public static void SpawnNewEntity(Vector3 position, float rotationX, float rotationY, float rotationZ, int typeID, MinecraftGameBase game)
        {
            switch (typeID)
            {
                case 0:
                    ZombieEntityBeh tmp = new ZombieEntityBeh(position, rotationX, rotationY, rotationZ, Guid.NewGuid().ToString("N"), 20f, false, game);

                    break;
                default:
                    break;
            }

        }


        public static void HurtEntity(string entityID, float hurtValue, Vector3 sourcePos)
        {
            EntityBeh entityBeh;
            int index = worldEntities.FindIndex((e) => { return entityID == e.entityID; });
            if (index != -1)
            {
                entityBeh = worldEntities[index];
            }
            else
            {
                return;
            }
            if (entityBeh.isEntityHurt == true)
            {
                return;
            }
            if (entitySounds.ContainsKey(entityBeh.typeID + "hurt"))
            {
                SoundsUtility.PlaySound(MinecraftGameBase.gameposition, entityBeh.position, entitySounds[entityBeh.typeID + "hurt"], 20f);
            }

            entityBeh.entityHealth -= hurtValue;
            entityBeh.entityHurtCD = 0.2f;
            entityBeh.entityMotionVec = Vector3.Normalize(entityBeh.position - sourcePos) * 15f;
        }
    }

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

        public EntityData(int typeid, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, string entityID, float entityHealth, int entityInWorldID)
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
        }
    }
}
