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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftShared.Rendering;
using EntityData = monogameMinecraftNetworking.Data.EntityData;

namespace monogameMinecraftNetworking.Client.Rendering
{
    public class ClientSideEntitiesRenderer:IEntityRenderer
    {
        public static Model zombieModel;
        public static Model pigModel;
        public static Texture2D zombieTex;
        public static Texture2D pigTex;
        public IGamePlayer curGamePlayer;
       
        public Effect gBufferEffect;
       
        public GraphicsDevice device;
       
    //    public List<(EntityData data, AnimationBlend animState)> allEntitiesCache;
     //   public List<EntityData> lastAllEntitiesDatas;
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

        public static Animation pigWalkingAnim = new Animation(new List<AnimationStep> {

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
        }, true);
        public static Animation entityDieAnim = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "waist", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f,0f, 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.4f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "waist", new AnimationTransformation(new Vector3(0f, -0.75f, 0f), new Vector3(0f,0f, -90f), new Vector3(1f, 1f, 1f)) },
            }, 0.1f)
        }, false);
        public static Animation entityDieAnimRoot = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "RootNode", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f,0f, 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.4f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "RootNode", new AnimationTransformation(new Vector3(0f, 0f, 0f), new Vector3(0f,0f, -90f), new Vector3(1f, 1f, 1f)) },
            }, 0.1f)
        }, false);
        public ClientGameBase game;
        public ClientSideEntitiesRenderer(Model zombieModel,Model pigModel, Effect gBufferEffect, IGamePlayer gamePlayer,
            Texture2D zombieTex, Texture2D pigTex, IMultiplayerClient client, GraphicsDevice device,ClientGameBase game)
        {
            this.curGamePlayer = gamePlayer;

            ClientSideEntitiesRenderer.zombieTex = zombieTex;
            ClientSideEntitiesRenderer.pigTex = pigTex;
            this.gBufferEffect = gBufferEffect;
            ClientSideEntitiesRenderer.zombieModel = zombieModel;
            ClientSideEntitiesRenderer.pigModel = pigModel;
            this.client = client;
        //    client.allEntitiesUpdatedAction += Update;
        //    allEntitiesCache = new List<(EntityData data, AnimationBlend animState)>();
         //   lastAllEntitiesDatas = new List<EntityData>();
            this.device = device;
            this.game = game;
        }

       /* public void Update()
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

        }*/

    /*    public void FrameUpdate(float deltaTime)
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

        }*/

        public void DrawGBuffer(Effect gBufferEffect1)
        {
            BoundingFrustum frustum = new BoundingFrustum(curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
            if (game.clientSideEntityManager == null)
            {
                return;
            }
            lock (game.clientSideEntityManager.allEntitiesCacheLock)
            {

                foreach (var entity in game.clientSideEntityManager. allEntitiesCache)
                {


                    if (entity.data.entityInWorldID == ClientSideVoxelWorld.singleInstance.worldID)
                    {
                        switch (entity.data.typeid)
                        {
                            case 0:
                                gBufferEffect1.Parameters["TextureE"].SetValue(zombieTex);
                                //    DrawZombie(entity,gBufferShader);
                                Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY + 0.0005f, entity.data.posZ));


                                Quaternion bodyQuat = Quaternion.Identity;
                                if (entity.entityOptionalData is Float4Data)
                                {
                                    Float4Data? data1 = (entity.entityOptionalData as Float4Data?);
                                    if (data1 != null)
                                    {
                                        bodyQuat = new Quaternion(data1.Value.x, data1.Value.y, data1.Value.z, data1.Value.w);
                                    }
                                }
                                Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
                                {
                                    {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY), -MathHelper.ToRadians(entity.data.rotX) ,0) },
                                    {"body", Matrix.CreateFromQuaternion(bodyQuat)}
                                };

                                entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, gBufferEffect1, optionalParams, () =>
                                {
                                    if (entity.data.isEntityHurt)
                                    {


                                        gBufferEffect1.Parameters["DiffuseColor"]?.SetValue(Color.Red.ToVector3());

                                    }
                                    else
                                    {

                                        gBufferEffect1.Parameters["DiffuseColor"]?.SetValue(Color.White.ToVector3());

                                    }
                                });
                                break;
                            case 1:
                                gBufferEffect1.Parameters["TextureE"].SetValue(pigTex);
                                //    DrawZombie(entity,gBufferShader);
                                Matrix world1 = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY + 0.0005f, entity.data.posZ));


                                Vector3 headRot = new Vector3();
                                if (entity.entityOptionalData is Float3Data)
                                {
                                    Float3Data? data1 = (entity.entityOptionalData as Float3Data?);
                                    if (data1 != null)
                                    {
                                        headRot = new Vector3(data1.Value.x, data1.Value.y, data1.Value.z);
                                    }
                                }
                                Dictionary<string, Matrix> optionalParams1 = new Dictionary<string, Matrix>
                                {
                                    {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(headRot.Y), -MathHelper.ToRadians(headRot.X) ,0) },
                                    {"body", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY),0 ,0)}
                                };

                                entity.animState.DrawAnimatedModel(device, world1, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, gBufferEffect, optionalParams1, () =>
                                {
                                    if (entity.data.isEntityHurt)
                                    {


                                        gBufferEffect.Parameters["DiffuseColor"]?.SetValue(Color.Red.ToVector3());

                                    }
                                    else
                                    {

                                        gBufferEffect.Parameters["DiffuseColor"]?.SetValue(Color.White.ToVector3());

                                    }
                                });
                                break;
                        }
                    }
              




                }
            }
           
        }

        public void DrawLowDefForward(Effect forwardEffect)
        {
            throw new NotImplementedException();//mobile multiplayer not supported
        }
        public void DrawZombieShadow(ClientSideEntityCacheObject entity, Matrix lightSpaceMat, Effect shadowMapShader)
        {

            Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY + 0.0005f, entity.data.posZ));
            //   zombieModel.Bones["head"].Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), -MathHelper.ToRadians(entity.rotationX), 0) * zombieModelRef.Bones["head"].Transform;
            //      zombieModel.Bones["body"].Transform = Matrix.CreateFromQuaternion(entity.bodyQuat) * zombieModelRef.Bones["body"].Transform;
            //   zombieModel.Bones["rightLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["rightLeg"].Transform;
            //   zombieModel.Bones["leftLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, -MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["leftLeg"].Transform;
            //  DrawModelShadow(zombieModel, world, lightSpaceMat,shadowMapShader);
           Quaternion bodyQuat=Quaternion.Identity;
           if (entity.entityOptionalData is Float4Data)
           {
               Float4Data? data1 = (entity.entityOptionalData as Float4Data?);
               if (data1 != null)
               {
                   bodyQuat = new Quaternion(data1.Value.x, data1.Value.y, data1.Value.z, data1.Value.w);
               }
           }
            Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
            {
                {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY), -MathHelper.ToRadians(entity.data.rotX), 0) },
                {"body", Matrix.CreateFromQuaternion(bodyQuat)}
            };

            entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, shadowMapShader, optionalParams, () => { shadowMapShader.Parameters["LightSpaceMat"].SetValue(lightSpaceMat); });
        }
        public void DrawPigShadow(ClientSideEntityCacheObject entity, Matrix lightSpaceMat, Effect shadowMapShader)
        {

            Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY+0.0005f, entity.data.posZ));
            //   zombieModel.Bones["head"].Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), -MathHelper.ToRadians(entity.rotationX), 0) * zombieModelRef.Bones["head"].Transform;
            //      zombieModel.Bones["body"].Transform = Matrix.CreateFromQuaternion(entity.bodyQuat) * zombieModelRef.Bones["body"].Transform;
            //   zombieModel.Bones["rightLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["rightLeg"].Transform;
            //   zombieModel.Bones["leftLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, -MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["leftLeg"].Transform;
            //  DrawModelShadow(zombieModel, world, lightSpaceMat,shadowMapShader);
            Vector3 headRot = new Vector3();
            if (entity.entityOptionalData is Float3Data)
            {
                Float3Data? data1 = (entity.entityOptionalData as Float3Data?);
                if (data1 != null)
                {
                    headRot = new Vector3(data1.Value.x, data1.Value.y, data1.Value.z);
                }
            }
            Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
            {
                {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(headRot.Y), -MathHelper.ToRadians(headRot.X) ,0) },
                {"body", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY),0 ,0)}
            };
           

            entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, shadowMapShader, optionalParams, () => { shadowMapShader.Parameters["LightSpaceMat"].SetValue(lightSpaceMat); });
        }
        public void DrawShadow(Matrix shadowMat,Effect shadowMapShader)
        {
            BoundingFrustum frustum = new BoundingFrustum(curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
            if (game.clientSideEntityManager == null)
            {
                return;
            }
            lock (game.clientSideEntityManager.allEntitiesCacheLock)
            {

                foreach (var entity in game.clientSideEntityManager.allEntitiesCache)
                {


                    if (entity.data.entityInWorldID == ClientSideVoxelWorld.singleInstance.worldID)
                    {
                        switch (entity.data.typeid)
                        {
                            case 0:
                                /* shadowMapShader.Parameters["TextureE"].SetValue(zombieTex);
                                 //    DrawZombie(entity,gBufferShader);
                                 Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY, entity.data.posZ));
                                 Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
                                 {
                                     {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY), -MathHelper.ToRadians(entity.data.rotX) ,0) },
                                     {"body", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY),0,0)}
                                 };

                                 entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, gBufferEffect, optionalParams, () =>
                                 {
                                     if (entity.data.isEntityHurt)
                                     {


                                         gBufferEffect.Parameters["DiffuseColor"]?.SetValue(Color.Red.ToVector3());

                                     }
                                     else
                                     {

                                         gBufferEffect.Parameters["DiffuseColor"]?.SetValue(Color.White.ToVector3());

                                     }
                                 });*/
                                DrawZombieShadow(entity, shadowMat, shadowMapShader);
                                break;
                            case 1:
                                DrawPigShadow(entity, shadowMat, shadowMapShader);
                                break;


                        }
                    }





                }
            }

        }

    }
}
