using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Client.Updateables;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;
using static System.Net.Mime.MediaTypeNames;
using EntityData = monogameMinecraftNetworking.Data.EntityData;
namespace monogameMinecraftNetworking.Client
{
    public class MultiplayerClient:IMultiplayerClient
    {
        public object todoListLock { get=> _todoListLock; }
        public object _todoListLock=new object();
        public ConcurrentQueue<MessageProtocol> todoList { get; set; }
        public UserData playerData
        {
            get { return gamePlayer.ToUserData(); }
        }

        public List<UserData> allUserDatas { get; set; }

        public List<EntityData> allEntityDatas {
            get
            {
                return _allEntityDatas;
            }

            set
            {
                _allEntityDatas = value;
            }
        }
        public List<EntityData> _allEntityDatas;
        public IMultiplayerClient.OnAllEntitiesDataUpdated allEntitiesUpdatedAction
        {


            get { return _allEntitiesUpdatedAction; }

            set { _allEntitiesUpdatedAction = value; }
        }
        public IMultiplayerClient.OnAllEntitiesDataUpdated allEntitiesPreUpdatedAction
        {


            get { return _allEntitiesPreUpdatedAction; }

            set { _allEntitiesPreUpdatedAction = value; }
        }
        public IMultiplayerClient.OnAllEntitiesDataUpdated _allEntitiesUpdatedAction;
        public IMultiplayerClient.OnAllEntitiesDataUpdated _allEntitiesPreUpdatedAction;

        public IMultiplayerClient.OnClientDisconnected clientDisconnectedAction
        {


            get { return _clientDisconnectedAction; }

            set { _clientDisconnectedAction = value; }
        }
        public IMultiplayerClient.OnClientDisconnected _clientDisconnectedAction;
        public Socket socket { get; set; }
        public bool isLoggedIn { get; set; }
        public bool isGoingToQuitGame { get; set; }=false;
        public MessageParserSingleSocket messageParser { get; set; }
        public ClientSideGamePlayer gamePlayer { get; set; }
        public ClientGameBase game { get; set; }

        public IMultiplayerClient.OnAllUsersDataUpdated allUsersUpdatedAction
        {
            get { return _allUsersUpdatedAction;}

            set { _allUsersUpdatedAction = value; }


        }
        public IMultiplayerClient.OnAllUsersDataUpdated _allUsersUpdatedAction;

        public IMultiplayerClient.OnAllUsersDataUpdated prevAllUsersUpdatedAction
        {
            get { return _prevAllUsersUpdatedAction; }

            set { _prevAllUsersUpdatedAction = value; }


        }
        public IMultiplayerClient.OnAllUsersDataUpdated _prevAllUsersUpdatedAction;

        public IMultiplayerClient.OnChatMessageReceived chatMessageReceivedAction
        {
            get { return _chatMessageReceivedAction; }

            set { _chatMessageReceivedAction = value; }


        }

        public IMultiplayerClient.OnChatMessageReceived _chatMessageReceivedAction;

        public IPAddress address;
        public int port;

        public MultiplayerClient(IPAddress address, int port, ClientSideGamePlayer gamePlayer, ClientGameBase game)
        {
            this.gamePlayer = gamePlayer;
            this.address = address;
            this.port = port;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.todoList = new ConcurrentQueue<MessageProtocol>();
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
                if (isGoingToQuitGame == true)
                {
                    Debug.WriteLine("quit game");
                    return;
                }
                
                if (todoList.Count > 0)
                {
                    MessageProtocol item;
                    todoList.TryDequeue(out item);
                    if (item == null)
                    {
                        Debug.WriteLine("null message");
                        continue;
                    }
                    switch ((MessageCommandType)item.command)
                    {
                        case MessageCommandType.WorldData:


                            Task.Run(() => {

                                lock (ClientSideVoxelWorld.singleInstance.allChunksBuildingLock)
                                {
                                    ChunkDataWithWorldID chunkData = ChunkDataSerializingUtility.DeserializeChunkWithWorldID(item.messageData);// MessagePackSerializer.Deserialize<ChunkData>(item.messageData);
                                    if (chunkData == null)
                                    {
                                        return;
                                    }

                                    if (chunkData.worldID != ClientSideVoxelWorld.singleInstance.worldID)
                                    {
                                        return;
                                    }
                                    if (ClientSideVoxelWorld.singleInstance.chunks.ContainsKey(chunkData.chunkData.chunkPos))
                                    {
                                        ClientSideChunk c =
                                            ClientSideVoxelWorld.singleInstance.GetChunk(chunkData.chunkData.chunkPos);
                                        bool isChunkFirstLoaded = (c.isMapDataFetchedFromServer == false);
                                        lock (c.chunkBuildingLock)
                                        {
                                            c.map =
                                                (BlockData[,,])chunkData.chunkData.map.Clone();
                                         

                                            c.isMapDataFetchedFromServer = true;


                                            c.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance
                                                .clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                        }

                                        if (!isChunkFirstLoaded)
                                        {
                                            ClientSideChunk c1 = ClientSideChunkHelper
                                                .GetChunk(new Vector2Int(c.chunkPos.x - ClientSideChunk.chunkWidth,
                                                    c.chunkPos.y));
                                            if (c1!=null&&c1.isMapDataFetchedFromServer == true)
                                            {
                                                lock (c1.chunkBuildingLock)
                                                {
                                                    c1.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                                }
                                            
                                            }



                                            ClientSideChunk c2 = ClientSideChunkHelper
                                                .GetChunk(new Vector2Int(c.chunkPos.x + ClientSideChunk.chunkWidth,
                                                    c.chunkPos.y));
                                            if (c2!=null&&c2.isMapDataFetchedFromServer == true)
                                            {
                                                lock (c2.chunkBuildingLock)
                                                {
                                                    c2.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                                }
                                              
                                            }

                                            ClientSideChunk c3 = ClientSideChunkHelper
                                                .GetChunk(new Vector2Int(c.chunkPos.x,
                                                    c.chunkPos.y - ClientSideChunk.chunkWidth));


                                            if (c3!=null&&c3.isMapDataFetchedFromServer == true)
                                            {
                                                lock (c3.chunkBuildingLock)
                                                {
                                                    c3.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                                }
                                            
                                            }


                                            ClientSideChunk c4 = ClientSideChunkHelper
                                                .GetChunk(new Vector2Int(c.chunkPos.x,
                                                    c.chunkPos.y + ClientSideChunk.chunkWidth));



;                                            if (c4 !=null&&c4.isMapDataFetchedFromServer == true)
                                            {
                                                lock (c4.chunkBuildingLock)
                                                {
                                                    c4.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                                }
                                             
                                            }
                                         gamePlayer.isGetBlockNeeded=true;
                                        }
                                 


                                        

                                 
                                 
                                  
                                    }
                                }
                            });
                               
                            break;
                        case MessageCommandType.UserLoginReturn:
                            string result= MessagePackSerializer.Deserialize<string>(item.messageData);
                            if (result == "Success")
                            {
                                isLoggedIn=true;
                            }else if (result == "Failed")
                            {
                                isGoingToQuitGame = true;
                                if (_clientDisconnectedAction != null)
                                {
                                    _clientDisconnectedAction(
                                        "User Login Failed: A Player With The Same Username Has Joined The Server.");
                                }
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
                        case MessageCommandType.UserDataBroadcast:

                            if (_prevAllUsersUpdatedAction != null)
                            {
                                _prevAllUsersUpdatedAction();
                            }
                            allUserDatas = MessagePackSerializer.Deserialize<List<UserData>>(item.messageData);
                            if (_allUsersUpdatedAction != null)
                            {
                                _allUsersUpdatedAction();
                            }
                            break;

                        case MessageCommandType.EntityDataBroadcast:
                            if (_allEntitiesPreUpdatedAction != null)
                            {
                                _allEntitiesPreUpdatedAction();
                            }
                            _allEntityDatas = EntityDataSerializingUtility.DeserializeEntityDatas(item.messageData);
                            if (_allEntitiesUpdatedAction != null)
                            {
                                _allEntitiesUpdatedAction();
                            }
                            break;
                        case MessageCommandType.HurtEntityRequest:
                            NetworkingUtility.SendMessageToServer(item, socket);
                            break;
                        case MessageCommandType.BlockSoundBroadcast:
                            BlockSoundBroadcastData data4 =
                                MessagePackSerializer.Deserialize<BlockSoundBroadcastData>(item.messageData);
                            if (Chunk.blockSoundInfo.ContainsKey(data4.blockID))
                            {
                                SoundsUtility.PlaySound(gamePlayer.position, new Vector3(data4.posX, data4.posY, data4.posZ), Chunk.blockSoundInfo[data4.blockID], 20f);
                            }
                         
                            break;
                        case MessageCommandType.BlockParticleBroadcast:
                 //           Debug.WriteLine("particle broadcast");
                            BlockParticleEffectBroadcastData data5 =
                                MessagePackSerializer.Deserialize<BlockParticleEffectBroadcastData>(item.messageData);
                    //        SoundsUtility.PlaySound(gamePlayer.position, new Vector3(data4.posX, data4.posY, data4.posZ), Chunk.blockSoundInfo[data4.blockID], 20f);
                            ClientSideParticleEmittingHelper.EmitParticleWithParamCustomUV(new Vector3(data5.posX, data5.posY, data5.posZ), ParticleEmittingHelper.allParticles["blockbreakingclientside"],
                                new Vector4(Chunk.blockInfosNew[data5.blockID].uvCorners[0].X,
                                    Chunk.blockInfosNew[data5.blockID].uvCorners[0].Y,
                                    Chunk.blockInfosNew[data5.blockID].uvSizes[0].X / 4.0f,
                                    Chunk.blockInfosNew[data5.blockID].uvSizes[0].Y / 4.0f), new Vector2(Chunk.blockInfosNew[data5.blockID].uvSizes[0].X * 0.75f, Chunk.blockInfosNew[data5.blockID].uvSizes[0].Y * 0.75f));
                            break;
                        case MessageCommandType.ChatMessageBroadcast:
                            string message = MessagePackSerializer.Deserialize<string>(item.messageData);
                      //      Debug.WriteLine("chat message received:"+message);
                            if (_chatMessageReceivedAction != null)
                            {
                                _chatMessageReceivedAction(message);
                            }
                            break;
                    }
                }
                
               
            }
        }

        public bool Connect()
        {
            try
            {
                this.socket.Connect(address, port);
               
            }
            catch(Exception e)
            {
                Debug.WriteLine("connecting failed:"+e);
                isGoingToQuitGame = true;
                if (_clientDisconnectedAction != null)
                {
                    _clientDisconnectedAction("Connecting Failed:" + e);
                }
                return false;
            }

            isLoggedIn = false;
          executeTodoListThread=new Thread(ExecuteTodoList);
          executeTodoListThread.IsBackground = true;
          executeTodoListThread.Start();
          messageParser.Start();
          disconnectIfSocketClosedThread = new Thread(DisconnectIfSocketClosedThread);
          disconnectIfSocketClosedThread.IsBackground= true;
          disconnectIfSocketClosedThread.Start();
          bool loginResult=PlayerLogin();
        
          return true;
        }
        public Thread disconnectIfSocketClosedThread;
        public void DisconnectIfSocketClosedThread()
        {
            while (true)
            {
                if (isGoingToQuitGame == true)
                {
                    Debug.WriteLine("quit game disconnecting socket thread");
                    return;
                }
                Thread.Sleep(2000);
                if (messageParser.isMessageParsingThreadRunning == false&&isGoingToQuitGame==false)
                {
                    isGoingToQuitGame = true;
                    if (_clientDisconnectedAction != null)
                    {
                        _clientDisconnectedAction("Client Disconnected: Server Closed");
                    }
                }
            }
        }
        public bool PlayerLogin()
        {
            byte[] userDataBytes = MessagePackSerializer.Serialize(playerData);
          bool result=  NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserLogin,userDataBytes),socket);
          return result;
        }

        public void Disconnect()
        {
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserLogout, new byte[]{}), socket);
            isGoingToQuitGame = true;
            if (_clientDisconnectedAction != null)
            {
                _clientDisconnectedAction("Client Logged Out");
            }
            messageParser.Stop();
            socket.Shutdown(SocketShutdown.Both);
            socket.Close(5000);
        }

        public void SendChatMessage(string message)
        {
            Debug.WriteLine("send message:" + message);
            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.ChatMessage,
                MessagePackSerializer.Serialize(message)),socket);
        }
    }
}
