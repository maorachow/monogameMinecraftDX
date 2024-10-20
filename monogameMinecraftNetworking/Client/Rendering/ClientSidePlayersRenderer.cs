﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftNetworking.Data;
using monogameMinecraftShared.Animations;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Updateables;
using static System.Net.Mime.MediaTypeNames;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector4 = Microsoft.Xna.Framework.Vector4;

namespace monogameMinecraftNetworking.Client.Rendering
{
    public class ClientSidePlayersRenderer:IEntityRenderer, IPostRenderingRenderer, IShadowDrawableRenderer, IGBufferDrawableRenderer
    {
        [Obsolete]
        public static Model playerModel; 
        [Obsolete]
        public Texture2D playerTex;
        public IGamePlayer curGamePlayer;
        public string curUserName;
        public Effect gBufferEffect;
        public object allUsersCacheLock=new object();
        public GraphicsDevice device;
        public SpriteBatch spriteBatch;
        public SpriteFont spriteFont;
        // public List<(UserData data, AnimationBlend animState)> allUsersCache;
        //     public List<UserData> latestAllUserDatas;
        //   public IMultiplayerClient client;
        [Obsolete]
        public static Animation playerAnim = new Animation(new List<AnimationStep> {

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "leftLeg",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "leftArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) }
            }, 0.5f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {
                { "rightLeg", new AnimationTransformation(new Vector3(0f, 0.0f, 0f), new Vector3(0f, 75f, 0f),  new Vector3(1f, 1f, 1f)) },
                { "leftLeg", new AnimationTransformation(new Vector3(0f,0.0f, 0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, -75f, 0f), new Vector3(1f, 1f, 1f)) },
                { "leftArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 75f, 0f), new Vector3(1f, 1f, 1f)) }
            }, 0.5f)
            
        }, true);
        [Obsolete]
        public static Animation playerAttackAnim = new Animation(new List<AnimationStep>
        {
           
          
           
          
           

            new AnimationStep(new Dictionary<string, AnimationTransformation> {

                
                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)) },
               
            }, 0.05f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {


                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(-20f,30f , 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.05f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {


                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(-40f,110f , 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.05f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {


                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3( 50f,80f, 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.05f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {


                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3( 60f,30f, 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.05f),
            new AnimationStep(new Dictionary<string, AnimationTransformation> {


                { "rightArm",new AnimationTransformation(new Vector3(0f,0f,0f),new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)) },

            }, 0.05f),

        }, true);
        public ClientGameBase game;
        public ClientSidePlayersRenderer(Effect gBufferEffect, IGamePlayer gamePlayer,
            IMultiplayerClient client, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont spriteFont,ClientGameBase game)
        {
            this.curGamePlayer = gamePlayer;
            curUserName = client.gamePlayer.playerName;
         //   this.playerTex = playerTex;
            this.gBufferEffect = gBufferEffect;
           // ClientSidePlayersRenderer.playerModel = playerModel;
            //this.client = client;
         //   client.allUsersUpdatedAction += Update;
           // allUsersCache = new List<(UserData data, AnimationBlend animState)>();
          //  latestAllUserDatas = new List<UserData>();
            this.device = device;
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
            this.game= game;
        }

        /*   public void Update()
           {
               lock (allUsersCacheLock)
               {
              //     Debug.WriteLine("update user");
               latestAllUserDatas = client.allUserDatas;
               foreach (var item1 in latestAllUserDatas)
               {
                   if (allUsersCache.FindIndex((item) => { return item.data.userName == item1.userName; }) == -1)
                   {
                       allUsersCache.Add(new ValueTuple<UserData, AnimationBlend>(item1,new AnimationBlend(new AnimationState[]{new AnimationState(playerAnim,playerModel)},playerModel)));
                   }
               }

               for (int i=0;i< allUsersCache.Count;i++)
               {
                   ValueTuple < UserData, AnimationBlend > item2 = allUsersCache[i];
                   if (latestAllUserDatas.FindIndex((item) => { return item.userName == item2.Item1.userName; }) == -1)
                   {
                       allUsersCache.RemoveAt(i);
                       i--;
                   }
               }
               }

           }

           public void FrameUpdate(float deltaTime)
           {
               lock (allUsersCacheLock)
               {
                   foreach (var item1 in latestAllUserDatas)
                   {
                       int idx = allUsersCache.FindIndex((item) => { return item.data.userName == item1.userName; });
                       if ( idx!= -1)
                       {

                           Vector3 curPos = new Vector3(allUsersCache[idx].data.posX, allUsersCache[idx].data.posY,
                               allUsersCache[idx].data.posZ);
                           Vector3 targetPos = new Vector3(item1.posX, item1.posY,
                               item1.posZ);
                           Vector3 curRot= new Vector3(allUsersCache[idx].data.rotX, allUsersCache[idx].data.rotY,
                               allUsersCache[idx].data.rotZ);
                           Vector3 targetRot = new Vector3(item1.rotX, item1.rotY,
                               item1.rotZ);

                           Vector3 lerpPos = Vector3.Lerp(curPos, targetPos, 1f * deltaTime);
                           Vector3 lerpRot = Vector3.Lerp(curRot, targetRot, 1f * deltaTime);
                           float moveLength =
                               (new Vector3(curPos.X, 0, curPos.Z) - new Vector3(targetPos.X, 0, targetPos.Z)).Length();
                           allUsersCache[idx].data.posX=lerpPos.X;
                           allUsersCache[idx].data.posY = lerpPos.Y;
                           allUsersCache[idx].data.posZ = lerpPos.Z;
                           allUsersCache[idx].data.rotX = lerpRot.X;
                           allUsersCache[idx].data.rotY = lerpRot.Y;
                           allUsersCache[idx].data.rotZ = lerpRot.Z;
                          // Debug.WriteLine("speed:"+ (moveLength / deltaTime));

                          if ((moveLength / deltaTime) / 5f < 0.1f)
                          {
                              float lerpValue =
                                  MathHelper.Lerp(
                                      allUsersCache[idx].animState.animationStates[0].elapsedTimeInStep, 0.25f,
                                      deltaTime * 10f);
                              float animationUpdateDelta = lerpValue - allUsersCache[idx].animState
                                  .animationStates[0].elapsedTimeInStep;
                              allUsersCache[idx].animState.Update(animationUpdateDelta, 1);
                          }
                          else
                          {
                              Debug.WriteLine("player speed:"+(moveLength / deltaTime) / 5f);
                              allUsersCache[idx].animState.Update(deltaTime, (moveLength / deltaTime) / 5f);
                          }

                       }
                   }
               }

           }*/
        public void DrawLowDefForward(Effect forwardEffect)
        {
            throw new NotImplementedException();//mobile multiplayer not supported
        }
        public void DrawGBuffer(Effect gBufferEffect1)
        {
            BoundingFrustum frustum = new BoundingFrustum(curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
            foreach (var entity in game.clientSidePlayersManager. allUsersCache)
            {
                if (entity.data.userName == curUserName)
                {
                    continue;
                }

                if (entity.data.curWorldID != ClientSideVoxelWorld.singleInstance.worldID)
                {
                    continue;
                }

              //  gBufferEffect1.Parameters["TextureE"].SetValue(playerTex);
                            //    DrawZombie(entity,gBufferShader);
                            Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY, entity.data.posZ));
                            Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
                            {
                                {"head", Matrix.CreateFromYawPitchRoll(-MathHelper.ToRadians(entity.data.rotX), MathHelper.ToRadians(entity.data.rotY) ,0) },
                                {"body", Matrix.CreateFromYawPitchRoll(-MathHelper.ToRadians(entity.data.rotX),0,0)}
                            };

                            entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, gBufferEffect1, optionalParams, () =>
                            {
                                gBufferEffect1.Parameters["DiffuseColor"]?.SetValue(Color.White.ToVector3());
                            });
                          
                    
                

            }
        }


        public void DrawGBuffer()
        {
            BoundingFrustum frustum = new BoundingFrustum(curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
            foreach (var entity in game.clientSidePlayersManager.allUsersCache)
            {
                if (entity.data.userName == curUserName)
                {
                    continue;
                }

                if (entity.data.curWorldID != ClientSideVoxelWorld.singleInstance.worldID)
                {
                    continue;
                }

           //     gBufferEffect.Parameters["TextureE"].SetValue(playerTex);
                //    DrawZombie(entity,gBufferShader);
                Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY, entity.data.posZ));
                Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
                {
                    {"head", Matrix.CreateFromYawPitchRoll(-MathHelper.ToRadians(entity.data.rotX), MathHelper.ToRadians(entity.data.rotY) ,0) },
                    {"body", Matrix.CreateFromYawPitchRoll(-MathHelper.ToRadians(entity.data.rotX),0,0)}
                };

                entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, gBufferEffect, optionalParams, () =>
                {
                    gBufferEffect.Parameters["DiffuseColor"]?.SetValue(Color.White.ToVector3());
                });




            }
        }

        public void DrawPostRendering()
        {
            DrawPlayerNames();
        }

        private static readonly float playerNameTextWidth = 0.2f;
        public void DrawPlayerNames()
        {
            float screenWidth = device.PresentationParameters.BackBufferWidth;
            float screenHeight = device.PresentationParameters.BackBufferHeight;
            spriteBatch.Begin(blendState:BlendState.AlphaBlend,samplerState:SamplerState.PointClamp);
            foreach (var entity in game.clientSidePlayersManager.allUsersCache)
            {

                if (entity.data.userName == curUserName)
                {
                    continue;
                }

                if (entity.data.curWorldID != ClientSideVoxelWorld.singleInstance.worldID)
                {
                    continue;
                }
                Vector4 positionWS = new Vector4(entity.data.posX, entity.data.posY+1.8f, entity.data.posZ, 1);
                Vector4 positionVS= Vector4.Transform(positionWS, curGamePlayer.cam.viewMatrix);
                float distance = MathF.Max(-positionVS.Z, 0.05f);
                float scale =1;
                Vector4 projectionPos = Vector4.Transform(positionWS, curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
                Vector4 screenSpacePos = new Vector4(projectionPos.X / projectionPos.W, projectionPos.Y / projectionPos.W, projectionPos.Z / projectionPos.W, projectionPos.W / projectionPos.W);
                screenSpacePos.Y = -screenSpacePos.Y;
                screenSpacePos.X *= 0.5f;
                screenSpacePos.Y *= 0.5f;
                screenSpacePos += new Vector4(0.5f, 0.5f, 0f, 0f);

                if (screenSpacePos.Z > 1f)
                {
                    screenSpacePos = new Vector4(-10f, -10f, 0f, 0f);
                }
                Vector2 pixelPos= new Vector2(screenSpacePos.X*screenWidth, screenSpacePos.Y*screenHeight);
                Vector2 textSizeMeasured = spriteFont.MeasureString(entity.data.userName);
                float textScale = playerNameTextWidth * screenWidth / (MathF.Max(textSizeMeasured.X, textSizeMeasured.Y) );
              Vector2   textSize = (textSizeMeasured / 2f)* (scale / distance) * textScale;
                spriteBatch.DrawString(spriteFont,entity.data.userName,pixelPos-textSize,Color.White,0f,new Vector2(0f,0f),new Vector2(scale/ distance) * textScale, SpriteEffects.None,1);

            }
            spriteBatch.End();
        }



        public void DrawPlayerShadow(ClientSidePlayersCacheObject entity, Matrix lightSpaceMat, Effect shadowMapShader)
        {

            Matrix world = Matrix.CreateTranslation(new Vector3(entity.data.posX, entity.data.posY, entity.data.posZ));
            //   zombieModel.Bones["head"].Transform = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.rotationY), -MathHelper.ToRadians(entity.rotationX), 0) * zombieModelRef.Bones["head"].Transform;
            //      zombieModel.Bones["body"].Transform = Matrix.CreateFromQuaternion(entity.bodyQuat) * zombieModelRef.Bones["body"].Transform;
            //   zombieModel.Bones["rightLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["rightLeg"].Transform;
            //   zombieModel.Bones["leftLeg"].Transform = Matrix.CreateFromYawPitchRoll(0, -MathHelper.ToRadians(MathHelper.Clamp(MathF.Cos(entity.entityLifetime * 6f) * entity.curSpeed * 15f, -55f, 55f)), 0) * zombieModelRef.Bones["leftLeg"].Transform;
            //  DrawModelShadow(zombieModel, world, lightSpaceMat,shadowMapShader);
            Dictionary<string, Matrix> optionalParams = new Dictionary<string, Matrix>
            {
                {"head", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY), -MathHelper.ToRadians(entity.data.rotX), 0) },
                {"body", Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(entity.data.rotY),0,0)}
            };

            entity.animState.DrawAnimatedModel(device, world, curGamePlayer.cam.viewMatrix, curGamePlayer.cam.projectionMatrix, shadowMapShader, optionalParams, () => { shadowMapShader.Parameters["LightSpaceMat"].SetValue(lightSpaceMat); });
        }
        public void DrawShadow(Matrix shadowMat, Effect shadowMapShader)
        {
           
            if (game.clientSidePlayersManager == null)
            {
                return;
            }
            BoundingFrustum frustum = new BoundingFrustum(curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
            foreach (var entity in game.clientSidePlayersManager.allUsersCache)
            {
                if (entity.data.userName == curUserName)
                {
                    continue;
                }

                if (entity.data.curWorldID != ClientSideVoxelWorld.singleInstance.worldID)
                {
                    continue;
                }

                DrawPlayerShadow(entity,shadowMat, shadowMapShader);




            }

        }
    }
}
