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
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace monogameMinecraftClientDX
{
    public class MinecraftGameClient : ClientGameBase
    {
        private GraphicsDeviceManager _graphics;
    //    private SpriteBatch _spriteBatch;
        public RandomTextureGenerator randomTextureGenerator;
        public MouseMovementManager mouseMovementManager;
    //    public ParticleManager particleManager;
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
          
            renderPipelineManager = new HighDefRenderPipelineManager(this, effectsManager, () =>
            {
                var renderPipelineManager1 = (renderPipelineManager as HighDefRenderPipelineManager);
                if (renderPipelineManager1.entityRenderers.Count > 0)
                {
                    renderPipelineManager1.entityRenderers[0] = new ClientSideEntitiesRenderer(this.Content.Load<Model>("zombiefbx"), this.Content.Load<Model>("pigfbx"),
                        effectsManager.gameEffects["gbufferentityeffect"], this.gamePlayerR.gamePlayer,
                        this.Content.Load<Texture2D>("husk"), this.Content.Load<Texture2D>("pig"), this.networkingClient, this.GraphicsDevice, this);

                }
                else
                {
                    renderPipelineManager1.entityRenderers.Add(new ClientSideEntitiesRenderer(this.Content.Load<Model>("zombiefbx"), this.Content.Load<Model>("pigfbx"),
                        effectsManager.gameEffects["gbufferentityeffect"], this.gamePlayerR.gamePlayer,
                        this.Content.Load<Texture2D>("husk"), this.Content.Load<Texture2D>("pig"), this.networkingClient, this.GraphicsDevice, this));
                }

                renderPipelineManager1.gBufferRenderer.entityRenderer = renderPipelineManager1. entityRenderers[0];
                renderPipelineManager1.shadowRenderer.entityRenderer= renderPipelineManager1.entityRenderers[0];
            });
            gamePlayerR = new GamePlayerReference();
            gamePlatformType = GamePlatformType.HighDefDX;
            gameArchitecturePatternType = GameArchitecturePatternType.ClientServer;

        }

        public void OnResize(object sender, EventArgs e)
        {
            UIElement.ScreenRect = Window.ClientBounds;
            if (mouseMovementManager != null)
            {
                mouseMovementManager.windowBounds = Window.ClientBounds;
            }
            MultiplayerClientUIResizingManager.Resize(this);
            switch (status)
            {
                case GameStatus.Started:
                
                  
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

        public override void OpenStructureOperationsUI(object obj = null)
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
                    if (button.optionalTag == "connectResultButton"
                       )
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
            gameTimeManager = new GameTimeManager(gamePlayerR.gamePlayer,false);
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
            particleManager = new ClientSideParticleManager();
            particleManager.Initialize();
            renderPipelineManager.InitRenderPipeline((item) =>
            {

                if (item.game.gameArchitecturePatternType == GameArchitecturePatternType.ClientServer)
                {
              //  item.entityRenderer    
                }
            });
            ClientSideEntityManager.LoadEntitySounds(Content);



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
           if (!IsActive)
            {
                return;
            }
     /*       if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();*/
            

       //     playerInputManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        //    gamePlayer.UpdatePlayer(this, (float)gameTime.ElapsedGameTime.TotalSeconds);



            switch (status)
            {
               
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
                        ParticleManagerBase.instance.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                 
                    (gamePlayerR.gamePlayer as ClientSideGamePlayer).UpdatePlayer(this, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    playerInputManager.ResetPlayerInputValues();
                    if (isGamePaused)
                    {

                        if (uiStateManager.curState is not UIStateInGamePaused)
                        {
                            uiStateManager.SwitchToState(UIStateManager.allStates[UIStateTypes.InGamePaused]);
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

                        OpenInventory(null);

                    }
             
                    

                   
                     
                    /*      if (Keyboard.GetState().IsKeyUp(Keys.K) && !lastKeyState1.IsKeyUp(Keys.K))
                          {
                              //  ChunkHelper.FillBlocks(new BlockData[50,50,50],(Vector3Int)gamePlayer.position+ new Vector3Int(-25,-25,-25));
                              ChunkHelper.FillBlocks(StructureManager.LoadStructure(Directory.GetCurrentDirectory() + "/defaultstructure.bin"), (Vector3Int)gamePlayer.position + new Vector3Int(-5, -5, -5),BlockFillMode.ReplaceAir); ;
                          }*/
                    lastKeyState1 = Keyboard.GetState();
                    if (isChatMessageSendingUIOpen)
                    {
                        
                        break;
                    }
                    //       Debug.WriteLine(isInventoryOpen);
                    if (isInventoryOpen)
                    {
                        
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

            uiStateManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*      gamePlayer.cam.updateCameraVectors();
                    renderPipelineManager.RenderWorld(gameTime,_spriteBatch);

                   */
           
    switch (status)
    {
        case GameStatus.Started:

            //            Debug.WriteLine("started");

            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Debug.WriteLine(ChunkManager.chunks.Count);
            gamePlayerR.gamePlayer.cam.updateCameraVectors();
            renderPipelineManager.curRenderingWorld = ClientSideVoxelWorld.singleInstance;
            renderPipelineManager.RenderWorld(gameTime, _spriteBatch);
            //        _spriteBatch.Begin(blendState:BlendState.Additive);
            //        _spriteBatch.Draw(volumetricLightRenderer.lightShaftTarget, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth , GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            //       _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

        /*    foreach (var el in UIElement.inGameUIs)
            {
                el.DrawString(el.text);
            }*/
            _spriteBatch.End();

            _spriteBatch.Begin();

            if (GameOptions.showGraphicsDebug == true)
            {
                if (renderPipelineManager is HighDefRenderPipelineManager)
                {
                    HighDefRenderPipelineManager renderPipelineManagerLowDef = renderPipelineManager as HighDefRenderPipelineManager;
                    _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTarget, new Rectangle(200, 0, 200, 200), Color.White);
                    _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTargetFar, new Rectangle(200, 200, 200, 200), Color.White);
                    for (int i = 0; i < renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips.Length; i++)
                    {
                        _spriteBatch.Draw(renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips[i], new Rectangle(1200 + i * 200, 200, 200, 200), Color.White);
                    }


                    //    _spriteBatch.Draw(renderPipelineManagerLowDef.ssaoRenderer.ssaoTarget, new Rectangle(400, 400, 400, 400), Color.White);

                    //      _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.re, new Rectangle(400, 200, 200, 200), Color.White);
                    //    _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetNormalWS, new Rectangle(600, 200, 200, 200), Color.White);
                    _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetAlbedoTrans0, new Rectangle(200, 600, 200, 200), Color.White);
                    _spriteBatch.Draw(renderPipelineManagerLowDef.deferredShadingRenderer.renderTargetLumTransparent, new Rectangle(1600, 400, 400, 400), Color.White);

                }
                    }

          
                    _spriteBatch.End();

            /*            if (isInventoryOpen)
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
                    }*/
                    break;
           /*     case GameStatus.Menu:
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
                    }*/

                 
                 

            }

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            uiStateManager.Draw();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
