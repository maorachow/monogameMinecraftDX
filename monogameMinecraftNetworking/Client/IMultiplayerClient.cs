using System;
using System.Collections.Concurrent;
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
        public ConcurrentQueue<MessageProtocol> todoList { get; }
        public List<UserData> allUserDatas { get; set; }
        public UserData playerData { get; }
        public Socket socket { get; }
        public bool isLoggedIn { get; }
        public MessageParserSingleSocket  messageParser { get; }

        public ClientSideGamePlayer gamePlayer { get; }

        public ClientGameBase game { get; }

        public bool isGoingToQuitGame { get; set; }
        public bool Connect();

        public bool PlayerLogin();

        public void Disconnect();
        public void DisconnectIfSocketClosedThread();

        public delegate void OnAllUsersDataUpdated();

        public OnAllUsersDataUpdated allUsersUpdatedAction { get; set; }
        //   public void MessageParsingThread(Socket s);
    }
}
