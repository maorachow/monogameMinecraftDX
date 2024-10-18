using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System.Numerics;
using monogameMinecraftShared.World;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Asset;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Input.Touch;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Pathfinding;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.UI;

using monogameMinecraftShared.Physics;
using monogameMinecraftShared;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Updateables;

 
namespace monogameMinecraftAndroid
{
  
    public class MinecraftGame : MinecraftGameBase
    {
        private GraphicsDeviceManager _graphics;
     
        public AlphaTestEffect chunkNSEffect;
    
        public RandomTextureGenerator randomTextureGenerator;
     
 
        public MinecraftGame(Android.App.Activity activity)
        {
           
            optionalPlatformDataPath = activity.GetExternalFilesDir("")?.Path;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";
            //    IsMouseVisible = false;
            Window.AllowUserResizing = false;

       //     Window.ClientSizeChanged += OnResize;
                  _graphics.PreparingDeviceSettings += PrepareGraphicsDevice;
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            effectsManager = new LowDefEffectsManager();
            gamePlatformType = GamePlatformType.VeryLowDefMobile;
            gamePlayerR = new GamePlayerReference();
            renderPipelineManager = new VeryLowDefRenderPipelineManager(this, effectsManager);
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
 
        }

        public void PrepareGraphicsDevice(object sender, PreparingDeviceSettingsEventArgs e)
        {
            _graphics.PreferMultiSampling = false;
            _graphics.SynchronizeWithVerticalRetrace = false;

        }
       public override void OnResize(Object sender, EventArgs e)
        {
            //    UIElement.ScreenRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
          
            switch (status)
            {
                case GameStatus.Started:
                  
                    /*      int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
                          int height = GraphicsDevice.PresentationParameters.BackBufferHeight;
                          Debug.WriteLine(width);
                          Debug.WriteLine(height);
                          gBufferRenderer.Resize(width, height, GraphicsDevice);



                          ssaoRenderer.ssaoTarget = new RenderTarget2D(ssaoRenderer.graphicsDevice, width / 2, height / 2, false, SurfaceFormat.Color, DepthFormat.Depth24);
                          volumetricLightRenderer.blendVolumetricMap = new RenderTarget2D(volumetricLightRenderer.device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          volumetricLightRenderer.renderTargetLum = new RenderTarget2D(volumetricLightRenderer.device, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          //      ssrRenderer.renderTargetSSR=new RenderTarget2D(ssrRenderer.graphicsDevice, width,height,false,SurfaceFormat.Vector4, DepthFormat.Depth24);
                          volumetricLightRenderer.lightShaftTarget = new RenderTarget2D(GraphicsDevice, (int)((float)width / 2f), (int)((float)height / 2f), false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          contactShadowRenderer.contactShadowRenderTarget = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);

                          motionVectorRenderer.renderTargetMotionVector = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          deferredShadingRenderer.renderTargetLum = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          deferredShadingRenderer.finalImage = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                          deferredShadingRenderer.renderTargetLumSpec = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          motionBlurRenderer.processedImage = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
                          hiZBufferRenderer.ResizeTarget();
                          ssidRenderer.renderTargetSSID = new RenderTarget2D(GraphicsDevice, width / 2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          ssidRenderer.renderTargetSSIDPrev = new RenderTarget2D(GraphicsDevice, width / 2, height / 2, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          ssrRenderer.renderTargetSSR = new RenderTarget2D(GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          ssrRenderer.renderTargetSSRPrev = new RenderTarget2D(GraphicsDevice, hiZBufferRenderer.hiZBufferTargetMips[0].Width, hiZBufferRenderer.hiZBufferTargetMips[0].Height, false, SurfaceFormat.Vector4, DepthFormat.Depth24);
                          foreach (var processor in customPostProcessors)
                          {
                              processor.processedImage.Dispose();
                              processor.processedImage = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                          }
                          float aspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
                          gamePlayer.cam.aspectRatio = aspectRatio;
                          gamePlayer.cam.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), aspectRatio, 0.1f, 1000f);*/
                    renderPipelineManager.Resize();
                    break;
            }
            //  button.OnResize();



            Debug.WriteLine(GraphicsDevice.Viewport.Width + " " + GraphicsDevice.Viewport.Height);
        }
     //   private static AlertDialog alert;
      //  string resultText = "";
        public override void InitGameplay(object obj)
        {

            if (VoxelWorld.currentWorld.updateWorldThread != null)
            {
                if (VoxelWorld.currentWorld.updateWorldThread.IsAlive)
                {
                    Debug.WriteLine("thread not stopped");
                    return;
                }
            }

         

            GraphicsDevice.PresentationParameters.MultiSampleCount = 0;

            IsMouseVisible = false;
            //   ChunkManager.chunks = new System.Collections.Concurrent.ConcurrentDictionary<Vector2Int, Chunk>();
            //  ChunkManager.chunkDataReadFromDisk = new Dictionary<Vector2Int, ChunkData>();
            Chunk.biomeNoiseGenerator.SetFrequency(0.002f);

            //   ChunkManager.ReadJson();
            GameOptions.ReadOptionsJson();

            /*     
                BlockResourcesManager.WriteDefaultBlockSoundInfo(Directory.GetCurrentDirectory() + "/blocksoundinfodata.json");
                    BlockResourcesManager.WriteDefaultBlockSpritesInfo(Directory.GetCurrentDirectory() + "/blockspritesinfodata.json");*/
            //    BlockResourcesManager.LoadResources(Directory.GetCurrentDirectory() + "/customresourcespack",Content,GraphicsDevice);
         //   BlockResourcesManager.WriteDefaultBlockInfo(Directory.GetCurrentDirectory() + "/blockinfodata.json");

  
           gamePlayerR. gamePlayer = new monogameMinecraftShared.Updateables.GamePlayer(new Vector3(-0.3f, 100, -0.3f), new Vector3(0.3f, 101.8f, 0.3f), this);
            playerInputManager = new PlayerInputManager(gamePlayerR.gamePlayer, true);
           
            //  GamePlayer.ReadPlayerData(gamePlayer, this);
          
            particleManager = new ParticleManager();
            particleManager.Initialize();
            

            /*     updateWorldThread = new Thread(() => ChunkManager.UpdateWorldThread( gamePlayer,this));
                 updateWorldThread.IsBackground = true;
                 updateWorldThread.Start();
                 tryRemoveChunksThread = new Thread(() => ChunkManager.TryDeleteChunksThread( gamePlayer, this));
                 tryRemoveChunksThread.IsBackground = true;
                 tryRemoveChunksThread.Start();*/

            effectsManager.LoadEffects(Content);
            renderPipelineManager.InitRenderPipeline();
            randomTextureGenerator = new RandomTextureGenerator();
            RandomTextureGenerator.instance.GenerateTexture(1024, 1024, GraphicsDevice);

         
          
          
            gameTimeManager = new GameTimeManager(gamePlayerR.gamePlayer);
       

   
            EntityManager.InitEntityList();
            EntityManager.LoadEntitySounds(Content);
           
            EntityManager.ReadEntityData();
            Debug.WriteLine(EntityManager.entityDataReadFromDisk.Count);
            EntityManager.SpawnEntityFromData(this);
            EntityManager.pathfindingManager.drawDebugLines = false;
            isGamePaused = false;
            status = GameStatus.Started;
            VoxelWorld.currentWorld.InitWorld(this);


            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGame]);




        }

        public override void QuitGameplay()
        {
            IsMouseVisible = true;
            GameOptions.SaveOptions(null);
            VoxelWorld.currentWorld.SaveAndQuitWorld(this);

           // monogameMinecraftShared.Updateables. GamePlayer.SavePlayerData(gamePlayerR.gamePlayer);
            /*     foreach(var c in ChunkManager.chunks)
                 {
                 c.Value.Dispose();
                 }*/
            EntityManager.SaveWorldEntityData();
            ChunkHelper.isJsonReadFromDisk = false;
       //     gamePlayerR.gamePlayer.curChunk = null;
            /*     lock(.updateWorldThreadLock)
                 {
                 foreach (var c in ChunkManager.chunks)
                 {

                     c.Value.Dispose();


                 }
                 }*/
          /*  foreach (var world in VoxelWorld.voxelWorlds)
            {
                world.DestroyAllChunks();
            }*/

            //        ChunkManager.chunks.Keys.Clear() ; 


            //      ChunkManager.chunkDataReadFromDisk.Clear();


            //    ChunkManager.chunks =null;
            //   ChunkManager.chunkDataReadFromDisk=new Dictionary<Vector2Int, ChunkData> ();
        //   UIUtility. InitStructureOperationsUI(this, UIUtility. sf);
           EntityManager.StopAllThreads();
       //     EntityManager.InitEntityList();
            ParticleManagerBase.instance.ReleaseResources();
            GC.Collect();



            status = GameStatus.Menu;
            // updateWorldThread.Abort();
            //   tryRemoveChunksThread.Abort();
            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.Menu]);
        }

        public override void GoToSettings(object obj)
        {
            GameOptions.ReadOptionsJson();

            this.status = GameStatus.Settings;

            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.Settings]);
        }

        public override void GoToMenuFromSettings(object obj)
        {
            GameOptions.SaveOptions(null);
            this.status = GameStatus.Menu;
            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.Menu]);
        }


        public override void OpenInventory(object obj)
        {
            isInventoryOpen = !isInventoryOpen;
            if (isInventoryOpen == true)
            {
                uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGameInventoryOpened]);
            //    mouseMovementManager.isMouseLocked = false;
                IsMouseVisible = true;
            }
            else
            {
                uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGame]);
            //    mouseMovementManager.isMouseLocked = true;
                IsMouseVisible = false;
            }
        }
        SpriteFont sf;
        protected override void Initialize()
        {


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //  InitGameplay();
            //    sf = Content.Load<SpriteFont>("defaultfont");
            //   UIUtility.InitGameUI(this);
          
            UIResourcesManager.instance.LoadTextures(this);
            UIResourcesManager.instance.LoadDefaultBlockSpriteResources(this);
            uiStateManager = new UIStateManager(this);
            uiStateManager.ScreenRect = Window.ClientBounds;
            uiStateManager.Initialize();
            uiResizingManager = new UIResizingManager(uiStateManager);
            GameOptions.ReadOptionsJson();
            // Chunk c = new Chunk(new Vector2Int(0,0));
            //  chunkSolidEffect = new Effect();
            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.Menu]);
            EntityResourcesManager.instance.Initialize();
            EntityResourcesManager.instance.LoadAllDefaultDesources(Content);


            UITouchscreenInputHelper.screenRectOffset = new Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y);
            // UIResizingManager.Resize(this);
            uiStateManager.ScreenRect = Window.ClientBounds;
            uiResizingManager.Resize(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {



            //     skyboxTex = Content.Load<TextureCube>("skybox");
            // terrainTex.
            //       Debug.WriteLine(terrainTex.Width + " " + terrainTex.Height);
            //   button = new UIButton(new Vector2(0.1f, 0.1f), 0.3f, 0.2f, terrainTex, new Vector2(0.15f, 0.15f), sf, _spriteBatch,this.Window);
            // chunkRenderer.atlas = terrainTex;

            // TODO: use this.Content to load your game content here
        }
        int lastMouseX;
        int lastMouseY;
        public bool isGamePaused = false;
        public bool isInventoryOpen = false;
        public bool isStructureOperationsUIOpened = false;


        public override void PauseGame(object obj)
        {
            isGamePaused = true;
       
        }
        public override void ResumeGame()
        {
            isGamePaused = false;
           
        }

      
        public override void CloseStructureOperationsUI(object o)
        {
            isStructureOperationsUIOpened = false;
            if (isStructureOperationsUIOpened == true)
            {
                IsMouseVisible = true;
            }
            else
            {
                IsMouseVisible = false;
            }
        }
        float prevFPS = 0f;
        public KeyboardState lastKeyState1;
        //  public Vector3Int debugMapOrigin;

        public TouchCollection prevTouches;
        protected override void Update(GameTime gameTime)
        {
    //        string s = resultText;
       //     Debug.WriteLine("input string received:" + s);
            if (!IsActive) return;
            //  Draw1(gameTime);
            UITouchscreenInputHelper.UpdateTouches();
            AndroidTextInputManager.Update();
            switch (status)
            {
               
            /*    case GameStatus.Menu:
                    foreach (var el in UIElement.menuUIs)
                    {
                        el.Update();
                    }
                    break;


                case GameStatus.Settings:
                    switch (UIElement.settingsUIsPageID)
                    {
                        case 0:
                            foreach (var el in UIElement.settingsUIsPage1)
                            {
                                el.Update();
                            }
                            break;
                        case 1:
                            foreach (var el in UIElement.settingsUIsPage2)
                            {
                                el.Update();
                            }
                            break;
                        default:
                            foreach (var el in UIElement.settingsUIsPage1)
                            {
                                el.Update();
                            }
                            break;
                    }
                    break;*/
                case GameStatus.Started:

                    if (isGamePaused)
                    {

                        if (uiStateManager.curState is not UIStateInGamePaused)
                        {
                            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGamePaused]);
                        }
                        break;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.E)&&!lastKeyState1.IsKeyUp(Keys.E) && !isStructureOperationsUIOpened)
                    {
                        //     status = GameStatus.Quiting;
                        //  QuitGameplay();
                        //  Exit();
                        //   Environment.Exit(0);

                        OpenInventory(null);


                    }
                   /* if (Keyboard.GetState().IsKeyUp(Keys.H) && !lastKeyState1.IsKeyUp(Keys.H)&&!isInventoryOpen)
                    {
                        //     status = GameStatus.Quiting;
                        //  QuitGameplay();
                        //  Exit();
                        //   Environment.Exit(0);

                        isStructureOperationsUIOpened = true;
                        if (isStructureOperationsUIOpened == true)
                        {
                            IsMouseVisible = true;
                        }
                        else
                        {
                            IsMouseVisible = false;
                        }

                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.J) && !lastKeyState1.IsKeyUp(Keys.J))
                    {
                      //  ChunkHelper.FillBlocks(new BlockData[50,50,50],(Vector3Int)gamePlayer.position+ new Vector3Int(-25,-25,-25));
                      VoxelWorld.currentWorld.structureOperationsManager.AddOrReplaceStructure("teststructure",new StructureData(ChunkHelper.GetBlocks((Vector3Int)gamePlayerR.gamePlayer.position + new Vector3Int(-5, -5, -5), 11, 11, 11)));
                    }

                    if (Keyboard.GetState().IsKeyUp(Keys.K) && !lastKeyState1.IsKeyUp(Keys.K))
                    {
                        //  ChunkHelper.FillBlocks(new BlockData[50,50,50],(Vector3Int)gamePlayer.position+ new Vector3Int(-25,-25,-25));
                        VoxelWorld.currentWorld.structureOperationsManager.PlaceStructure((Vector3Int)gamePlayerR.gamePlayer.position + new Vector3Int(-5, -5, -5), "teststructure",false,true,false);
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.P) && !lastKeyState1.IsKeyUp(Keys.P))
                    {
                      
                     /*   if (gamePlayer.curChunk != null)
                        {
                           debugMapOrigin = new Vector3Int(gamePlayer.curChunk.chunkPos.x,
                                (int)gamePlayer.position.Y - 10,
                                gamePlayer.curChunk.chunkPos.y);
                            BlockData[,,] map = ChunkHelper.GetBlocks(
                                debugMapOrigin, Chunk.chunkWidth, 20, Chunk.chunkWidth);


                            WalkablePath path;

                            if (map != null)
                            {
                               // Debug.WriteLine("top:"+(ChunkHelper.GetChunkLandingPoint(gamePlayer.curChunk.chunkPos.x + 14, gamePlayer.curChunk.chunkPos.y + 14)));
                               // Debug.WriteLine("toporigin:" + (ChunkHelper.GetChunkLandingPoint(ChunkHelper.FloatToInt(gamePlayer.position.X), ChunkHelper.FloatToInt(gamePlayer.position.Z))));
                               // Debug.WriteLine("origin:"+ debugMapOrigin.y);
                               // Debug.WriteLine("playerPos:" +( (int)gamePlayer.position.X - gamePlayer.curChunk.chunkPos.x)+" "+( (int)gamePlayer.position.Z - gamePlayer.curChunk.chunkPos.y));
                                path = ThreeDimensionalMapPathfindingUtility.FindPathByBlockData(map,
                                    new Vector3Int(ChunkHelper.FloatToInt(gamePlayer.position.X) - gamePlayer.curChunk.chunkPos.x, (ChunkHelper.GetChunkLandingPoint(ChunkHelper.FloatToInt(gamePlayer.position.X), ChunkHelper.FloatToInt(gamePlayer.position.Z)) )- debugMapOrigin.y,
                                        ChunkHelper.FloatToInt(gamePlayer.position.Z) - gamePlayer.curChunk.chunkPos.y),
                                new Vector3Int(14, (ChunkHelper.GetChunkLandingPoint(gamePlayer.curChunk.chunkPos.x + 14, gamePlayer.curChunk.chunkPos.y + 14)) - debugMapOrigin.y,
                                       14));
                                foreach (var VARIABLE in path.steps)
                                {
                                   // Debug.WriteLine(VARIABLE);
                                }
                                EntityManager.pathfindingManager.curDebuggingPath= path;
                            }
                        }

                        
                       
                    EntityManager.pathfindingManager.drawDebugLines=!EntityManager.pathfindingManager.drawDebugLines;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.N) && !lastKeyState1.IsKeyUp(Keys.N))
                    {
                    EntityManager.SpawnNewEntity(gamePlayerR.gamePlayer.position,0,0,0,0,this);
                    }*/
                    /*      if (Keyboard.GetState().IsKeyUp(Keys.K) && !lastKeyState1.IsKeyUp(Keys.K))
                          {
                              //  ChunkHelper.FillBlocks(new BlockData[50,50,50],(Vector3Int)gamePlayer.position+ new Vector3Int(-25,-25,-25));
                              ChunkHelper.FillBlocks(StructureManager.LoadStructure(Directory.GetCurrentDirectory() + "/defaultstructure.bin"), (Vector3Int)gamePlayer.position + new Vector3Int(-5, -5, -5),BlockFillMode.ReplaceAir); ;
                          }*/
                    lastKeyState1 = Keyboard.GetState();
                    //       Debug.WriteLine(isInventoryOpen);
                    lastKeyState1 = Keyboard.GetState();
                    //       Debug.WriteLine(isInventoryOpen);




                    if (gamePlayerR.gamePlayer is GamePlayer player1)
                    {
                        player1.UpdatePlayer(this, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }


                    //    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                    /*  foreach (var el in UIElement.inGameUIs)
                      {
                          el.Update();
                      }*/

                    if (VoxelWorld.currentWorld.isThreadsStopping == false)
                    {
                        VoxelWorld.currentWorld.FrameUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);
                    }

                    gameTimeManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                    EntityManager.UpdateAllEntity((float)gameTime.ElapsedGameTime.TotalSeconds);
                    EntityManager.FixedUpdateAllEntity((float)gameTime.ElapsedGameTime.TotalSeconds);
                    ParticleManagerBase.instance.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    EntityManager.TrySpawnNewZombie(this, (float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (isInventoryOpen || isStructureOperationsUIOpened)
                    {

                        break;
                    }

                    /*     if (isStructureOperationsUIOpened)
                         {
                             switch (UIElement.structureOperationsUIsPageID)
                             {
                                 case 0:
                                     foreach (var el in UIElement.structureOperationsSavingUIs)
                                     {
                                         el.Update();
                                     }

                                     break;
                                 case 1:
                                     foreach (var el in UIElement.structureOperationsPlacingUIs)
                                     {
                                         el.Update();
                                     }

                                     break;
                                 default:
                                     foreach (var el in UIElement.structureOperationsSavingUIs)
                                     {
                                         el.Update();
                                     }

                                     break;
                             }

                             break;
                         }*/
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {

                        /*   mouseMovementManager.isMouseLocked = false;
                           isGamePaused = true;
                           IsMouseVisible = true;*/
                        PauseGame(null);
                    }




                    //      GlobalMaterialParamsManager.instance.Update(gameTime);
                    gameposition = gamePlayerR.gamePlayer.position;
                    playerInputManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                  
                    break;
            }
            uiStateManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            prevTouches = UITouchscreenInputHelper.allTouches;
            //     Debug.WriteLine(gamePlayer.position);
            //     Debug.WriteLine(gamePlayer.cam.Pitch+" "+ gamePlayer.cam.Yaw);
            //    Debug.WriteLine(gamePlayer.cam.position + " " + gamePlayer.cam.front+" "+gamePlayer.cam.up);
            base.Update(gameTime);

        }
     /*   void ProcessPlayerMouseInput(bool isTouchEnabled=false)
        {
            var mState = Mouse.GetState();
            gamePlayer.cam.ProcessMouseMovement(mState.X - lastMouseX, lastMouseY - mState.Y);
            lastMouseY = mState.Y;
            lastMouseX = mState.X;

            if (isTouchEnabled)
            {
                if (prevTouches.Count > 0 && UIElement.allTouches.Count > 0)
                {
                    lastMouseX = (int)(prevTouches[0].Position.X - UIElement.allTouches[0].Position.X);
                    lastMouseY = (int)(prevTouches[0].Position.Y - UIElement.allTouches[0].Position.Y);
                }
            }
        }
        MouseState lastMouseState;
        KeyboardState lastKeyboardState;
        void ProcessPlayerKeyboardInput(GameTime gameTime)
        {

            var kState = Keyboard.GetState();
            var mState = Mouse.GetState();
            Vector3 playerVec = new Vector3(0f, 0f, 0f);

            if (kState.IsKeyDown(Keys.T) && !lastKeyboardState.IsKeyDown(Keys.T))
            {
                //   gamePlayer.PlayerTryTeleportToEnderWorld(this,);


            }
            if (kState.IsKeyDown(Keys.W))
            {
                playerVec.Z = 1f;
            }

            if (kState.IsKeyDown(Keys.S))
            {
                playerVec.Z = -1f;
            }

            if (kState.IsKeyDown(Keys.A))
            {
                playerVec.X = -1f;
            }

            if (kState.IsKeyDown(Keys.D))
            {
                playerVec.X = 1f;
            }
            if (kState.IsKeyDown(Keys.Space))
            {
                playerVec.Y = 1f;
            }
            if (kState.IsKeyDown(Keys.LeftShift))
            {
                playerVec.Y = -1f;
            }

       //     gamePlayer.ProcessPlayerInputs(playerVec, (float)gameTime.ElapsedGameTime.TotalSeconds, kState, mState, lastMouseState);

            lastKeyboardState = kState;
            lastMouseState = mState;

        }*/
        RasterizerState rasterizerState = new RasterizerState();
        RasterizerState rasterizerState1 = new RasterizerState{DepthClipEnable=false};

        /*  public void RenderWorld(GameTime gameTime, SpriteBatch sb)
          {
              shadowRenderer.UpdateLightMatrices(gamePlayer);
              GraphicsDevice.DepthStencilState = DepthStencilState.Default;

              //  GraphicsDevice.RasterizerState = rasterizerState;
              shadowRenderer.RenderShadow(gamePlayer);
             gBufferRenderer.Draw();
              ssaoRenderer.Draw();
            hiZBufferRenderer.Draw();
              contactShadowRenderer.Draw();
             deferredShadingRenderer.Draw(gamePlayer);
               volumetricLightRenderer.Draw();
             motionVectorRenderer.Draw();
                shadowRenderer.UpdateLightMatrices(gamePlayer);

                    shadowRenderer.UpdateLightMatrices(gamePlayer);
              ssrRenderer.Draw(gameTime);
              ssidRenderer.Draw(gameTime, sb);

              //    skyboxRenderer.Draw(null);

              //   GraphicsDevice.RasterizerState = rasterizerState1;
              pointLightUpdater.UpdatePointLight();
              //        chunkRenderer.RenderAllChunksOpq(ChunkManager.chunks, gamePlayer);

              //    entityRenderer.Draw();

              //        chunkRenderer.RenderAllChunksTransparent(ChunkManager.chunks, gamePlayer);


              deferredShadingRenderer.FinalBlend(_spriteBatch, volumetricLightRenderer, GraphicsDevice, gamePlayer);
              GraphicsDevice.DepthStencilState = DepthStencilState.None;



          }*/
        protected override void Draw(GameTime gameTime)
        {
            //     if (!IsActive) return;

            switch (status)
            {
                case GameStatus.Started:

                    //            Debug.WriteLine("started");

                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    // Debug.WriteLine(ChunkManager.chunks.Count);
                    gamePlayerR.gamePlayer.cam.updateCameraVectors();

                    renderPipelineManager.RenderWorld(gameTime, _spriteBatch);


                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);


                    _spriteBatch.End();
                //    GraphicsDevice.RasterizerState = rasterizerState1;
                 
                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

                    if (GameOptions.showGraphicsDebug)
                    {
                        if (renderPipelineManager is HighDefRenderPipelineManager)
                        {
                            HighDefRenderPipelineManager renderPipelineManagerHighDef = renderPipelineManager as HighDefRenderPipelineManager;
                            _spriteBatch.Draw(renderPipelineManagerHighDef.shadowRenderer.shadowMapTarget, new Rectangle(200, 0, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.shadowRenderer.shadowMapTargetFar, new Rectangle(200, 200, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetProjectionDepth, new Rectangle(1000, 200, 200, 200), Color.White);
                            for (int i = 0; i < renderPipelineManagerHighDef.hiZBufferRenderer.hiZBufferTargetMips.Length; i++)
                            {
                                _spriteBatch.Draw(renderPipelineManagerHighDef.hiZBufferRenderer.hiZBufferTargetMips[i], new Rectangle(1200 + i * 200, 200, 200, 200), Color.White);
                            }


                            //   _spriteBatch.Draw(renderPipelineManagerHighDef.ssaoRenderer.ssaoTarget, new Rectangle(400, 400, 400, 400), Color.White);

                            _spriteBatch.Draw(renderPipelineManagerHighDef.contactShadowRenderer.contactShadowRenderTarget, new Rectangle(1200, 800, 400, 400), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLumAllDiffuse, new Rectangle(1600, 800, 400, 400), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLum, new Rectangle(1600, 0, 400, 400), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.ssidRenderer.renderTargetSSID, new Rectangle(2000, 800, 400, 400), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetProjectionDepth, new Rectangle(400, 200, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWS, new Rectangle(600, 200, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetAlbedo, new Rectangle(200, 600, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetMER, new Rectangle(1600, 400, 400, 400), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.volumetricLightRenderer.blendVolumetricMap, new Rectangle(800, 200, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.volumetricLightRenderer.lightShaftTarget, new Rectangle(800, 400, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.motionVectorRenderer.renderTargetMotionVector, new Rectangle(800, 800, 400, 400), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.ssrRenderer.renderTargetSSR, new Rectangle(200, 800, 400, 400), Color.White);

                            /*    _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLumTransparent, new Rectangle(400, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLumSpecTransparent, new Rectangle(400, 1200, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWSTrans0, new Rectangle(0, 400, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWSTrans1, new Rectangle(0, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWSTrans2, new Rectangle(0, 1200, 400, 400), Color.White);*/

                        }

                    }


                    _spriteBatch.End();
  
                    break;
              


            }
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            uiStateManager.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
            //    _

        }

    }
}