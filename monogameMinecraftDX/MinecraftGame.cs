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
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Pathfinding;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.UI;

using monogameMinecraftShared.Physics;
using monogameMinecraftShared;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Updateables;

//   public SpriteBatch _spriteBatch;
/*    public Effect chunkSolidEffect;
    public Effect chunkShadowEffect;
    public Effect entityEffect;
    public Effect ssaoEffect;
    public Effect gBufferEffect;
    public Effect gBufferEntityEffect;
    public Effect skyboxEffect;
    public Effect lightShaftEffect;
    public Effect ssrEffect;
    public Effect deferredBlockEffect;
    public Effect contactShadowEffect;
    public Effect ssidEffect;
    public Effect deferredBlendEffect;
    public Effect brdfLUTEffect;
    public Effect motionVectorEffect;
    public Effect textureCopyEffect;
    public Effect terrainMipmapEffect;
    public Effect hiZBufferEffect;
    public Effect fxaaEffect;
    public Effect motionBlurEffect;*/

//   public static Vector3 gameposition;
/*
    public Thread updateWorldThread;
    public Thread tryRemoveChunksThread;
    public int renderDistance = 512;*/
//    public GameStatus status = GameStatus.Menu;
//    public PlayerInputManager playerInputManager;
/*
public EntityRenderer entityRenderer;
public ChunkRenderer chunkRenderer;
public ShadowRenderer shadowRenderer;
public SSAORenderer ssaoRenderer;
public SkyboxRenderer skyboxRenderer;
public GBufferRenderer gBufferRenderer;
public SSRRenderer ssrRenderer;
public TextureCube skyboxTex;
public VolumetricLightRenderer volumetricLightRenderer;

public PointLightUpdater pointLightUpdater;
public ContactShadowRenderer contactShadowRenderer;
public SSIDRenderer ssidRenderer;
DeferredShadingRenderer deferredShadingRenderer;


public BRDFLUTRenderer brdfLUTRenderer;
public MotionVectorRenderer motionVectorRenderer;
public TerrainMipmapGenerator terrainMipmapGenerator;
public FXAARenderer fxaaRenderer;
public HiZBufferRenderer hiZBufferRenderer;
public MotionBlurRenderer motionBlurRenderer;
HDRCubemapRenderer hdrCubemapRenderer;*/
// public EffectsManager effectsManager = new EffectsManager();
//  public List<CustomPostProcessor> customPostProcessors = new List<CustomPostProcessor>();
/*
    Texture2D terrainTex;
    Texture2D terrainTexNoMip;
    Texture2D terrainNormal;
    Texture2D terrainDepth;
    Texture2D terrainMER;*/

//  public GameTimeManager gameTimeManager;
//   public IRenderPipelineManager renderPipelineManager { get; set; }
//    public ParticleManager particleManager;


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



/*  UIElement.ScreenRect = this.Window.ClientBounds;
       Debug.WriteLine("screenRect:"+UIElement.ScreenRect);
       foreach (UIElement element in UIElement.menuUIs)
       {
           element.OnResize();
       }
       foreach (UIElement element1 in UIElement.settingsUIsPage1)
       {
           element1.OnResize();
       }
       foreach (UIElement element1 in UIElement.settingsUIsPage2)
       {
           element1.OnResize();
       }
       foreach (UIElement element1 in UIElement.pauseMenuUIs)
       {
           element1.OnResize();
       }
       foreach (UIElement element1 in UIElement.inventoryUIs)
       {
           element1.OnResize();
       }
       foreach (UIElement element1 in UIElement.structureOperationsSavingUIs)
       {
           element1.OnResize();
       }

       foreach (UIElement element1 in UIElement.inGameUIs)
       {
           element1.OnResize();
       }
       foreach (UIElement element1 in UIElement.structureOperationsPlacingUIs)
       {
           element1.OnResize();
       }*/
namespace monogameMinecraftDX
{

    public class MinecraftGame : MinecraftGameBase
    {
        private GraphicsDeviceManager _graphics;
  
        public RandomTextureGenerator randomTextureGenerator;
      

        public MouseMovementManager mouseMovementManager;
    
        public MinecraftGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";
            //    IsMouseVisible = false;
            Window.AllowUserResizing = true;

            Window.ClientSizeChanged += OnResize;
                  _graphics.PreparingDeviceSettings += PrepareGraphicsDevice;
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            effectsManager=new EffectsManager();
            gamePlayerR = new GamePlayerReference();
            renderPipelineManager = new HighDefRenderPipelineManager(this, effectsManager);
          
            gamePlatformType = GamePlatformType.HighDefDX;
            //    TargetElapsedTime = System.TimeSpan.FromMilliseconds(10);
            //         TargetElapsedTime = System.TimeSpan.FromMilliseconds(33);
            //this.OnExiting += OnExit;
        }

        public void PrepareGraphicsDevice(object sender, PreparingDeviceSettingsEventArgs e)
        {
            _graphics.PreferMultiSampling = false;
            _graphics.SynchronizeWithVerticalRetrace = false;

        }
       public override void OnResize(Object sender, EventArgs e)
        {
     
       //   UIResizingManager.Resize(this);
       uiStateManager.ScreenRect = Window.ClientBounds;
       uiResizingManager.Resize(this);
            switch (status)
            {
                case GameStatus.Started:
              
             
                    renderPipelineManager.Resize();
                    break;
            }
            //  button.OnResize();



            Debug.WriteLine(GraphicsDevice.Viewport.Width + " " + GraphicsDevice.Viewport.Height);
        }
 
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
      //      BlockResourcesManager.WriteDefaultBlockInfo(Directory.GetCurrentDirectory() + "/blockinfodata.json");

            status = GameStatus.Started;
            gamePlayerR.gamePlayer = new monogameMinecraftShared.Updateables.GamePlayer(new Vector3(-0.3f, 100, -0.3f), new Vector3(0.3f, 101.8f, 0.3f), this);
            playerInputManager = new PlayerInputManager(gamePlayerR.gamePlayer, false);
            mouseMovementManager = new MouseMovementManager(playerInputManager);
            //  GamePlayer.ReadPlayerData(gamePlayer, this);
            VoxelWorld.currentWorld.InitWorld(this);
            particleManager = new ParticleManager();
            particleManager.Initialize();


            effectsManager.LoadEffects(Content);
            randomTextureGenerator = new RandomTextureGenerator();
            RandomTextureGenerator.instance.GenerateTexture(1024, 1024, GraphicsDevice);

            gameTimeManager = new GameTimeManager(gamePlayerR.gamePlayer);

            renderPipelineManager.InitRenderPipeline();


            EntityManager.InitEntityList();
            EntityManager.LoadEntitySounds(Content);

            EntityManager.ReadEntityData();
            Debug.WriteLine(EntityManager.entityDataReadFromDisk.Count);
            EntityManager.SpawnEntityFromData(this);

            isGamePaused = false;
            mouseMovementManager.isMouseLocked = true;
            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGame]);
        }

        public override void QuitGameplay()
        {
            IsMouseVisible = true;
            GameOptions.SaveOptions(null);
            VoxelWorld.currentWorld.SaveAndQuitWorld(this);
  
            EntityManager.SaveWorldEntityData();
            ChunkHelper.isJsonReadFromDisk = false;
    
  //         UIUtility. InitStructureOperationsUI(this, UIUtility. sf);
           EntityManager.StopAllThreads();
 
            ParticleManagerBase.instance.ReleaseResources();
            GC.Collect();



            status = GameStatus.Menu;
            mouseMovementManager.isMouseLocked = false;
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
                mouseMovementManager.isMouseLocked = false;
                IsMouseVisible = true;
            }
            else
            {
                uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGame]);
                mouseMovementManager.isMouseLocked = true;
                IsMouseVisible = false;
            }
        }
        protected override void Initialize()
        {


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //  InitGameplay();
            //    sf = Content.Load<SpriteFont>("defaultfont");
         //   UIUtility.InitGameUI(this);
            UIResourcesManager.instance.LoadTextures(this);
            UIResourcesManager.instance.LoadDefaultBlockSpriteResources(this);
          
            uiStateManager = new UIStateManager(this);
            uiStateManager.Initialize();
            uiResizingManager = new UIResizingManager(uiStateManager);
            GameOptions.ReadOptionsJson();
            // Chunk c = new Chunk(new Vector2Int(0,0));
            //  chunkSolidEffect = new Effect();
            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.Menu]);
         

            EntityResourcesManager.instance.Initialize();
            EntityResourcesManager.instance.LoadAllDefaultDesources(Content);
            BlockResourcesManager.instance.LoadDefaultResources(Content, GraphicsDevice);
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
        public override void ResumeGame()
        {
            isGamePaused = false;
            IsMouseVisible = false;
            mouseMovementManager.isMouseLocked = true;

            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGame]);
        }
        public override void PauseGame(object obj)
        {
            isGamePaused = true;
            IsMouseVisible = true;
            mouseMovementManager.isMouseLocked = false;

            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGamePaused]);
        }
        public override void CloseStructureOperationsUI(object obj = null)
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
            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGame]);
        }

        public override void OpenStructureOperationsUI(object obj=null)
        {
           
             isStructureOperationsUIOpened = true;
                        if (isStructureOperationsUIOpened == true)
                        {
                            mouseMovementManager.isMouseLocked = false;
                            IsMouseVisible = true;
                        }
                        else
                        {
                            mouseMovementManager.isMouseLocked = true;
                            IsMouseVisible = false;
                        }
            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.StructureOperations]);
        }


        
        float prevFPS = 0f;
        public KeyboardState lastKeyState1;
        //  public Vector3Int debugMapOrigin;
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            //  Draw1(gameTime);

            switch (status)
            {
             /*   case GameStatus.Menu:
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
                       
                        OpenInventory(null);



                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.H) && !lastKeyState1.IsKeyUp(Keys.H)&&!isInventoryOpen)
                    {
                        OpenStructureOperationsUI();



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
 
                    EntityManager.pathfindingManager.drawDebugLines=!EntityManager.pathfindingManager.drawDebugLines;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.N) && !lastKeyState1.IsKeyUp(Keys.N))
                    {
                    EntityManager.SpawnNewEntity(gamePlayerR.gamePlayer.position,0,0,0,0,this);
                    }
         
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
                   
                    if (isInventoryOpen||isStructureOperationsUIOpened)
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
                    mouseMovementManager.windowBounds = Window.ClientBounds;
                    mouseMovementManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                    /*      float curFps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
                          float deltaFps = Math.Abs(curFps - prevFPS);
                          Window.Title = deltaFps < 20f ? deltaFps.ToString() : "delta fps more than 20";
                          prevFPS = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;*/


                    break;
            }
            uiStateManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            //     Debug.WriteLine(gamePlayer.position);
            //     Debug.WriteLine(gamePlayer.cam.Pitch+" "+ gamePlayer.cam.Yaw);
            //    Debug.WriteLine(gamePlayer.cam.position + " " + gamePlayer.cam.front+" "+gamePlayer.cam.up);
            base.Update(gameTime);

        }
  
        RasterizerState rasterizerState = new RasterizerState();
        RasterizerState rasterizerState1 = new RasterizerState{DepthClipEnable=false};
        
  
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

                    renderPipelineManager.RenderWorld(gameTime,_spriteBatch);
   

                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                 
                    _spriteBatch.End();
                    GraphicsDevice.RasterizerState = rasterizerState1;
            /*        foreach (var block in VoxelCast.blocksTmp)
                    {
                        VoxelCast.line.SetUpBasicEffect(GraphicsDevice, RandomTextureGenerator.instance.randomTex, gamePlayer.cam.viewMatrix, gamePlayer.cam.projectionMatrix);
                        VoxelCast.line.ReCreateVisualLine(null, block.Min, block.Max, 0.01f, new Color(1f, 1f, 1f));
                        VoxelCast.line.Draw(GraphicsDevice);
                        VoxelCast.line.SetUpBasicEffect(GraphicsDevice, RandomTextureGenerator.instance.randomTex, gamePlayer.cam.viewMatrix, gamePlayer.cam.projectionMatrix);
                        VoxelCast.line.ReCreateVisualLine(null, new Vector3(block.Min.X, block.Max.Y, block.Min.Z), block.Max, 0.01f, new Color(1f, 1f, 1f));
                        VoxelCast.line.Draw(GraphicsDevice);
                        VoxelCast.line.SetUpBasicEffect(GraphicsDevice, RandomTextureGenerator.instance.randomTex, gamePlayer.cam.viewMatrix, gamePlayer.cam.projectionMatrix);
                        VoxelCast.line.ReCreateVisualLine(null, new Vector3(block.Min.X, block.Min.Y, block.Max.Z), block.Max, 0.01f, new Color(1f, 1f, 1f));
                        VoxelCast.line.Draw(GraphicsDevice);
                        VoxelCast.line.SetUpBasicEffect(GraphicsDevice, RandomTextureGenerator.instance.randomTex, gamePlayer.cam.viewMatrix, gamePlayer.cam.projectionMatrix);
                        VoxelCast.line.ReCreateVisualLine(null, new Vector3(block.Max.X, block.Min.Y, block.Min.Z), block.Max, 0.01f, new Color(1f, 1f, 1f));
                        VoxelCast.line.Draw(GraphicsDevice);
                    }*/
                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap,blendState:BlendState.AlphaBlend);

                    if (GameOptions.showGraphicsDebug)
                    {
                        if (renderPipelineManager is HighDefRenderPipelineManager)
                        {
                            HighDefRenderPipelineManager renderPipelineManagerHighDef=renderPipelineManager as HighDefRenderPipelineManager;
                            _spriteBatch.Draw(renderPipelineManagerHighDef.shadowRenderer.shadowMapTarget, new Rectangle(200, 0, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.shadowRenderer.shadowMapTargetFar, new Rectangle(200, 200, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetProjectionDepth, new Rectangle(1000, 200, 200, 200), Color.White);
                            for (int i = 0; i < renderPipelineManagerHighDef.hiZBufferRenderer.hiZBufferTargetMips.Length; i++)
                            {
                                _spriteBatch.Draw(renderPipelineManagerHighDef.hiZBufferRenderer.hiZBufferTargetMips[i], new Rectangle(1200 + i * 200, 200, 200, 200), Color.White);
                            }


                            //   _spriteBatch.Draw(renderPipelineManagerHighDef.ssaoRenderer.ssaoTarget, new Rectangle(400, 400, 400, 400), Color.White);

                           //      _spriteBatch.Draw(renderPipelineManagerHighDef.contactShadowRenderer.contactShadowRenderTarget, new Rectangle(1200, 800, 400, 400), Color.White);
                               
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.ssidRenderer.renderTargetSSID, new Rectangle(2000, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetProjectionDepth, new Rectangle(400, 200, 200, 200), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWS, new Rectangle(600, 200, 200, 200), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetAlbedo, new Rectangle(200, 600, 200, 200), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetMER, new Rectangle(1600, 400, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.volumetricLightRenderer.blendVolumetricMap, new Rectangle(800, 200, 200, 200), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.volumetricLightRenderer.lightShaftTarget, new Rectangle(800, 400, 200, 200), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.motionVectorRenderer.renderTargetMotionVector, new Rectangle(800, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.ssrRenderer.renderTargetSSR, new Rectangle(200, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLumSpecTransparent, new Rectangle(1200, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLumTransparent, new Rectangle(1600, 800, 400, 400), Color.White);
                            /*    _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLumTransparent, new Rectangle(400, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.deferredShadingRenderer.renderTargetLumSpecTransparent, new Rectangle(400, 1200, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWSTrans0, new Rectangle(0, 400, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWSTrans1, new Rectangle(0, 800, 400, 400), Color.White);
                                 _spriteBatch.Draw(renderPipelineManagerHighDef.gBufferRenderer.renderTargetNormalWSTrans2, new Rectangle(0, 1200, 400, 400), Color.White);*/

                        }
                       
                    }


                    _spriteBatch.End();
               /*     if (isInventoryOpen)
                    {
                        _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

                        foreach (var el in UIElement.inventoryUIs)
                        {
                            el.DrawString(el.text);
                        }
                        _spriteBatch.End();
                    }

                    if (isStructureOperationsUIOpened)
                    {
                        switch (UIElement.structureOperationsUIsPageID)
                        {
                            case 0:
                                _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

                                foreach (var el in UIElement.structureOperationsSavingUIs)
                                {
                                    el.DrawString(el.text);
                                }
                                _spriteBatch.End();
                                break;
                            case 1:
                                _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

                                foreach (var el in UIElement.structureOperationsPlacingUIs)
                                {
                                    el.DrawString(el.text);
                                }
                                _spriteBatch.End();
                                break;
                            default:
                                _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

                                foreach (var el in UIElement.structureOperationsSavingUIs)
                                {
                                    el.DrawString(el.text);
                                }
                                _spriteBatch.End();
                                break;
                              
                        }
                       
                    }
                    if (isGamePaused)
                    {
                        _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);

                        foreach (var el in UIElement.pauseMenuUIs)
                        {
                            el.DrawString(el.text);
                        }
                        _spriteBatch.End();
                    }*/
                    break;
               /* case GameStatus.Menu:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                    foreach (var el in UIElement.menuUIs)
                    {
                        el.DrawString(el.text);
                    }
                    _spriteBatch.End();
                    break;



                case GameStatus.Settings:
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                    switch (UIElement.settingsUIsPageID)
                    {
                        case 0:
                            foreach (var el in UIElement.settingsUIsPage1)
                            {
                                el.DrawString(el.text);
                            }
                            break;
                        case 1:
                            foreach (var el in UIElement.settingsUIsPage2)
                            {
                                el.DrawString(el.text);
                            }
                            break;
                        default:
                            foreach (var el in UIElement.settingsUIsPage1)
                            {
                                el.DrawString(el.text);
                            }
                            break;
                    }

                    _spriteBatch.End();
                    break;*/


            }
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            uiStateManager.Draw();
            _spriteBatch.End();
           
            base.Draw(gameTime);
            //    _

        }

    }
}