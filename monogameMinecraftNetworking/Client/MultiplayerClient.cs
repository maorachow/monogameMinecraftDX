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
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Client
{
    public class MultiplayerClient:IMultiplayerClient
    {
        public object todoListLock { get=> _todoListLock; }
        public object _todoListLock=new object();
        public Queue<MessageProtocol> todoList { get; set; }
        public UserData playerData
        {
            get { return gamePlayer.ToUserData(); }
        }
        public Socket socket { get; set; }
        public bool isLoggedIn { get; set; }
        public bool isGoingToQuitGame { get; set; }=false;
        public MessageParserSingleSocket messageParser { get; set; }
        public ClientSideGamePlayer gamePlayer { get; set; }
        public ClientGameBase game { get; set; }

        public IPAddress address;
        public int port;

        public MultiplayerClient(IPAddress address, int port, ClientSideGamePlayer gamePlayer, ClientGameBase game)
        {
            this.gamePlayer = gamePlayer;
            this.address = address;
            this.port = port;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.todoList = new Queue<MessageProtocol>();
            isLoggedIn = false;
            isGoingToQuitGame=false;
            messageParser = new MessageParserSingleSocket(todoList, socket, todoListLock);
            this.game = game;
        }

        public Thread executeTodoListThread;

        public void ExecuteTodoList()
        {
            while (true)
            {
                lock (todoListLock)
                {
                    if (todoList.Count > 0)
                    {
                        MessageProtocol item = todoList.Dequeue();
                        switch ((MessageCommandType)item.command)
                        {
                            case MessageCommandType.WorldData:
                                ChunkData chunkData = MessagePackSerializer.Deserialize<ChunkData>(item.messageData);
                                if (ClientSideVoxelWorld.singleInstance.chunks.ContainsKey(chunkData.chunkPos))
                                {
                                    ClientSideChunk c =
                                        ClientSideVoxelWorld.singleInstance.GetChunk(chunkData.chunkPos);
                                   c.map =
                                        (BlockData[,,])chunkData.map.Clone();
                                   bool isChunkFirstLoaded = (c.isMapDataFetchedFromServer == false);

                                   c.isMapDataFetchedFromServer = true;


                                   c.BuildChunk();

                                   if (!isChunkFirstLoaded)
                                   {
                                       if (ClientSideChunkHelper
                                               .GetChunk(new Vector2Int(c.chunkPos.x - ClientSideChunk.chunkWidth,
                                                   c.chunkPos.y))?.isMapDataFetchedFromServer == true)
                                       {
                                           ClientSideChunkHelper.GetChunk(new Vector2Int(c.chunkPos.x - ClientSideChunk.chunkWidth, c.chunkPos.y))?.BuildChunk();
                                       }




                                       if (ClientSideChunkHelper
                                               .GetChunk(new Vector2Int(c.chunkPos.x + ClientSideChunk.chunkWidth,
                                                   c.chunkPos.y))?.isMapDataFetchedFromServer == true)
                                       {
                                           ClientSideChunkHelper.GetChunk(new Vector2Int(c.chunkPos.x + ClientSideChunk.chunkWidth, c.chunkPos.y))?.BuildChunk();
                                       }


                                       if (ClientSideChunkHelper
                                               .GetChunk(new Vector2Int(c.chunkPos.x,
                                                   c.chunkPos.y - ClientSideChunk.chunkWidth))
                                               ?.isMapDataFetchedFromServer == true)
                                       {
                                           ClientSideChunkHelper.GetChunk(new Vector2Int(c.chunkPos.x, c.chunkPos.y - ClientSideChunk.chunkWidth))?.BuildChunk();
                                       }



                                       if (ClientSideChunkHelper
                                               .GetChunk(new Vector2Int(c.chunkPos.x,
                                                   c.chunkPos.y + ClientSideChunk.chunkWidth))
                                               ?.isMapDataFetchedFromServer == true)
                                       {
                                           ClientSideChunkHelper.GetChunk(new Vector2Int(c.chunkPos.x, c.chunkPos.y + ClientSideChunk.chunkWidth))?.BuildChunk();
                                       }
                                    }

                                 
                                 
                                  
                                }
                                break;
                            case MessageCommandType.UserLoginReturn:
                                string result= MessagePackSerializer.Deserialize<string>(item.messageData);
                                if (result == "Success")
                                {
                                    isLoggedIn=true;
                                }else if (result == "Failed")
                                {
                                    isGoingToQuitGame = true;
                                }
                                break;
                            case MessageCommandType.UserDataRequest:

                             
                                NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserDataUpdate, MessagePackSerializer.Serialize(playerData)),socket);
                                    break;
                            case MessageCommandType.WorldGenParamsData:

                                WorldGenParamsData data =
                                    MessagePackSerializer.Deserialize<WorldGenParamsData>(item.messageData);
                                if (data.worldID == ClientSideVoxelWorld.singleInstance.worldID)
                                {
                                    ClientSideVoxelWorld.singleInstance.isWorldGenParamsInited = true;
                                    ClientSideVoxelWorld.singleInstance.genParamsData= data;
                                }
                              //  NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserDataUpdate, MessagePackSerializer.Serialize(playerData)), socket);
                                break;
                        }
                    }
                }
               
            }
        }

        public void Connect()
        {
            try
            {
                this.socket.Connect(address, port);
               
            }
            catch(Exception e)
            {
                Debug.WriteLine("connecting failed:"+e);
                isGoingToQuitGame = true;
                return;
            }
          executeTodoListThread=new Thread(ExecuteTodoList);
          executeTodoListThread.IsBackground = true;
          executeTodoListThread.Start();
          messageParser.Start();

          PlayerLogin();
        }

        public void PlayerLogin()
        {
            byte[] userDataBytes = MessagePackSerializer.Serialize(playerData);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserLogin,userDataBytes),socket);
        }
    }
}
