using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Data;
using monogameMinecraftShared.Animations;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Updateables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftShared.Rendering;
using EntityData = monogameMinecraftNetworking.Data.EntityData;

namespace monogameMinecraftNetworking.Client.Rendering
{
    public class ClientSideEntitiesRenderer:IGBufferDrawableRenderer
    {
        public static Model zombieModel;
        public Texture2D zombieTex;
        public IGamePlayer curGamePlayer;
       
        public Effect gBufferEffect;
        public object allEntitiesCacheLock = new object();
        public GraphicsDevice device;
       
        public List<(EntityData data, AnimationBlend animState)> allEntitiesCache;
        public List<EntityData> lastAllEntitiesDatas;
        public IMultiplayerClient client;
        public static Animation zombieAnim = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "leftLeg",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
             
            }, 0.5f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 75f, 0f),  new Vector3(1f, 1f, 1f)) },
                { "leftLeg", new AnimationTransformation(new Vector3(0f,0.0f, 0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
              
            }, 0.5f)
        }, true);
        public ClientSideEntitiesRenderer(Model zombieModel, Effect gBufferEffect, IGamePlayer gamePlayer,
            Texture2D zombieTex, IMultiplayerClient client, GraphicsDevice device)
        {
            this.curGamePlayer = gamePlayer;
            
            this.zombieTex = zombieTex;
            this.gBufferEffect = gBufferEffect;
            ClientSideEntitiesRenderer.zombieModel = zombieModel;
            this.client = client;
            client.allEntitiesUpdatedAction += Update;
            allEntitiesCache = new List<(EntityData data, AnimationBlend animState)>();
            lastAllEntitiesDatas = new List<EntityData>();
            this.device = device;
            
        }

        public void Update()
        {
            lock (allEntitiesCacheLock)
            {
                    Debug.WriteLine("update entities");
                lastAllEntitiesDatas = client.allEntityDatas;
                foreach (var item1 in lastAllEntitiesDatas)
                {
                    if (allEntitiesCache.FindIndex((item) => { return item.data.entityID == item1.entityID; }) == -1)
                    {
                        allEntitiesCache.Add(new ValueTuple<EntityData, AnimationBlend>(item1, new AnimationBlend(new AnimationState[] { new AnimationState(zombieAnim, zombieModel) }, zombieModel)));
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
                       
                        // Debug.WriteLine("speed:"+ (moveLength / deltaTime));
                        allEntitiesCache[idx].animState.Update(deltaTime, (moveLength));
                    }
                }
            }

        }

        public void DrawGBuffer()
        {
            BoundingFrustum frustum = new BoundingFrustum(curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
            lock (allEntitiesCacheLock)
            {
                foreach (var entity in allEntitiesCache)
                {


                    if (entity.data.entityInWorldID == ClientSideVoxelWorld.singleInstance.worldID)
                    {
                        switch (entity.data.typeid)
                        {
                            case 0:
                                gBufferEffect.Parameters["TextureE"].SetValue(zombieTex);
                                //    DrawZombie(entity,gBufferShader);
                                Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY, entity.data.posZ));
                                Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
                                {
                                    {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY), -MathHelper.ToRadians(entity.data.rotX) ,0) },
                                    {"body", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY),0,0)}
                                };

                                entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, gBufferEffect, optionalParams, () =>
                                {

                                });
                                break;

                        }
                    }
              




                }
            }
           
        }

     
    }
}
