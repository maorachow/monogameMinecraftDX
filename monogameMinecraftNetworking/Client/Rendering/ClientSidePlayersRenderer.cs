using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class ClientSidePlayersRenderer:IGBufferDrawableRenderer
    {
        public static Model playerModel;
        public Texture2D playerTex;
        public IGamePlayer curGamePlayer;
        public string curUserName;
        public Effect gBufferEffect;
        public object allUsersCacheLock=new object();
        public GraphicsDevice device;
        public SpriteBatch spriteBatch;
        public SpriteFont spriteFont;
        public List<(UserData data, AnimationBlend animState)> allUsersCache;
        public List<UserData> latestAllUserDatas;
        public IMultiplayerClient client;
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
        public ClientSidePlayersRenderer(Model playerModel, Effect gBufferEffect, IGamePlayer gamePlayer,
            Texture2D playerTex,IMultiplayerClient client, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            this.curGamePlayer = gamePlayer;
            curUserName = client.gamePlayer.playerName;
            this.playerTex = playerTex;
            this.gBufferEffect = gBufferEffect;
            ClientSidePlayersRenderer.playerModel = playerModel;
            this.client = client;
            client.allUsersUpdatedAction += Update;
            allUsersCache = new List<(UserData data, AnimationBlend animState)>();
            latestAllUserDatas = new List<UserData>();
            this.device = device;
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
        }

        public void Update()
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

                        Vector3 lerpPos = Vector3.Lerp(curPos, targetPos, 10f * deltaTime);
                        Vector3 lerpRot = Vector3.Lerp(curRot, targetRot, 10f * deltaTime);
                        float moveLength =
                            (new Vector3(curPos.X, 0, curPos.Z) - new Vector3(targetPos.X, 0, targetPos.Z)).Length();
                        allUsersCache[idx].data.posX=lerpPos.X;
                        allUsersCache[idx].data.posY = lerpPos.Y;
                        allUsersCache[idx].data.posZ = lerpPos.Z;
                        allUsersCache[idx].data.rotX = lerpRot.X;
                        allUsersCache[idx].data.rotY = lerpRot.Y;
                        allUsersCache[idx].data.rotZ = lerpRot.Z;
                       // Debug.WriteLine("speed:"+ (moveLength / deltaTime));
                        allUsersCache[idx].animState.Update(deltaTime,(moveLength));
                    }
                }
            }
          
        }

        public void DrawGBuffer()
        {
            BoundingFrustum frustum = new BoundingFrustum(curGamePlayer.cam.viewMatrix * curGamePlayer.cam.projectionMatrix);
            foreach (var entity in allUsersCache)
            {
                if (entity.data.userName == curUserName)
                {
                    continue;
                }
                   
                      
                            gBufferEffect.Parameters["TextureE"].SetValue(playerTex);
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

        public void DrawPlayerNames()
        {
            float screenWidth = device.PresentationParameters.BackBufferWidth;
            float screenHeight = device.PresentationParameters.BackBufferHeight;
            spriteBatch.Begin(blendState:BlendState.AlphaBlend,samplerState:SamplerState.PointClamp);
            foreach (var entity in allUsersCache)
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
                float distance = MathF.Max(-positionVS.Z, 1f);
                float scale = (screenWidth/UIElement.ScreenRectInital.Width)*10f ;
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
              Vector2   textSize = (spriteFont.MeasureString(entity.data.userName) / 2f)* (scale / distance);
                spriteBatch.DrawString(spriteFont,entity.data.userName,pixelPos-textSize,Color.White,0f,new Vector2(0f,0f),new Vector2(scale/ distance),SpriteEffects.None,1);

            }
            spriteBatch.End();
        }
    }
}
