using System;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameMinecraftNetworking.Client;
using monogameMinecraftNetworking.Client.Rendering;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace Project1
{
    public class MinecraftGameClient : ClientGameBase
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public RandomTextureGenerator randomTextureGenerator;
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
            effectsManager = new LowDefEffectsManager();
         
            renderPipelineManager = new LowDefNetworkingClientRenderPipelineManager(this, effectsManager);
           
        }

        public void OnResize(object sender, EventArgs e)
        {
            renderPipelineManager.Resize();
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            InitGameplay();
            base.Initialize();
        }

        public void InitGameplay()
        {
            randomTextureGenerator= new RandomTextureGenerator();
            GameOptions.renderSSAO = true;
            RandomTextureGenerator.instance.GenerateTexture(1024, 1024, GraphicsDevice);
            font =Content.Load<SpriteFont>("defaultfont");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gamePlayer =
                new ClientSideGamePlayer(new Vector3(-0.3f, 100, -0.3f), new Vector3(0.3f, 101.8f, 0.3f), this, "abcd");
            playerInputManager = new PlayerInputManager(gamePlayer, false);
            gameTimeManager = new GameTimeManager(gamePlayer);
            effectsManager.LoadEffects(Content);
            renderPipelineManager.InitRenderPipeline();
        
            networkingClient = new MultiplayerClient(IPAddress.Parse("127.0.0.1"), 11111, gamePlayer, this);
            networkingClient.Connect();
            ClientSideVoxelWorld.singleInstance.InitWorld(this);
        }
        protected override void LoadContent()
        {
        

            // TODO: use this.Content to load your game content here
        }

        public SpriteFont font;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            playerInputManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            gamePlayer.UpdatePlayer(this, (float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

          gamePlayer.cam.updateCameraVectors();
            renderPipelineManager.RenderWorld(gameTime,_spriteBatch);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, new StringBuilder("Position:" + (int)gamePlayer.position.X + " " + (int)gamePlayer.position.Y + " " + (int)gamePlayer.position.Z), new Vector2(0, 0), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 1);
          

            if (renderPipelineManager is LowDefNetworkingClientRenderPipelineManager)
            {
                LowDefNetworkingClientRenderPipelineManager renderPipelineManagerLowDef = renderPipelineManager as LowDefNetworkingClientRenderPipelineManager;
                /*    _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTarget, new Rectangle(200, 0, 200, 200), Color.White);
                    _spriteBatch.Draw(renderPipelineManagerLowDef.shadowRenderer.shadowMapTargetFar, new Rectangle(200, 200, 200, 200), Color.White);
                    for (int i = 0; i < renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips.Length; i++)
                    {
                        _spriteBatch.Draw(renderPipelineManagerLowDef.hiZBufferRenderer.hiZBufferTargetMips[i], new Rectangle(1200 + i * 200, 200, 200, 200), Color.White);
                    }*/


                _spriteBatch.Draw(renderPipelineManagerLowDef.ssaoRenderer.ssaoTarget, new Rectangle(400, 400, 400, 400), Color.White);

                _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetProjectionDepth, new Rectangle(400, 200, 200, 200), Color.White);
                _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetNormalWS, new Rectangle(600, 200, 200, 200), Color.White);
                _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetAlbedo, new Rectangle(200, 600, 200, 200), Color.White);
                _spriteBatch.Draw(renderPipelineManagerLowDef.gBufferRenderer.renderTargetMER, new Rectangle(1600, 400, 400, 400), Color.White);

            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
