using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking.Client.World;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;
using monogameMinecraftShared.Asset;
using monogameMinecraftShared.Core;
using monogameMinecraftShared.Updateables;
using monogameMinecraftShared.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Client
{
    public interface INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient  client,MessageProtocol protocol);
    }

    public class NetworkingClientReceivedProtocolActionHelper
    {
        public static readonly Dictionary<MessageCommandType, INetworkingClientReceivedProtocolAction> mappedActions =
            new Dictionary<MessageCommandType, INetworkingClientReceivedProtocolAction>()
            {
                {MessageCommandType.WorldData,new WorldDataReceivedProtocolAction()},
                {MessageCommandType.UserLoginReturn,new UserLoginReturnReceivedProtocolAction()},
                {MessageCommandType. UserDataRequest,new UserDataRequestReceivedProtocolAction()},
                {MessageCommandType.WorldGenParamsData,new WorldGenParamsDataReceivedProtocolAction()},
                {MessageCommandType.UserDataBroadcast,new UserDataBroadcastReceivedProtocolAction()},
                {MessageCommandType.EntityDataBroadcast,new EntityDataBroadcastReceivedProtocolAction()},
                {MessageCommandType.HurtEntityRequest,new HurtEntityRequestReceivedProtocolAction()},
                {MessageCommandType.BlockSoundBroadcast,new BlockSoundBroadcastReceivedProtocolAction()},
                {MessageCommandType.BlockParticleBroadcast,new BlockParticleBroadcastReceivedProtocolAction()},
                {MessageCommandType.ChatMessageBroadcast,new ChatMessageBroadcastReceivedProtocolAction()},
                {MessageCommandType.EntitySoundBroadcast,new EntitySoundBroadcastReceivedProtocolAction()},
                {MessageCommandType.WorldTimeDataBroadcast,new WorldTimeDataBroadcastReceivedProtocolAction()},
            };

        public static void ProcessMessageProtocolActions(IMultiplayerClient client, MessageProtocol protocol)
        {
            MessageCommandType itemCommand = (MessageCommandType)protocol.command;
            INetworkingClientReceivedProtocolAction action=null;
            mappedActions.TryGetValue(itemCommand, out action);
            if (action != null)
            {
                action.Process(client,protocol);
            }
        }
    }

    public class WorldDataReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            Task.Run(() => {

                lock (ClientSideVoxelWorld.singleInstance.allChunksBuildingLock)
                {
                    ChunkDataWithWorldID chunkData = ChunkDataSerializingUtility.DeserializeChunkWithWorldID(protocol.messageData);// MessagePackSerializer.Deserialize<ChunkData>(item.messageData);
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
                            if (c1 != null && c1.isMapDataFetchedFromServer == true)
                            {
                                lock (c1.chunkBuildingLock)
                                {
                                    c1.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                }

                            }



                            ClientSideChunk c2 = ClientSideChunkHelper
                                .GetChunk(new Vector2Int(c.chunkPos.x + ClientSideChunk.chunkWidth,
                                    c.chunkPos.y));
                            if (c2 != null && c2.isMapDataFetchedFromServer == true)
                            {
                                lock (c2.chunkBuildingLock)
                                {
                                    c2.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                }

                            }

                            ClientSideChunk c3 = ClientSideChunkHelper
                                .GetChunk(new Vector2Int(c.chunkPos.x,
                                    c.chunkPos.y - ClientSideChunk.chunkWidth));


                            if (c3 != null && c3.isMapDataFetchedFromServer == true)
                            {
                                lock (c3.chunkBuildingLock)
                                {
                                    c3.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                }

                            }


                            ClientSideChunk c4 = ClientSideChunkHelper
                                .GetChunk(new Vector2Int(c.chunkPos.x,
                                    c.chunkPos.y + ClientSideChunk.chunkWidth));



                            ; if (c4 != null && c4.isMapDataFetchedFromServer == true)
                            {
                                lock (c4.chunkBuildingLock)
                                {
                                    c4.BuildChunkAsyncWithActionOnCompleted(ClientSideVoxelWorld.singleInstance.clientSideWorldUpdater.chunkMainThreadUpdateActions);
                                }

                            }
                           client. gamePlayer.isGetBlockNeeded = true;
                        }








                    }
                }
            });
        }
    }

    public class UserLoginReturnReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            string result = MessagePackSerializer.Deserialize<string>(protocol.messageData);
            if (result == "Success")
            {
                client. isLoggedIn = true;
            }
            else if (result == "Failed")
            {
                client.isGoingToQuitGame = true;
                if (client.clientDisconnectedAction != null)
                {
                    client.clientDisconnectedAction(
                        "User Login Failed: A Player With The Same Username Has Joined The Server.");
                }
            }
            else if (result == "Failed:Banned")
            {
                client.isGoingToQuitGame = true;
                if (client.clientDisconnectedAction != null)
                {
                    client.clientDisconnectedAction(
                        "User Login Failed: Current Username Is Banned From The Server.");
                }
            }
        }
    }

    public class UserDataRequestReceivedProtocolAction : INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {

            NetworkingUtility.SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserDataUpdate, MessagePackSerializer.Serialize(client.playerData)), client.socket);
        }
    }

    public class WorldGenParamsDataReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            WorldGenParamsData data =
                MessagePackSerializer.Deserialize<WorldGenParamsData>(protocol.messageData);
            if (ClientSideVoxelWorld.singleInstance.isWorldGenParamsInited == false)
            {
                if (data.worldID == ClientSideVoxelWorld.singleInstance.worldID)
                {
                    ClientSideVoxelWorld.singleInstance.isWorldGenParamsInited = true;
                    ClientSideVoxelWorld.singleInstance.genParamsData = data;
                }
            }
            else if(ClientSideVoxelWorld.singleInstance.isWorldGenParamsInited == true&& data.worldID == ClientSideVoxelWorld.singleInstance.worldID)
            {
                throw new Exception("worldgen params data already received");
            }
            
        }
    }

    public class UserDataBroadcastReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            if (client.prevAllUsersUpdatedAction != null)
            {
                client.prevAllUsersUpdatedAction();
            }
            client.allUserDatas = MessagePackSerializer.Deserialize<List<UserData>>(protocol.messageData);
            if (client.allUsersUpdatedAction != null)
            {
                client.allUsersUpdatedAction();
            }
        }
    }

    public class EntityDataBroadcastReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            if (client.allEntitiesPreUpdatedAction != null)
            {
                client.allEntitiesPreUpdatedAction();
            }
            client.allEntityDatas = EntityDataSerializingUtility.DeserializeEntityDatas(protocol.messageData);
            if (client.allEntitiesUpdatedAction != null)
            {
                client.allEntitiesUpdatedAction();
            }
        }
    }

    public class HurtEntityRequestReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            NetworkingUtility.SendMessageToServer(protocol, client.socket);
        }
    }

    public class BlockSoundBroadcastReceivedProtocolAction : INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            BlockSoundBroadcastData data4 =
                MessagePackSerializer.Deserialize<BlockSoundBroadcastData>(protocol.messageData);
            if (Chunk.blockSoundInfo.ContainsKey(data4.blockID))
            {
                SoundsUtility.PlaySound(client.gamePlayer.position, new Vector3(data4.posX, data4.posY, data4.posZ), Chunk.blockSoundInfo[data4.blockID], 20f);
            }
        }
    }

    public class BlockParticleBroadcastReceivedProtocolAction : INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            BlockParticleEffectBroadcastData data5 =
                MessagePackSerializer.Deserialize<BlockParticleEffectBroadcastData>(protocol.messageData);
            //        SoundsUtility.PlaySound(gamePlayer.position, new Vector3(data4.posX, data4.posY, data4.posZ), Chunk.blockSoundInfo[data4.blockID], 20f);
            if (Chunk.blockInfosNew.ContainsKey(data5.blockID))
            {
                ParticleEmittingHelper.EmitParticleWithParamCustomUV(new Vector3(data5.posX, data5.posY, data5.posZ), ParticleEmittingHelper.allParticles["blockbreaking"],
                    new Vector4(Chunk.blockInfosNew[data5.blockID].uvCorners[0].X,
                        Chunk.blockInfosNew[data5.blockID].uvCorners[0].Y,
                        Chunk.blockInfosNew[data5.blockID].uvSizes[0].X / 4.0f,
                        Chunk.blockInfosNew[data5.blockID].uvSizes[0].Y / 4.0f), new Vector2(Chunk.blockInfosNew[data5.blockID].uvSizes[0].X * 0.75f, Chunk.blockInfosNew[data5.blockID].uvSizes[0].Y * 0.75f));
            }
        }
    }

    public class ChatMessageBroadcastReceivedProtocolAction : INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            string message = MessagePackSerializer.Deserialize<string>(protocol.messageData);
            //      Debug.WriteLine("chat message received:"+message);
            if (client.chatMessageReceivedAction != null)
            {
                client.chatMessageReceivedAction(message);
            }
        }
    }

    public class EntitySoundBroadcastReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            EntitySoundBroadcastData data6 =
                MessagePackSerializer.Deserialize<EntitySoundBroadcastData>(protocol.messageData);
            if (EntityResourcesManager.instance.loadedEntitySounds.ContainsKey(data6.soundID))
            {
                SoundsUtility.PlaySound(client.gamePlayer.position, new Vector3(data6.posX, data6.posY, data6.posZ), EntityResourcesManager.instance.loadedEntitySounds[data6.soundID], 20f);
            }
        }
    }

    public class WorldTimeDataBroadcastReceivedProtocolAction: INetworkingClientReceivedProtocolAction
    {
        public void Process(IMultiplayerClient client, MessageProtocol protocol)
        {
            float timeData = MessagePackSerializer.Deserialize<float>(protocol.messageData);
            client. game.gameTimeManager.SetDateTime(timeData);
        }
    }



}
