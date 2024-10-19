using monogameMinecraftShared.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Data;
using System.Diagnostics;
using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Client.Rendering;
using monogameMinecraftNetworking.Protocol;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using monogameMinecraftShared.Asset;

namespace monogameMinecraftNetworking.Client.Updateables
{
    public class ClientSideEntityCacheObject
    {
        public EntityData data;
        public object entityOptionalData;
        public AnimationBlend animState;
        public float entitySpeed;

        public ClientSideEntityCacheObject(EntityData data, AnimationBlend animState, float speed)
        {
            this.data = data;
            this.animState = animState;
            this.entitySpeed=speed;
        }

    }
    public class ClientSideEntityManager
    {
        public object allEntitiesCacheLock=new object();
        public List<ClientSideEntityCacheObject> allEntitiesCache;
        public List<EntityData> lastAllEntitiesDatas;
        public List<EntityData> lastPreviousAllEntitiesDatas;
        public IMultiplayerClient client;
        public float timeSinceLastUpdate = 0f;
        public float previousTimeSinceLastUpdate = 0.05f;
        public ClientSideEntityManager(IMultiplayerClient client)
        {
            this.client= client;
            this.allEntitiesCache = new List<ClientSideEntityCacheObject>();
            this.lastAllEntitiesDatas = new List<EntityData>();
            this.lastPreviousAllEntitiesDatas=new List<EntityData>();
            isFirstUpdatePassed = false;
            client.allEntitiesUpdatedAction += Update;
            client.allEntitiesPreUpdatedAction += PreUpdate;
        }
        [Obsolete]
        public static Dictionary<string, SoundEffect> entitySounds = new Dictionary<string, SoundEffect>();


        public static void LoadEntityAssets(ContentManager cm)
        {
            EntityResourcesManager.instance.TryLoadCustomEntitySounds(cm,new Tuple<string, string>("0hurt", "sounds/zombiehurt"),

                new Tuple<string, string>("0say", "sounds/zombiesay"),
                new Tuple<string, string>( "1hurt","sounds/pighurt"),
                new Tuple<string, string>("1say","sounds/pigsay")

                );
            EntityResourcesManager.instance.TryLoadCustomEntityAnims(new Tuple<string, Animation>("zombieAnim", new Animation(new List<AnimationStep> {

                    new AnimationStep(new Dictionary<string, AnimationTransformation> {

                        { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leftLeg",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.5f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {
                        { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 75f, 0f),  new Vector3(1f, 1f, 1f)) },
                        { "leftLeg", new AnimationTransformation(new Vector3(0f,0.0f, 0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.5f)
                }, true)),
                new Tuple<string, Animation>("pigWalkingAnim", new Animation(new List<AnimationStep> {

                    new AnimationStep(new Dictionary<string, AnimationTransformation> {

                        { "leg0", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leg1",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leg2",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leg3",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.5f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {
                        { "leg0", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leg1",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leg2",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
                        { "leg3",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.5f)
                }, true)),

                new Tuple<string, Animation>("entityDieAnim", new Animation(new List<AnimationStep> {

                    new AnimationStep(new Dictionary<string, AnimationTransformation> {

                        { "waist", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f,0f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.4f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {
                        { "waist", new AnimationTransformation(new Vector3(0f, -0.75f, 0f), new Vector3(0f,0f, -90f), new Vector3(1f, 1f, 1f)) },
                    }, 0.1f)
                }, false)),
                new Tuple<string, Animation>("entityDieAnimRoot", new Animation(new List<AnimationStep> {

                    new AnimationStep(new Dictionary<string, AnimationTransformation> {

                        { "RootNode", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f,0f, 0f), new Vector3(1f, 1f, 1f)) },

                    }, 0.4f),
                    new AnimationStep(new Dictionary<string, AnimationTransformation> {
                        { "RootNode", new AnimationTransformation(new Vector3(0f, 0f, 0f), new Vector3(0f,0f, -90f), new Vector3(1f, 1f, 1f)) },
                    }, 0.1f)
                }, false))
             
                );
            EntityResourcesManager.instance.TryAddCustomEntityModels(cm,
                new CustomModelLoadingItem("zombie","zombiefbx","husk"),
                new CustomModelLoadingItem("pig", "pigfbx", "pig"));
            //entitySounds.TryAdd("0hurt", cm.Load<SoundEffect>());
          //  entitySounds.TryAdd("0say", cm.Load<SoundEffect>("sounds/zombiesay"));
          //  entitySounds.TryAdd("1hurt", cm.Load<SoundEffect>("sounds/pighurt"));
          //  entitySounds.TryAdd("1say", cm.Load<SoundEffect>("sounds/pigsay"));
        }
        

        public bool isFirstUpdatePassed = false;
        public void Update()
        {
            lock (allEntitiesCacheLock)
            {
                //   Debug.WriteLine("update entities");
                lastAllEntitiesDatas = client.allEntityDatas;
                foreach (var item1 in lastAllEntitiesDatas)
                {
                    if (allEntitiesCache.FindIndex((item) => { return item.data.entityID == item1.entityID; }) == -1)
                    {
                        switch (item1.typeid)
                        {
                            
                            case 0:
                                allEntitiesCache.Add(new ClientSideEntityCacheObject(item1, new SingleTexturedAnimatedModel(new AnimationState[]
                                {
                                    new AnimationState(EntityResourcesManager.instance.loadedEntityAnims["zombieAnim"],EntityResourcesManager.instance.loadedEntityModels["zombie"].model),

                                    new AnimationState(EntityResourcesManager.instance.loadedEntityAnims["entityDieAnim"],EntityResourcesManager.instance.loadedEntityModels["zombie"].model)
                                }, EntityResourcesManager.instance.loadedEntityModels["zombie"].model, EntityResourcesManager.instance.loadedEntityModels["zombie"].texture), 0f));
                                break;
                            case 1:
                                allEntitiesCache.Add(new ClientSideEntityCacheObject(item1, new SingleTexturedAnimatedModel(new AnimationState[]
                                {
                                    new AnimationState(EntityResourcesManager.instance.loadedEntityAnims["pigWalkingAnim"],  EntityResourcesManager.instance.loadedEntityModels["pig"].model),

                                    new AnimationState(EntityResourcesManager.instance.loadedEntityAnims["entityDieAnimRoot"],  EntityResourcesManager.instance.loadedEntityModels["pig"].model)
                                }, EntityResourcesManager.instance.loadedEntityModels["pig"].model, EntityResourcesManager.instance.loadedEntityModels["pig"].texture), 0f));
                                break;
                        }
                    
                    }
                }

                for (int i = 0; i < allEntitiesCache.Count; i++)
                {
                    ClientSideEntityCacheObject item2 = allEntitiesCache[i];
                    if (lastAllEntitiesDatas.FindIndex((item) => { return item.entityID == item2.data.entityID; }) == -1)
                    {
                        allEntitiesCache.RemoveAt(i);
                        i--;
                    }
                }

             

                previousTimeSinceLastUpdate = timeSinceLastUpdate;
                timeSinceLastUpdate = 0f;
                if (previousTimeSinceLastUpdate <= 0f)
                {
                    previousTimeSinceLastUpdate = 0.05f;
                }
              
            }

        }
        public void PreUpdate()
        {
            
                //   Debug.WriteLine("update entities");
                if (client.allEntityDatas != null)
                {
                    lastPreviousAllEntitiesDatas =client.allEntityDatas;
                }
               
               

        }
        public void FrameUpdate(float deltaTime)
        {
            timeSinceLastUpdate += deltaTime;
            /*  if (lastPreviousAllEntitiesDatas.Count > 0 && lastAllEntitiesDatas.Count > 0)
              {
                  Debug.WriteLine("prev data:" + new Vector3(lastPreviousAllEntitiesDatas[0].posX, lastPreviousAllEntitiesDatas[0].posY, lastPreviousAllEntitiesDatas[0].posZ) + " cur data:" + new Vector3(lastAllEntitiesDatas[0].posX, lastAllEntitiesDatas[0].posY, lastAllEntitiesDatas[0].posZ));
              }*/
            //   Debug.WriteLine("entity lerp time:" + timeSinceLastUpdate / previousTimeSinceLastUpdate);
            if (lastPreviousAllEntitiesDatas == null)
            {
                Debug.WriteLine("previous data null");
                return;

            }


            foreach (var item1 in lastAllEntitiesDatas)
            {
                int idx = allEntitiesCache.FindIndex((item) => { return item.data.entityID == item1.entityID; });
                int idxInPreviousData = lastPreviousAllEntitiesDatas.FindIndex((item) =>
                {
                    return item.entityID == item1.entityID;
                });
                if (idx != -1 && idxInPreviousData != -1)
                {
                    Vector3 curPos = new Vector3(item1.posX, item1.posY,
                        item1.posZ);
                    Vector3 lastPos = new Vector3(lastPreviousAllEntitiesDatas[idxInPreviousData].posX,
                        lastPreviousAllEntitiesDatas[idxInPreviousData].posY,
                        lastPreviousAllEntitiesDatas[idxInPreviousData].posZ);
                    /*   Vector3 targetPos = new Vector3(item1.posX, item1.posY,
                           item1.posZ);*/
                    Vector3 curRot = new Vector3(allEntitiesCache[idx].data.rotX, allEntitiesCache[idx].data.rotY,
                        allEntitiesCache[idx].data.rotZ);
                    Vector3 targetRot = new Vector3(item1.rotX, item1.rotY,
                        item1.rotZ);

                    Vector3 lerpPos = Vector3.Lerp(lastPos, curPos, timeSinceLastUpdate / previousTimeSinceLastUpdate);
                    Vector3 lerpRot = Vector3.Lerp(curRot, targetRot, 10f * deltaTime);
                    float moveLength =
                        (new Vector3(allEntitiesCache[idx].data.posX, 0, allEntitiesCache[idx].data.posZ) -
                         new Vector3(lerpPos.X, 0, lerpPos.Z)).Length();

                    allEntitiesCache[idx].data.posX = lerpPos.X;
                    allEntitiesCache[idx].data.posY = lerpPos.Y;
                    allEntitiesCache[idx].data.posZ = lerpPos.Z;

                    allEntitiesCache[idx].data.rotX = targetRot.X;
                    allEntitiesCache[idx].data.rotY = targetRot.Y;
                    allEntitiesCache[idx].data.rotZ = targetRot.Z;
                    allEntitiesCache[idx].data.isEntityHurt = item1.isEntityHurt;
                    allEntitiesCache[idx].data.isEntityDying = item1.isEntityDying;
                    allEntitiesCache[idx].data.entityInWorldID = item1.entityInWorldID;
                    allEntitiesCache[idx].entitySpeed = moveLength / deltaTime;
                    if (float.IsInfinity(moveLength) == true)
                    {
                        Debug.WriteLine("inf move length");
                        moveLength = 0.1f;
                        allEntitiesCache[idx].entitySpeed = moveLength / deltaTime;
                    }

                    // Debug.WriteLine("speed:"+ (moveLength / deltaTime));
                    if (allEntitiesCache[idx].data.isEntityDying == false)
                    {
                        if (allEntitiesCache[idx].entitySpeed / 4f < 0.1f)
                        {
                            float lerpValue =
                                MathHelper.Lerp(
                                    allEntitiesCache[idx].animState.animationStates[0].elapsedTimeInStep, 0.25f,
                                    deltaTime * 10f);
                            float animationUpdateDelta = lerpValue - allEntitiesCache[idx].animState
                                .animationStates[0].elapsedTimeInStep;
                            allEntitiesCache[idx].animState.Update(animationUpdateDelta, 1, 0);
                        }
                        else
                        {
                            allEntitiesCache[idx].animState
                                .Update(deltaTime, allEntitiesCache[idx].entitySpeed / 4f, 0);
                        }
                    }
                    else
                    {
                        allEntitiesCache[idx].animState.Update(deltaTime, 0, 1);
                    }

                    allEntitiesCache[idx].data.optionalData = item1.optionalData;
                    switch (allEntitiesCache[idx].data.typeid)
                    {
                        case 0:
                            Float4Data? curBodyQuatData =
                                (Float4Data.FromBytes(item1.optionalData) as Float4Data?);

                            Float4Data? prevBodyQuatData =
                                (Float4Data.FromBytes(lastPreviousAllEntitiesDatas[idxInPreviousData].optionalData) as Float4Data?);
                            Quaternion curBodyQuat = new Quaternion(curBodyQuatData.Value.x, curBodyQuatData.Value.y,
                                curBodyQuatData.Value.z, curBodyQuatData.Value.w);

                            Quaternion prevBodyQuat = new Quaternion(prevBodyQuatData.Value.x, prevBodyQuatData.Value.y,
                                prevBodyQuatData.Value.z, prevBodyQuatData.Value.w);
                            Quaternion lerpBodyQuat=Quaternion.Lerp(prevBodyQuat,curBodyQuat, timeSinceLastUpdate / previousTimeSinceLastUpdate);
                            allEntitiesCache[idx].entityOptionalData =
                                new Float4Data(lerpBodyQuat.X, lerpBodyQuat.Y, lerpBodyQuat.Z, lerpBodyQuat.W) as Float4Data?;
                              //  (Float4Data.FromBytes(allEntitiesCache[idx].data.optionalData) as Float4Data?);
                            break;
                        case 1:
                            Float4Data? curBodyQuatData1 =
                                Float4Data.FromBytes(item1.optionalData);

                            Float4Data? prevBodyQuatData1 =
                                Float4Data.FromBytes(lastPreviousAllEntitiesDatas[idxInPreviousData].optionalData);
                            Quaternion curBodyQuat1 = new Quaternion(curBodyQuatData1.Value.x, curBodyQuatData1.Value.y,
                                curBodyQuatData1.Value.z, curBodyQuatData1.Value.w);

                            Quaternion prevBodyQuat1 = new Quaternion(prevBodyQuatData1.Value.x, prevBodyQuatData1.Value.y,
                                prevBodyQuatData1.Value.z, prevBodyQuatData1.Value.w);
                            Quaternion lerpBodyQuat1 = Quaternion.Lerp(prevBodyQuat1, curBodyQuat1, timeSinceLastUpdate / previousTimeSinceLastUpdate);
                            allEntitiesCache[idx].entityOptionalData =
                                new Float4Data(lerpBodyQuat1.X, lerpBodyQuat1.Y, lerpBodyQuat1.Z, lerpBodyQuat1.W) as Float4Data?;
                            break;
                    }
                }
                else if (idxInPreviousData == -1 && idx != -1)
                {
                    // Debug.WriteLine("previous data not found"+ timeSinceLastUpdate);
                    Vector3 targetPos = new Vector3(item1.posX, item1.posY,
                        item1.posZ);

                    Vector3 targetRot = new Vector3(item1.rotX, item1.rotY,
                        item1.rotZ);


                    allEntitiesCache[idx].data.posX = targetPos.X;
                    allEntitiesCache[idx].data.posY = targetPos.Y;
                    allEntitiesCache[idx].data.posZ = targetPos.Z;

                    allEntitiesCache[idx].data.rotX = targetRot.X;
                    allEntitiesCache[idx].data.rotY = targetRot.Y;
                    allEntitiesCache[idx].data.rotZ = targetRot.Z;
                    allEntitiesCache[idx].data.isEntityHurt = item1.isEntityHurt;
                    allEntitiesCache[idx].data.entityInWorldID = item1.entityInWorldID;
                    allEntitiesCache[idx].data.isEntityDying = item1.isEntityDying;
                    allEntitiesCache[idx].entitySpeed = 0f;
                    if (allEntitiesCache[idx].data.isEntityDying == false)
                    {
                        allEntitiesCache[idx].animState.Update(deltaTime, 0f, 0);
                    }
                    else
                    {
                        allEntitiesCache[idx].animState.Update(deltaTime, 0, 1);
                    }

                    allEntitiesCache[idx].data.optionalData = item1.optionalData;
                    switch (allEntitiesCache[idx].data.typeid)
                    {
                        case 0:
                            allEntitiesCache[idx].entityOptionalData =
                                (Float4Data.FromBytes(allEntitiesCache[idx].data.optionalData) as Float4Data?);
                            break;
                        case 1:
                            allEntitiesCache[idx].entityOptionalData =
                                (Float4Data.FromBytes(allEntitiesCache[idx].data.optionalData) as Float4Data?);
                            break;
                    }
                }
            }

        }

        public void SendHurtEntityRequest(string entityID, float hurtValue,Vector3 sourcePos)
        {

            if (client.isGoingToQuitGame == false)
            {
                HurtEntityRequestData data =
                    new HurtEntityRequestData(entityID, hurtValue, sourcePos.X, sourcePos.Y, sourcePos.Z);
                byte[] dataBytes = MessagePackSerializer.Serialize(data);
                client.todoList.Enqueue(new MessageProtocol((byte)MessageCommandType.HurtEntityRequest,dataBytes));
            }
             
        }

    }
}
