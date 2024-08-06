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
    public class ClientSideEntityManager
    {
        public object allEntitiesCacheLock=new object();
        public List<(EntityData data, AnimationBlend animState)> allEntitiesCache;
        public List<EntityData> lastAllEntitiesDatas;
        public IMultiplayerClient client;
        public ClientSideEntityManager(IMultiplayerClient client)
        {
            this.client= client;
            this.allEntitiesCache = new List<(EntityData data, AnimationBlend animState)>();
            this.lastAllEntitiesDatas = new List<EntityData>();
            client.allEntitiesUpdatedAction += Update;
        }

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
                        allEntitiesCache.Add(new ValueTuple<EntityData, AnimationBlend>(item1, new AnimationBlend(new AnimationState[]
                        {
                            new AnimationState(ClientSideEntitiesRenderer. zombieAnim, ClientSideEntitiesRenderer.zombieModel),

                            new AnimationState(ClientSideEntitiesRenderer. entityDieAnim, ClientSideEntitiesRenderer.zombieModel)
                        }, ClientSideEntitiesRenderer.zombieModel)));
                    }
                }

                for (int i = 0; i < allEntitiesCache.Count; i++)
                {
                    ValueTuple<EntityData, AnimationBlend> item2 = allEntitiesCache[i];
                    if (lastAllEntitiesDatas.FindIndex((item) => { return item.entityID == item2.Item1.entityID; }) == -1)
                    {
                        allEntitiesCache.RemoveAt(i);
                        i--;
                    }
                }
            }

        }

        public void FrameUpdate(float deltaTime)
        {
            lock (allEntitiesCacheLock)
            {
                foreach (var item1 in lastAllEntitiesDatas)
                {
                    int idx = allEntitiesCache.FindIndex((item) => { return item.data.entityID == item1.entityID; });
                    if (idx != -1)
                    {

                        Vector3 curPos = new Vector3(allEntitiesCache[idx].data.posX, allEntitiesCache[idx].data.posY,
                            allEntitiesCache[idx].data.posZ);
                        Vector3 targetPos = new Vector3(item1.posX, item1.posY,
                            item1.posZ);
                        Vector3 curRot = new Vector3(allEntitiesCache[idx].data.rotX, allEntitiesCache[idx].data.rotY,
                            allEntitiesCache[idx].data.rotZ);
                        Vector3 targetRot = new Vector3(item1.rotX, item1.rotY,
                            item1.rotZ);

                        Vector3 lerpPos = Vector3.Lerp(curPos, targetPos, 10f * deltaTime);
                        Vector3 lerpRot = Vector3.Lerp(curRot, targetRot, 10f * deltaTime);
                        float moveLength =
                            (new Vector3(curPos.X, 0, curPos.Z) - new Vector3(targetPos.X, 0, targetPos.Z)).Length();

                        allEntitiesCache[idx].data.posX = lerpPos.X;
                        allEntitiesCache[idx].data.posY = lerpPos.Y;
                        allEntitiesCache[idx].data.posZ = lerpPos.Z;
                        allEntitiesCache[idx].data.rotX = targetRot.X;
                        allEntitiesCache[idx].data.rotY = targetRot.Y;
                        allEntitiesCache[idx].data.rotZ = targetRot.Z;
                        allEntitiesCache[idx].data.isEntityHurt = item1.isEntityHurt;
                        allEntitiesCache[idx].data.isEntityDying = item1.isEntityDying;
                        // Debug.WriteLine("speed:"+ (moveLength / deltaTime));
                        if (allEntitiesCache[idx].data.isEntityDying == false)
                        {
                            allEntitiesCache[idx].animState.Update(deltaTime, (moveLength), 0);
                        }
                        else
                        {
                            allEntitiesCache[idx].animState.Update(deltaTime, 0, 1);
                        }
                        
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
