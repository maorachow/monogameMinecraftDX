using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftNetworking.Client.Rendering;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftShared;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Client
{
    public class ClientGameBase:Game
    {

        public GamePlatformType gamePlatformType;
        public GameStatus status;
        public string inputIPAddress;
        public int inputPort;
        public string inputUserName;
       
        public GamePlayerReference gamePlayerR { get; set; }
        public GameTimeManager gameTimeManager { get; set; }
        public static Vector3 gameposition;

        public SpriteBatch _spriteBatch;
        public PlayerInputManager playerInputManager;

        public ClientSideEntityManager clientSideEntityManager;
        public IEffectsManager effectsManager;

        public INetworkClientRenderPipelineManager renderPipelineManager;
        public IMultiplayerClient networkingClient;

        public virtual void OpenInventory(UIButton ub)
        {
          //  throw new NotImplementedException();
        }

        public virtual void InitGameplay(UIButton obj)
        {
          //  throw new NotImplementedException();
        }

        public virtual void GoToSettings(UIButton obj)
        {
          //  throw new NotImplementedException();
        }

        public virtual void PauseGame(object o)
        {
        //    throw new NotImplementedException();
        }

        

        public virtual void QuitGameplay()
        {
          //  throw new NotImplementedException();
        }

        public virtual void ResumeGame(object obj)
        {
            
        }

        public virtual void ClientSwitchToWorld(int worldID)
        {

        }
    }
}
