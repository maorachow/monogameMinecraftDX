using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;
using monogameMinecraftNetworking.World;
using monogameMinecraftShared.Core;

namespace monogameMinecraftNetworking
{
    public class MultiplayerServer:IMultiplayerServer
    {
      //  public List<MessageParser> messageParser { get; set; }
      public object remoteClientsLock
      {
          get { return _remoteClientsLock; }
      }

      private object _remoteClientsLock=new object();

        public object todoListLock
        {
            get { return _todoListLock; }
        }

        private object _todoListLock = new object();
        public List<RemoteClient> remoteClients { get; set; }
        public List<ServerTodoList> serverTodoLists { get; set; }

        public List<UserData> allUserDatas
        {
            get
            {
                List<UserData> retValue= new List<UserData>();
                lock (remoteClientsLock)
                {
                    foreach (var item in remoteClients)
                    {
                        if (item.isUserDataLoaded == true)
                        {
                            retValue.Add(item.curUserData);
                        }
                    }
                }
             
                return retValue;

            }
        }

        public Thread socketWaitThread;
        public void SocketWait(Socket socket)
        {
            while (true)
            {

                socket.Listen();
                Socket s = socket.Accept();
                
                RemoteClient client = new RemoteClient(s, null,this);
                client.isUserDataLoaded = false;
                lock (remoteClientsLock)
                {
                    remoteClients.Add(client);
                }
               

                //   allClientSockets.Add(s);
               // Thread t = new Thread(new ParameterizedThreadStart(RecieveClient));
              //  t.Start(s);
              Console.WriteLine("connected");
              Console.WriteLine(socket.LocalEndPoint. ToString());
            }


        }

        public Socket serverSocket;
        public IPEndPoint ipEndPoint{ get; set; }
        public void Initialize()
        {
            foreach (var world in ServerSideVoxelWorld.voxelWorlds)
            {
                world.InitWorld(this);
            }
            remoteClients=new List<RemoteClient>();
            serverTodoLists = new List<ServerTodoList> { new ServerTodoList() };
            ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11111);
                
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndPoint);
         
        }

        public Thread executeTodoListThread;
        public void ExecuteToDoList()
        {
            while (true)
            {
                Thread.Sleep(10);
                lock (todoListLock)
                {
                    if (serverTodoLists.Count > 0)
                    {
                        foreach (var todo in serverTodoLists)
                        {
                            if (todo.value.Count > 0)
                            {
                                var item = todo.value.Dequeue();


                                switch ((MessageCommandType)item.message.command )
                                {
                                    case MessageCommandType.ChunkDataRequest:
                                        ChunkDataRequestData data =
                                            MessagePackSerializer.Deserialize<ChunkDataRequestData>(item.message.messageData);
                                        if (ServerSideVoxelWorld.voxelWorlds[data.worldID] != null)
                                        {
                                            ServerSideVoxelWorld.voxelWorlds[data.worldID].chunkBuildingQueue.Enqueue((item.sourceClient,data.chunkPos));
                                        }
                                        break;

                                    case MessageCommandType.UserLogin:
                                        NetworkingUtility.UserLogin(item.sourceClient,item.message.messageData,this);
                                        break;

                                    case MessageCommandType.UserLogout:
                                       NetworkingUtility.UserLogout(item.sourceClient,this);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Start()
        {
            socketWaitThread = new Thread(() => { SocketWait(serverSocket); });
            socketWaitThread.Start();
            executeTodoListThread = new Thread(() => { ExecuteToDoList(); });
            executeTodoListThread.Start();
        }
    }
}
