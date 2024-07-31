using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Updateables;
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

        public List<IUpdatingManager> updatingManagers { get; set; }
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

            ServerSideVoxelWorld.serverInstance = this;
            updatingManagers = new List<IUpdatingManager> { new UserUpdatingManager(this) };
            remoteClients =new List<RemoteClient>();
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
                Thread.Sleep(1);
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
                                        Console.WriteLine("login");
                                        NetworkingUtility.UserLogin(item.sourceClient,item.message.messageData,this);
                                        break;

                                    case MessageCommandType.UserLogout:
                                       NetworkingUtility.UserLogout(item.sourceClient,this);
                                        break;


                                    case MessageCommandType.UserDataUpdate:
                                        UserData userData = MessagePackSerializer.Deserialize<UserData>(item.message.messageData);
                                        lock (remoteClientsLock)
                                        {
                                            int idx = remoteClients.FindIndex((client =>
                                            {
                                                if (client.isUserDataLoaded == false)
                                                {
                                                    return false;
                                                }

                                                return client.curUserData.userName == userData.userName;

                                            }));
                                            Console.WriteLine(userData.userName);
                                            if (idx != -1)
                                            {
                                                remoteClients[idx] .curUserData=userData;
                                            }
                                            else
                                            {
                                                Console.WriteLine("unknown user data");
                                                Console.WriteLine(userData.userName);
                                            }
                                        }
                                    
                                        break;
                                    case MessageCommandType.UserDataRequest:
                                        NetworkingUtility.CastToAllClients(this,item.message,true);
                                        break;

                                    case MessageCommandType.UserDataBroadcast:
                                        NetworkingUtility.CastToAllClients(this,new MessageProtocol((byte)MessageCommandType.UserDataBroadcast, MessagePackSerializer.Serialize(this.allUserDatas)),true );
                                        break;

                                    case MessageCommandType.WorldGenParamsRequest:
                                        int worldID = MessagePackSerializer.Deserialize<int>(item.message.messageData);
                                        NetworkingUtility.SendToClient(item.sourceClient, new MessageProtocol((byte)MessageCommandType.WorldGenParamsData, MessagePackSerializer.Serialize(ServerSideVoxelWorld.voxelWorlds[worldID].genParamsData)));
                                        break;
                                    case MessageCommandType.ChunkUpdateData:
                                        ChunkUpdateData data2 =
                                            MessagePackSerializer
                                                .Deserialize<ChunkUpdateData>(item.message.messageData);
                                        if (ServerSideVoxelWorld.voxelWorlds[data2.worldID] != null)
                                        {
                                            IChunkUpdateOperation? operation1 = IChunkUpdateOperation.ParseFromData(data2, ServerSideVoxelWorld.voxelWorlds[data2.worldID].worldUpdater);
                                            ServerSideVoxelWorld.voxelWorlds[data2.worldID].worldUpdater.queuedChunkUpdatePoints.Enqueue(operation1);
                                        }
                                    
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public Thread validateClientsThread;
        public void ValidateClientsThread()
        {
            while (true)
            {
                Thread.Sleep(50);
                NetworkingUtility.RemoveDisconnectedClients(this);
            }
        }

        public void Start()
        {
            socketWaitThread = new Thread(() => { SocketWait(serverSocket); });
            socketWaitThread.Start();

            validateClientsThread = new Thread(ValidateClientsThread);
            validateClientsThread.Start();
            executeTodoListThread = new Thread(() => { ExecuteToDoList(); });
            executeTodoListThread.Start();
            foreach (var item in updatingManagers)
            {
                item.Start();
            }
        }
    }
}
