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
    public class ClientGameBase: MinecraftGameBase
    {

        
     //   public GameStatus status;
        public string inputIPAddress;
        public int inputPort;
        public string inputUserName;
       
      //  public GamePlayerReference gamePlayerR { get; set; }
       // public GameTimeManager gameTimeManager { get; set; }
      //  public static Vector3 gameposition;

   //     public SpriteBatch _spriteBatch;
     //   public PlayerInputManager playerInputManager;

        public ClientSideEntityManager clientSideEntityManager;
        public ClientSidePlayersManager clientSidePlayersManager;
    //    public IEffectsManager effectsManager;

      //  public INetworkClientRenderPipelineManager renderPipelineManager;
        public IMultiplayerClient networkingClient;

        public override void OpenInventory(object ub)
        {
          //  throw new NotImplementedException();
        }

        public override void InitGameplay(object obj)
        {
          //  throw new NotImplementedException();
        }

      /*  public virtual void GoToSettings(object obj)
        {
          //  throw new NotImplementedException();
        }*/

      //  public virtual void PauseGame(object o)
     //   {
        //    throw new NotImplementedException();
      //  }

        

     //   public virtual void QuitGameplay()
     //   {
          //  throw new NotImplementedException();
     //   }

        public override void ResumeGame()
        {
            
        }

        public virtual void ClientSwitchToWorld(int worldID)
        {

        }

      //  public virtual void GoToMenuFromSettings(object obj)
     //   {
           
     //   }

        public virtual void SendChatMessage(object obj, string text)
        {

        }


        

        public virtual void OpenChatUI()
        {
            
        }
        public virtual void CloseChatUI()
        {

        }
    }
}
