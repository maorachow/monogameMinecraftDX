using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameMinecraftNetworking.Client;
using monogameMinecraftNetworking.Client.Rendering;
using monogameMinecraftNetworking.Client.UI;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftShared;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftClientDX
{
    public class MinecraftGameClient : ClientGameBase
    {
        private GraphicsDeviceManager _graphics;
    //    private SpriteBatch _spriteBatch;
        public RandomTextureGenerator randomTextureGenerator;
        public MouseMovementManager mouseMovementManager;
        public ParticleManager particleManager;
        public MinecraftGameClient()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            Window.ClientSizeChanged += OnResize;
           
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            effectsManager = new EffectsManager();
          
            renderPipelineManager = new HighDefNetworkingRenderPipelineManager(this, effectsManager);
            gamePlayerR = new GamePlayerReference();
            gamePlatformType = GamePlatformType.HighDefDX;

        }

        public void OnResize(object sender, EventArgs e)
        {
            UIElement.ScreenRect = Window.ClientBounds;
            if (mouseMovementManager != null)
            {
                mouseMovementManager.windowBounds = Window.ClientBounds;
            }
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
            foreach (UIElement element1 in UIElement.structureOperationsPlacingUIs)
            {
                element1.OnResize();
            }
            foreach (UIElement element1 in UIElement.inGameUIs)
            {
                element1.OnResize();
            }
            foreach (UIElement element1 in UIElement.chatMessagesUIs)
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
        }

        public bool isChatMessageSendingUIOpen = false;

        public override void OpenChatUI()
        {
            isChatMessageSendingUIOpen = !isChatMessageSendingUIOpen;
            if (isChatMessageSendingUIOpen == true)
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
        public override void CloseChatUI()
        {
            isChatMessageSendingUIOpen = !isChatMessageSendingUIOpen;
            if (isChatMessageSendingUIOpen == true)
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

        public override void SendChatMessage(object obj,string text)
        {
         
            networkingClient.SendChatMessage(text);
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
           gamePlayerR. gamePlayer =
                new ClientSideGamePlayer(new Vector3(-0.3f, 100, -0.3f), new Vector3(0.3f, 101.8f, 0.3f), this, "default user");
            MultiplayerClientUIUtility.InitGameUI(this);
            GameOptions.ReadOptionsJson();
            status = GameStatus.Menu;
            base.Initialize();
        }


        public override void OpenInventory(UIButton ub)
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

        

        public override void GoToSettings(object obj)
        {
            //  throw new NotImplementedException();

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
        public override void PauseGame(object _)
        {
            isGamePaused = true;
            IsMouseVisible = true;
            mouseMovementManager.isMouseLocked = false;
        }

        public override void ResumeGame(object o)
        {
            isGamePaused = false;
            IsMouseVisible = false;
            mouseMovementManager.isMouseLocked = true;
        }

        public override void QuitGameplay()
        {
            particleManager.ReleaseResources();
            networkingClient.Disconnect();
            ClientSideVoxelWorld.singleInstance.Stop();
            status = GameStatus.Menu;
            mouseMovementManager.isMouseLocked = false;
        }
        public  void QuitGameplayDirectly()
        {
            particleManager.ReleaseResources();
            ClientSideVoxelWorld.singleInstance.Stop();
            status = GameStatus.Menu;
            mouseMovementManager.isMouseLocked = false;
        }
        public UIButton errorLogButton;


        public override void ClientSwitchToWorld(int worldID)
        {
            if (gamePlayerR.gamePlayer == null)
            {
                return;
            }

            if (status != GameStatus.Started)
            {
                return;
            }
            ClientSideVoxelWorld.singleInstance.Stop();
            ClientSideVoxelWorld.SwitchToWorldWithoutSaving(worldID);
            ClientSideVoxelWorld.singleInstance.InitWorld(this);
            (gamePlayerR.gamePlayer as ClientSideGamePlayer).Reset();
        }
        public override void InitGameplay(UIButton obj)
        {
          
                
                int buttonIndex = UIElement.menuUIs.FindIndex((element) =>
            {
                if (element is UIButton)
                {
                    UIButton button = element as UIButton;
                    if (button.isClickable == false &&
                        button.text.Contains("Connection Result : "))
                    {
                        return true;
                    }
                }

                return false;

            });
            errorLogButton = (UIButton)(buttonIndex == -1 ? (UIElement.menuUIs.Count >= 8 ? UIElement.menuUIs[7] : null) : UIElement.menuUIs[buttonIndex]);
            IPAddress address;
            int port;
            string name;
            try
            {
                address = IPAddress.Parse(inputIPAddress);
                port = inputPort;
                if (port < 0 || port > 65535)
                {
                    throw new ArgumentOutOfRangeException("port out of valid range");

                }

                name = inputUserName;
            }
            catch (Exception e)
            {
                if (errorLogButton != null)
                {
                    Debug.WriteLine("print to button");
                    errorLogButton.text = "Connection Result : Failed. Error:" + e.GetType();
                }
                Debug.WriteLine(e);
                return;
            }
            if (errorLogButton != null)
            {
                errorLogButton.text = "Connection Result : Success";
            }
            GameOptions.ReadOptionsJson();
            randomTextureGenerator = new RandomTextureGenerator();
        
            RandomTextureGenerator.instance.GenerateTexture(1024, 1024, GraphicsDevice);
            font =Content.Load<SpriteFont>("defaultfont");

           (gamePlayerR.gamePlayer as ClientSideGamePlayer).playerName = name;
           (gamePlayerR.gamePlayer as ClientSideGamePlayer).Reset();
           ClientSideVoxelWorld.singleInstance.worldID = 0;
           //  MultiplayerClientUIUtility.InitGameUI(this);
           playerInputManager = new PlayerInputManager(gamePlayerR.gamePlayer, false);
           mouseMovementManager = new MouseMovementManager(playerInputManager);
           mouseMovementManager.windowBounds = Window.ClientBounds;
            gameTimeManager = new GameTimeManager(gamePlayerR.gamePlayer);
            effectsManager.LoadEffects(Content);
         
            networkingClient = new MultiplayerClient(address, port, (gamePlayerR.gamePlayer as ClientSideGamePlayer), this);
            networkingClient.clientDisconnectedAction += (string s) => { errorLogButton.text = s; };
            TextListUI chatMessageListElement = (UIElement.inGameUIs.Find((item) => { return item is TextListUI; })) as TextListUI;
            if (chatMessageListElement != null)
            {
                chatMessageListElement.texts = new List<string>();
                networkingClient.chatMessageReceivedAction += chatMessageListElement.AppendText;
            }

          
            clientSideEntityManager = new ClientSideEntityManager(this.networkingClient);
            clientSidePlayersManager=new ClientSidePlayersManager(networkingClient);
            particleManager = new ParticleManager();
            particleManager.Initialize();
            renderPipelineManager.InitRenderPipeline();
        
           
           bool succeeded= networkingClient.Connect();
          
            ClientSideVoxelWorld.singleInstance.InitWorld(this);
            if (succeeded == false)
            {
                if (errorLogButton != null)
                {
                    errorLogButton.text = "Connection Result : Failed.";
                    return;
                }
            }
            status = GameStatus.Started;
            mouseMovementManager.isMouseLocked = true;
            OnResize(null, null);
        }
        
        protected override void LoadContent()
        {

        
            // TODO: use this.Content to load your game content here
        }

        public SpriteFont font;
        private bool isGamePaused;
        private bool isInventoryOpen;
        private KeyboardState lastKeyState1;
        private bool isStructureOperationsUIOpened;

        protected override void Update(GameTime gameTime)
        {
       /*     if (!IsActive)
            {
                return;
            }*/
     /*       if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/
            

       //     playerInputManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        //    gamePlayer.UpdatePlayer(this, (float)gameTime.ElapsedGameTime.TotalSeconds);



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

                    if (networkingClient.isGoingToQuitGame == true)
                    {
                        QuitGameplayDirectly();
                        return;
                    }
                    clientSideEntityManager.FrameUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);
                    clientSidePlayersManager.FrameUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);
                    ClientSideVoxelWorld.singleInstance.FrameUpdate((float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (particleManager.isResourcesReleased == false)
                    {
                        ParticleManager.instance.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                 
                    (gamePlayerR.gamePlayer as ClientSideGamePlayer).UpdatePlayer(this, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    playerInputManager.ResetPlayerInputValues();
                    if (isGamePaused)
                    {

                        foreach (var el in UIElement.pauseMenuUIs)
                        {
                            el.Update();
                        }
                        break;
                    }
                   
                    if (Keyboard.GetState().IsKeyUp(Keys.T) && !lastKeyState1.IsKeyUp(Keys.T) && !isStructureOperationsUIOpened&&!isInventoryOpen)
                    {
                        if (isChatMessageSendingUIOpen == false)
                        {
                            OpenChatUI();
                        }
                    

                    }
                    
                    if (Keyboard.GetState().IsKeyUp(Keys.E) && !lastKeyState1.IsKeyUp(Keys.E) && !isStructureOperationsUIOpened&&!isChatMessageSendingUIOpen)
                    {
                      
                        isInventoryOpen = !isInventoryOpen;
                        if (isInventoryOpen == true)
                        {
                            mouseMovementManager.isMouseLocked = false;
                            IsMouseVisible = true;
                        }
                        else
                        {
                            IsMouseVisible = false;
                            mouseMovementManager.isMouseLocked = true;
                        }

                    }
             
                    

                   
                     
                    /*      if (Keyboard.GetState().IsKeyUp(Keys.K) && !lastKeyState1.IsKeyUp(Keys.K))
                          {
                              //  ChunkHelper.FillBlocks(new BlockData[50,50,50],(Vector3Int)gamePlayer.position+ new Vector3Int(-25,-25,-25));
                              ChunkHelper.FillBlocks(StructureManager.LoadStructure(Directory.GetCurrentDirectory() + "/defaultstructure.bin"), (Vector3Int)gamePlayer.position + new Vector3Int(-5, -5, -5),BlockFillMode.ReplaceAir); ;
                          }*/
                    lastKeyState1 = Keyboard.GetState();
                    if (isChatMessageSendingUIOpen)
                    {
                        foreach (var el in UIElement.chatMessagesUIs)
                        {
                            el.Update();
                        }

                        break;
                    }
                    //       Debug.WriteLine(isInventoryOpen);
                    if (isInventoryOpen)
                    {
                        foreach (var el in UIElement.inventoryUIs)
                        {
                            el.Update();
                        }
                        break;
                    }
                  
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        //     status = GameStatus.Quiting;
                        //  QuitGameplay();
                        //  Exit();
                        //   Environment.Exit(0);
                    /*    isGamePaused = true;
                        IsMouseVisible = true;*/
                    PauseGame(null);
                    }




                    playerInputManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                    mouseMovementManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                    //    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                    foreach (var el in UIElement.inGameUIs)
                    {
                        el.Update();
                    }

                   
                    gameTimeManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    //    _spriteBatch.End();
                    // TODO: Add your update logic here
            
             
                    gameposition = gamePlayerR.gamePlayer.position;


                    /*      float curFps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
                          float deltaFps = Math.Abs(curFps - prevFPS);
                          Window.Title = deltaFps < 20f ? deltaFps.ToString() : "delta fps more than 20";
                          prevFPS = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;*/

                   
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
        /*    if (!IsActive)
            {
                return;
            }*/
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*      gamePlayer.cam.updateCameraVectors();
                    renderPipelineManager.RenderWorld(gameTime,_spriteBatch);

                    _spriteBatch.Begin();


                    if (renderPipelineManager is LowDefNetworkingClientRenderPipelineManager)
                    {
                        LowDefNetworkingClientRenderPipelineManager renderPipelineManagerLowDef = renderPipelineManager as LowDefNetworkingClientRenderPipelineManager;
                        /*    _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTarget, new Rectangle(200, 0, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTargetFar, new Rectangle(200, 200, 200, 200), Color.White);
                            for (int i = 0; i < renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips.Length; i++)
                            {
                                _spriteBatch.Draw(renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips[i], new Rectangle(1200 + i * 200, 200, 200, 200), Color.White);
                            }


                        _spriteBatch.Draw(renderPipelineManagerLowDef.ssaoRenderer.ssaoTarget, new Rectangle(400, 400, 400, 400), Color.White);

                        _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetProjectionDepth, new Rectangle(400, 200, 200, 200), Color.White);
                        _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetNormalWS, new Rectangle(600, 200, 200, 200), Color.White);
                        _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetAlbedo, new Rectangle(200, 600, 200, 200), Color.White);
                        _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetMER, new Rectangle(1600, 400, 400, 400), Color.White);

                    }
                    _spriteBatch.End();*/
            switch (status)
            {
                case GameStatus.Started:

                    //            Debug.WriteLine("started");

                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    // Debug.WriteLine(ChunkManager.chunks.Count);
                    gamePlayerR.gamePlayer.cam.updateCameraVectors();

                    renderPipelineManager.RenderWorld(gameTime, _spriteBatch);
                    //        _spriteBatch.Begin(blendState:BlendState.Additive);
                    //        _spriteBatch.Draw(volumetricLightRenderer.lightShaftTarget, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth , GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                    //       _spriteBatch.End();

                    _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

                    foreach (var el in UIElement.inGameUIs)
                    {
                        el.DrawString(el.text);
                    }
                    _spriteBatch.End();

                    _spriteBatch.Begin();


                    if (renderPipelineManager is HighDefNetworkingRenderPipelineManager)
                    {
                        HighDefNetworkingRenderPipelineManager renderPipelineManagerLowDef = renderPipelineManager as HighDefNetworkingRenderPipelineManager;
                        /*    _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTarget, new Rectangle(200, 0, 200, 200), Color.White);
                            _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTargetFar, new Rectangle(200, 200, 200, 200), Color.White);
                            for (int i = 0; i < renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips.Length; i++)
                            {
                                _spriteBatch.Draw(renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips[i], new Rectangle(1200 + i * 200, 200, 200, 200), Color.White);
                            }*/


                 //       _spriteBatch.Draw(renderPipelineManagerLowDef.ssaoRenderer.ssaoTarget, new Rectangle(400, 400, 400, 400), Color.White);

                //        _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetProjectionDepth, new Rectangle(400, 200, 200, 200), Color.White);
                //        _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetNormalWS, new Rectangle(600, 200, 200, 200), Color.White);
                 //       _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetAlbedo, new Rectangle(200, 600, 200, 200), Color.White);
                 //       _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetMER, new Rectangle(1600, 400, 400, 400), Color.White);

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
                    if (isChatMessageSendingUIOpen)
                    {
                        _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);
                        foreach (var el in UIElement.chatMessagesUIs)
                        {
                            el.DrawString(null);
                        }
                        _spriteBatch.End();


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
        }
    }
}
