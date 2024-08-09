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

namespace monogameMinecraftNetworking.Client.Updateables
{
    public class ClientSideEntityCacheObject
    {
        public EntityData data;
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
                        allEntitiesCache.Add(new ClientSideEntityCacheObject(item1, new AnimationBlend(new AnimationState[]
                        {
                            new AnimationState(ClientSideEntitiesRenderer. zombieAnim, ClientSideEntitiesRenderer.zombieModel),

                            new AnimationState(ClientSideEntitiesRenderer. entityDieAnim, ClientSideEntitiesRenderer.zombieModel)
                        }, ClientSideEntitiesRenderer.zombieModel),0f));
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
                    int idxInPreviousData= lastPreviousAllEntitiesDatas.FindIndex((item) => { return item.entityID == item1.entityID; });
                    if (idx != -1&&idxInPreviousData!=-1)
                    {

                        Vector3 curPos = new Vector3(item1.posX, item1.posY,
                            item1.posZ);
                        Vector3 lastPos = new Vector3(lastPreviousAllEntitiesDatas[idxInPreviousData].posX, lastPreviousAllEntitiesDatas[idxInPreviousData].posY,
                            lastPreviousAllEntitiesDatas[idxInPreviousData].posZ);
                     /*   Vector3 targetPos = new Vector3(item1.posX, item1.posY,
                            item1.posZ);*/
                        Vector3 curRot = new Vector3(allEntitiesCache[idx].data.rotX, allEntitiesCache[idx].data.rotY,
                            allEntitiesCache[idx].data.rotZ);
                        Vector3 targetRot = new Vector3(item1.rotX, item1.rotY,
                            item1.rotZ);

                        Vector3 lerpPos = Vector3.Lerp(lastPos, curPos, timeSinceLastUpdate/ previousTimeSinceLastUpdate);
                        Vector3 lerpRot = Vector3.Lerp(curRot, targetRot, 10f * deltaTime);
                        float moveLength =
                            (new Vector3(allEntitiesCache[idx].data.posX, 0, allEntitiesCache[idx].data.posZ) - new Vector3(lerpPos.X, 0, lerpPos.Z)).Length();

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
                            if (allEntitiesCache[idx].entitySpeed / 5f < 0.1f)
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
                                    .Update(deltaTime, allEntitiesCache[idx].entitySpeed / 5f, 0);
                            }
                        }
                        else
                        {
                            allEntitiesCache[idx].animState.Update(deltaTime, 0, 1);
                        }
                        
                    }
                    else if(idxInPreviousData==-1&& idx != -1)
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
                        allEntitiesCache[idx].entitySpeed =0f;
                        if (allEntitiesCache[idx].data.isEntityDying == false)
                        {
                            allEntitiesCache[idx].animState.Update(deltaTime,0f, 0);
                        }
                        else
                        {
                            allEntitiesCache[idx].animState.Update(deltaTime, 0, 1);
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
