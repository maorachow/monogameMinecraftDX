using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogameMinecraftNetworking.Client.Rendering;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Input;
using monogameMinecraftShared.Rendering;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Client
{
    public class ClientGameBase:Game
    {

        public ClientSideGamePlayer gamePlayer { get; set; }

        public GameTimeManager gameTimeManager { get; set; }
        public static Vector3 gameposition;

        public SpriteBatch _spriteBatch;
        public PlayerInputManager playerInputManager;

        public IEffectsManager effectsManager;

        public INetworkClientRenderPipelineManager renderPipelineManager;
        public IMultiplayerClient networkingClient;
    }
}
