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
 

namespace monogameMinecraftDX
{
  /*  public enum GameStatus
    {
        Menu,
        Settings,
        Started,
        Quiting
    }*/
    public class MinecraftGame : MinecraftGameBase
    {
        private GraphicsDeviceManager _graphics;
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
        public AlphaTestEffect chunkNSEffect;
    //    public GamePlayer gamePlayer;
     //   public static Vector3 gameposition;
    /*
        public Thread updateWorldThread;
        public Thread tryRemoveChunksThread;
        public int renderDistance = 512;*/
    //    public GameStatus status = GameStatus.Menu;

        public RandomTextureGenerator randomTextureGenerator;
        public GlobalMaterialParamsManager globalMaterialParamsManager;

      //  public GameTimeManager gameTimeManager;
     //   public IRenderPipelineManager renderPipelineManager { get; set; }
        public ParticleManager particleManager;
        public MouseMovementManager mouseMovementManager;
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
            UIElement.ScreenRect = this.Window.ClientBounds;
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
            }
            switch (status)
            {
                case GameStatus.Started:
                    foreach (UIElement element1 in UIElement.pauseMenuUIs)
                    {
                        element1.OnResize();
                    }
                    foreach (UIElement element1 in UIElement.inventoryUIs)
                    {
                        element1.OnResize();
                    }
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
            BlockResourcesManager.WriteDefaultBlockInfo(Directory.GetCurrentDirectory() + "/blockinfodata.json");

            status = GameStatus.Started;
            gamePlayerR.gamePlayer = new monogameMinecraftShared.Updateables.GamePlayer(new Vector3(-0.3f, 100, -0.3f), new Vector3(0.3f, 101.8f, 0.3f), this);
            playerInputManager = new PlayerInputManager(gamePlayerR.gamePlayer, false);
            mouseMovementManager = new MouseMovementManager(playerInputManager);
            //  GamePlayer.ReadPlayerData(gamePlayer, this);
            VoxelWorld.currentWorld.InitWorld(this);
            particleManager = new ParticleManager();
            particleManager.Initialize();
            

            /*     updateWorldThread = new Thread(() => ChunkManager.UpdateWorldThread( gamePlayer,this));
                 updateWorldThread.IsBackground = true;
                 updateWorldThread.Start();
                 tryRemoveChunksThread = new Thread(() => ChunkManager.TryDeleteChunksThread( gamePlayer, this));
                 tryRemoveChunksThread.IsBackground = true;
                 tryRemoveChunksThread.Start();*/

            effectsManager.LoadEffects(Content);
            randomTextureGenerator = new RandomTextureGenerator();
            RandomTextureGenerator.instance.GenerateTexture(1024, 1024, GraphicsDevice);

            globalMaterialParamsManager = new GlobalMaterialParamsManager();
            /*     chunkSolidEffect = Content.Load<Effect>("blockeffect");
                 chunkShadowEffect = Content.Load<Effect>("createshadowmapeffect");
                 entityEffect = Content.Load<Effect>("entityeffect");
                 //  chunkEffect = Content.Load<Effect>("blockeffect");
                 gBufferEffect = Content.Load<Effect>("gbuffereffect");
                 gBufferEntityEffect = Content.Load<Effect>("gbufferentityeffect");
                 ssaoEffect = Content.Load<Effect>("ssaoeffect");
                 lightShaftEffect = Content.Load<Effect>("lightshafteffect");
                 skyboxEffect = Content.Load<Effect>("skyboxeffect");
                 ssrEffect = Content.Load<Effect>("ssreffect");
                 ssidEffect = Content.Load<Effect>("ssideffect");
                 deferredBlockEffect = Content.Load<Effect>("deferredblockeffect");
                 contactShadowEffect = Content.Load<Effect>("contactshadoweffect");
                 deferredBlendEffect = Content.Load<Effect>("deferredblendeffect");
                 brdfLUTEffect = Content.Load<Effect>("brdfluteffect");
                 motionVectorEffect = Content.Load<Effect>("motionvectoreffect");
                 textureCopyEffect = Content.Load<Effect>("texturecopyraweffect");
                 terrainMipmapEffect = Content.Load<Effect>("texturecopyeffect");
                 hiZBufferEffect = Content.Load<Effect>("hizbuffereffect");
                 fxaaEffect = Content.Load<Effect>("fxaaeffect");
                 motionBlurEffect = Content.Load<Effect>("motionblureffect");*/
        /*    terrainTex = Content.Load<Texture2D>("terrain");
            terrainTexNoMip = Content.Load<Texture2D>("terrainnomipmap");
            terrainNormal = Content.Load<Texture2D>("terrainnormal");
            //   chunkSolidEffect = Content.Load<Effect>("blockeffect");
            terrainDepth = Content.Load<Texture2D>("terrainheight");
            terrainMER = Content.Load<Texture2D>("terrainmer");*/
          
            gameTimeManager = new GameTimeManager(gamePlayerR.gamePlayer);
        //    BlockResourcesManager.LoadDefaultResources(Content, GraphicsDevice, chunkRenderer);
         //   effectsManager.LoadCustomPostProcessEffects(GraphicsDevice, customPostProcessors, Content);


            renderPipelineManager.InitRenderPipeline();
      /*      terrainMipmapGenerator = new TerrainMipmapGenerator(GraphicsDevice, effectsManager.gameEffects["texturecopyeffect"]);
            brdfLUTRenderer = new BRDFLUTRenderer(GraphicsDevice, effectsManager.gameEffects["brdfluteffect"]);
            brdfLUTRenderer.CalculateLUT();
            hdrCubemapRenderer = new HDRCubemapRenderer(GraphicsDevice, effectsManager.gameEffects["hdricubeeffect"], environmentHDRITex, effectsManager.gameEffects["hdriirradianceeffect"], effectsManager.gameEffects["hdriprefiltereffect"]);
            hdrCubemapRenderer.Render();
           
            chunkRenderer = new ChunkRenderer(this, GraphicsDevice, effectsManager.gameEffects["blockforwardeffect"], null, gameTimeManager);
            pointLightUpdater = new PointLightUpdater(gamePlayer);*/
            //    chunkRenderer.SetTexture(terrainTexNoMip, terrainNormal, terrainDepth, terrainTexNoMip, terrainMER);
          
            /* gBufferEffect = Content.Load<Effect>("gbuffereffect");
             gBufferEntityEffect = Content.Load<Effect>("gbufferentityeffect");*/
    /*        entityRenderer = new EntityRenderer(this, GraphicsDevice, gamePlayer, effectsManager.gameEffects["entityeffect"], Content.Load<Model>("zombiefbx"), Content.Load<Texture2D>("husk"), Content.Load<Model>("zombiemodelref"), effectsManager.gameEffects["createshadowmapeffect"], null, gameTimeManager);
            gBufferRenderer = new GBufferRenderer(this.GraphicsDevice, effectsManager.gameEffects["gbuffereffect"], effectsManager.gameEffects["gbufferentityeffect"], gamePlayer, chunkRenderer, entityRenderer);
            skyboxRenderer = new SkyboxRenderer(GraphicsDevice, effectsManager.gameEffects["skyboxeffect"], null, gamePlayer, Content.Load<Texture2D>("skybox/skybox"), Content.Load<Texture2D>("skybox/skyboxup"), Content.Load<Texture2D>("skybox/skybox"), Content.Load<Texture2D>("skybox/skybox"), Content.Load<Texture2D>("skybox/skyboxdown"), Content.Load<Texture2D>("skybox/skybox"),
               Content.Load<Texture2D>("skybox/skyboxnight"), Content.Load<Texture2D>("skybox/skyboxnightup"), Content.Load<Texture2D>("skybox/skyboxnight"), Content.Load<Texture2D>("skybox/skyboxnight"), Content.Load<Texture2D>("skybox/skyboxnightdown"), Content.Load<Texture2D>("skybox/skyboxnight"), gameTimeManager
               );
            skyboxRenderer.skyboxTexture = hdrCubemapRenderer.resultSpecularCubemapMip0;
            contactShadowRenderer = new ContactShadowRenderer(GraphicsDevice, effectsManager.gameEffects["contactshadoweffect"], gBufferRenderer, gameTimeManager, gamePlayer);
            shadowRenderer = new ShadowRenderer(this, GraphicsDevice, effectsManager.gameEffects["createshadowmapeffect"], chunkRenderer, entityRenderer, gameTimeManager);
            motionVectorRenderer = new MotionVectorRenderer(this.GraphicsDevice, effectsManager.gameEffects["motionvectoreffect"], gBufferRenderer, gamePlayer);
            ssaoRenderer = new SSAORenderer(effectsManager.gameEffects["ssaoeffect"], gBufferRenderer, chunkRenderer, this.GraphicsDevice, gamePlayer, Content.Load<Texture2D>("randomnormal"));
            fxaaRenderer = new FXAARenderer(GraphicsDevice, effectsManager.gameEffects["fxaaeffect"]);
            motionBlurRenderer = new MotionBlurRenderer(GraphicsDevice, effectsManager.gameEffects["motionblureffect"], motionVectorRenderer);
            deferredShadingRenderer = new DeferredShadingRenderer(GraphicsDevice, effectsManager.gameEffects["deferredblockeffect"], shadowRenderer, ssaoRenderer, gameTimeManager, pointLightUpdater, gBufferRenderer, contactShadowRenderer, null, null, effectsManager.gameEffects["deferredblendeffect"], skyboxRenderer, fxaaRenderer, motionBlurRenderer, hdrCubemapRenderer);


            customPostProcessors.Add(new CustomPostProcessor(GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess0"));
            customPostProcessors.Add(new CustomPostProcessor(GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess1"));
            customPostProcessors.Add(new CustomPostProcessor(GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess2"));
            customPostProcessors.Add(new CustomPostProcessor(GraphicsDevice, motionVectorRenderer, gBufferRenderer, "postprocess3"));
         
            hiZBufferRenderer = new HiZBufferRenderer(GraphicsDevice, effectsManager.gameEffects["hizbuffereffect"], gBufferRenderer, effectsManager.gameEffects["texturecopyraweffect"]);
            ssrRenderer = new SSRRenderer(GraphicsDevice, gamePlayer, gBufferRenderer, effectsManager.gameEffects["ssreffect"], deferredShadingRenderer, effectsManager.gameEffects["texturecopyraweffect"], motionVectorRenderer, hiZBufferRenderer);
            ssidRenderer = new SSIDRenderer(GraphicsDevice, effectsManager.gameEffects["ssideffect"], gBufferRenderer, gamePlayer, deferredShadingRenderer, effectsManager.gameEffects["texturecopyraweffect"], motionVectorRenderer, hiZBufferRenderer);

            deferredShadingRenderer.customPostProcessors = customPostProcessors;
            deferredShadingRenderer.ssidRenderer = ssidRenderer;
            deferredShadingRenderer.ssrRenderer = ssrRenderer;
            chunkRenderer.shadowRenderer = shadowRenderer;
            chunkRenderer.SSAORenderer = ssaoRenderer;
            entityRenderer.shadowRenderer = shadowRenderer;
            chunkRenderer.lightUpdater = pointLightUpdater;
            shadowRenderer.zombieModel = Content.Load<Model>("zombiemodelref");

            volumetricLightRenderer = new VolumetricLightRenderer(GraphicsDevice, gBufferRenderer, _spriteBatch, effectsManager.gameEffects["volumetricmaskblendeffect"], effectsManager.gameEffects["lightshafteffect"], gamePlayer, gameTimeManager);
            chunkRenderer.SSRRenderer = ssrRenderer;
            volumetricLightRenderer.entityRenderer = entityRenderer;*/




            EntityManager.InitEntityList();
            EntityManager.LoadEntitySounds(Content);
            //  rasterizerState.CullMode = CullMode.None;
            //   rasterizerState1.CullMode = CullMode.CullCounterClockwiseFace;
            // EntityBeh.SpawnNewEntity(new Vector3(0, 100, 0), 0f, 0f, 0f, 0, this);
            EntityManager.ReadEntityData();
            Debug.WriteLine(EntityManager.entityDataReadFromDisk.Count);
            EntityManager.SpawnEntityFromData(this);

            isGamePaused = false;
            mouseMovementManager.isMouseLocked = true;
        }

        public override void QuitGameplay()
        {
            IsMouseVisible = true;
            GameOptions.SaveOptions(null);
            VoxelWorld.currentWorld.SaveAndQuitWorld(this);
        //    monogameMinecraftShared.Updateables. GamePlayer.SavePlayerData(gamePlayer);
            /*     foreach(var c in ChunkManager.chunks)
                 {
                 c.Value.Dispose();
                 }*/
            EntityManager.SaveWorldEntityData();
            ChunkHelper.isJsonReadFromDisk = false;
          //  gamePlayer.curChunk = null;
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
           UIUtility. InitStructureOperationsUI(this, UIUtility. sf);
           EntityManager.StopAllThreads();
     //       EntityManager.InitEntityList();
            ParticleManager.instance.ReleaseResources();
            GC.Collect();



            status = GameStatus.Menu;
            mouseMovementManager.isMouseLocked = false;
            // updateWorldThread.Abort();
            //   tryRemoveChunksThread.Abort();
        }

        public override void GoToSettings(object obj)
        {
            GameOptions.ReadOptionsJson();
            foreach (UIElement element1 in UIElement.settingsUIsPage1)
            {
                element1.Initialize();
            }
            foreach (UIElement element1 in UIElement.settingsUIsPage2)
            {
                element1.Initialize();
            }
            this.status = GameStatus.Settings;
        }

        public override void GoToMenuFromSettings(object obj)
        {
            GameOptions.SaveOptions(null);
            this.status = GameStatus.Menu;

        }
        SpriteFont sf;

        public override void OpenInventory(object obj)
        {
            isInventoryOpen = !isInventoryOpen;
            if (isInventoryOpen == true)
            {
                IsMouseVisible = true;
            }
            else
            {
                IsMouseVisible = false;
            }
        }
        protected override void Initialize()
        {


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //  InitGameplay();
            //    sf = Content.Load<SpriteFont>("defaultfont");
            UIUtility.InitGameUI(this);
            GameOptions.ReadOptionsJson();
            // Chunk c = new Chunk(new Vector2Int(0,0));
            //  chunkSolidEffect = new Effect();

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
        }

        public override void CloseStructureOperationsUI()
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
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            //  Draw1(gameTime);

            switch (status)
            {
                case GameStatus.Menu:
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
                    break;
                case GameStatus.Started:

                    if (isGamePaused)
                    {

                        foreach (var el in UIElement.pauseMenuUIs)
                        {
                            el.Update();
                        }
                        break;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.E)&&!lastKeyState1.IsKeyUp(Keys.E) && !isStructureOperationsUIOpened)
                    {
                        //     status = GameStatus.Quiting;
                        //  QuitGameplay();
                        //  Exit();
                        //   Environment.Exit(0);
                       
                        isInventoryOpen=!isInventoryOpen;
                        if (isInventoryOpen == true)
                        {mouseMovementManager.isMouseLocked = false;
                            IsMouseVisible = true;
                        }
                        else
                        {
                            mouseMovementManager.isMouseLocked = true;
                            IsMouseVisible = false;
                        }
                      
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.H) && !lastKeyState1.IsKeyUp(Keys.H)&&!isInventoryOpen)
                    {
                        //     status = GameStatus.Quiting;
                        //  QuitGameplay();
                        //  Exit();
                        //   Environment.Exit(0);

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
                        }*/

                        
                       
                    EntityManager.pathfindingManager.drawDebugLines=!EntityManager.pathfindingManager.drawDebugLines;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.N) && !lastKeyState1.IsKeyUp(Keys.N))
                    {
                    EntityManager.SpawnNewEntity(gamePlayerR.gamePlayer.position,0,0,0,0,this);
                    }
                    /*      if (Keyboard.GetState().IsKeyUp(Keys.K) && !lastKeyState1.IsKeyUp(Keys.K))
                          {
                              //  ChunkHelper.FillBlocks(new BlockData[50,50,50],(Vector3Int)gamePlayer.position+ new Vector3Int(-25,-25,-25));
                              ChunkHelper.FillBlocks(StructureManager.LoadStructure(Directory.GetCurrentDirectory() + "/defaultstructure.bin"), (Vector3Int)gamePlayer.position + new Vector3Int(-5, -5, -5),BlockFillMode.ReplaceAir); ;
                          }*/
                    lastKeyState1 = Keyboard.GetState();
             //       Debug.WriteLine(isInventoryOpen);
                    if (isInventoryOpen)
                    {
                        foreach (var el in UIElement.inventoryUIs)
                        {
                            el.Update();
                        }
                        break;
                    }

                    if (isStructureOperationsUIOpened)
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
                    }
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        //     status = GameStatus.Quiting;
                        //  QuitGameplay();
                        //  Exit();
                        //   Environment.Exit(0);
                        mouseMovementManager.isMouseLocked = false;
                        isGamePaused = true;
                        IsMouseVisible = true;
                    }
                    //    ProcessPlayerKeyboardInput(gameTime);


                    //    ProcessPlayerMouseInput();
                    playerInputManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (gamePlayerR.gamePlayer is GamePlayer player1)
                    {
                        player1.UpdatePlayer(this, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                  

                    //    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                    foreach (var el in UIElement.inGameUIs)
                    {
                        el.Update();
                    }

                    if (VoxelWorld.currentWorld.isThreadsStopping == false)
                    {
                        VoxelWorld.currentWorld.FrameUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                  
                    gameTimeManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    //    _spriteBatch.End();
                    // TODO: Add your update logic here
                    EntityManager.UpdateAllEntity((float)gameTime.ElapsedGameTime.TotalSeconds);
                    EntityManager.FixedUpdateAllEntity((float)gameTime.ElapsedGameTime.TotalSeconds);
                    ParticleManager.instance.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                   EntityManager.TrySpawnNewZombie(this, (float)gameTime.ElapsedGameTime.TotalSeconds);
              //      GlobalMaterialParamsManager.instance.Update(gameTime);
                    gameposition = gamePlayerR.gamePlayer.position;
                    mouseMovementManager.windowBounds = Window.ClientBounds;
                    mouseMovementManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    /*      float curFps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
                          float deltaFps = Math.Abs(curFps - prevFPS);
                          Window.Title = deltaFps < 20f ? deltaFps.ToString() : "delta fps more than 20";
                          prevFPS = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;*/


                    break;
            }


            //     Debug.WriteLine(gamePlayer.position);
            //     Debug.WriteLine(gamePlayer.cam.Pitch+" "+ gamePlayer.cam.Yaw);
            //    Debug.WriteLine(gamePlayer.cam.position + " " + gamePlayer.cam.front+" "+gamePlayer.cam.up);
            base.Update(gameTime);

        }
    /*    void ProcessPlayerMouseInput()
        {
            var mState = Mouse.GetState();
            gamePlayer.cam.ProcessMouseMovement(mState.X - lastMouseX, lastMouseY - mState.Y);
            lastMouseY = mState.Y;
            lastMouseX = mState.X;
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

            gamePlayer.ProcessPlayerInputs(playerVec, (float)gameTime.ElapsedGameTime.TotalSeconds, kState, mState, lastMouseState);

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

                    renderPipelineManager.RenderWorld(gameTime,_spriteBatch);
                    //        _spriteBatch.Begin(blendState:BlendState.Additive);
                    //        _spriteBatch.Draw(volumetricLightRenderer.lightShaftTarget, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth , GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                    //       _spriteBatch.End();

                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                    foreach (var el in UIElement.inGameUIs)
                    {
                        el.DrawString(el.text);
                    }
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
                    if (isInventoryOpen)
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
                    }
                    break;
                case GameStatus.Menu:
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
                    break;

            }
            base.Draw(gameTime);
            //    _

        }

    }
}