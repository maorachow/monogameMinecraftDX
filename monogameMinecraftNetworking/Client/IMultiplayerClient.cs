using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftShared.Updateables;

namespace monogameMinecraftNetworking.Client
{
    public interface IMultiplayerClient
    {
        public object todoListLock { get; }
        public Queue<MessageProtocol> todoList { get; }

        public UserData playerData { get; }
        public Socket socket { get; }
        public bool isLoggedIn { get; }
        public MessageParserSingleSocket  messageParser { get; }

        public ClientSideGamePlayer gamePlayer { get; }

        public ClientGameBase game { get; }

        public bool isGoingToQuitGame { get; set; }
        public void Connect();

        public void PlayerLogin();
        //   public void MessageParsingThread(Socket s);
    }
}
