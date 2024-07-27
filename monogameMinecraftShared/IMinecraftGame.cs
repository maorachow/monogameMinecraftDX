using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftShared
{


    public enum GameStatus
    {
        Menu,
        Settings,
        Started,
        Quiting
    }
    public class MinecraftGameBase:Game
    {

        public string optionalPlatformDataPath;
        public GamePlayer gamePlayer { get; set; }

        public static Vector3 gameposition;

        public SpriteBatch _spriteBatch;

        public GameTimeManager gameTimeManager;
        public GameStatus status { get; set; }

        public GamePlatformType gamePlatformType { get; set; }

        public IEffectsManager effectsManager;

        public IRenderPipelineManager renderPipelineManager;
        public virtual void UpdateStaticPlayerPos()
        {
            gameposition = gamePlayer.position;
        }

        public virtual void GoToSettings(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual void InitGameplay(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual void GoToMenuFromSettings(object obj)
        {
            throw new NotImplementedException();
        }

        public virtual void ResumeGame()
        {
            throw new NotImplementedException();
        }

        public virtual void QuitGameplay()
        {
            throw new NotImplementedException();
        }


        public virtual void CloseStructureOperationsUI()
        {
           
        }


        private GraphicsDeviceManager _graphics;
        public MinecraftGameBase()
        {
       /*     _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Content.RootDirectory = "Content";
            //    IsMouseVisible = false;
            Window.AllowUserResizing = true;

            Window.ClientSizeChanged += OnResize;
       //     _graphics.PreparingDeviceSettings += PrepareGraphicsDevice;
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsMouseVisible = true;
            this.IsFixedTimeStep = false;
            renderPipelineManager = new HighDefRenderPipelineManager(this, effectsManager);
            //    TargetElapsedTime = System.TimeSpan.FromMilliseconds(10);
            //         TargetElapsedTime = System.TimeSpan.FromMilliseconds(33);
            //this.OnExiting += OnExit;*/
        }




        public virtual void OnResize(Object sender, EventArgs e) {}
    }
}
