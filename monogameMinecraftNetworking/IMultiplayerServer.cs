using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Updateables;

namespace monogameMinecraftNetworking
{
    public interface IMultiplayerServer
    {

      //  public List<MessageParser> messageParser { get; set; }

          public object remoteClientsLock { get; }

        public object todoListLock { get;  }
        public List<RemoteClient> remoteClients { get; set; }
        
        public List<ServerTodoList> serverTodoLists { get; set; }

        public List<UserData> allUserDatas { get; }

        public IPEndPoint ipEndPoint { get; set; }

        public bool isGoingToQuit { get; set; }
        public List<IUpdatingManager> updatingManagers { get; set; }
        public void Initialize();
        public void Start();
        public void ShutDown();
    }
}
