using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftShared.Updateables;
using EntityData = monogameMinecraftNetworking.Data.EntityData;


namespace monogameMinecraftNetworking.Client
{
    public interface IMultiplayerClient
    {
        public object todoListLock { get; }
        public ConcurrentQueue<MessageProtocol> todoList { get; }
        public List<UserData> allUserDatas { get; set; }

        public List<EntityData> allEntityDatas { get; set; }
        public UserData playerData { get; }
        public Socket socket { get; }
        public bool isLoggedIn { get; }
        public MessageParserSingleSocket  messageParser { get; }

        public ClientSideGamePlayer gamePlayer { get; }

        public ClientGameBase game { get; }

        public bool isGoingToQuitGame { get; set; }
        public void Initialize(IPAddress address, int port, ClientSideGamePlayer gamePlayer, ClientGameBase game);
        public bool Connect();


        public bool PlayerLogin();

        public void Disconnect();
        public void DisconnectIfSocketClosedThread();

        public delegate void OnAllUsersDataUpdated();

        public delegate void OnAllEntitiesDataUpdated();

        public delegate void OnChatMessageReceived(string  message);

        public delegate void OnClientDisconnected(string optionalErrorMessage);

        public OnClientDisconnected clientDisconnectedAction { get; set; }
        public OnChatMessageReceived chatMessageReceivedAction { get; set; }
        public OnAllUsersDataUpdated allUsersUpdatedAction { get; set; }
        public OnAllUsersDataUpdated prevAllUsersUpdatedAction { get; set; }
        public OnAllEntitiesDataUpdated allEntitiesUpdatedAction { get; set; }
         public OnAllEntitiesDataUpdated allEntitiesPreUpdatedAction { get; set; }

         public void SendChatMessage(string text);

         //   public void MessageParsingThread(Socket s);
    }
}
