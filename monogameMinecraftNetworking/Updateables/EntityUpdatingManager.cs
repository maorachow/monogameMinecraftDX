using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;
using monogameMinecraftShared.Updateables;
using EntityData = monogameMinecraftNetworking.Data.EntityData;
namespace monogameMinecraftNetworking.Updateables
{
    public class EntityUpdatingManager:IUpdatingManager
    {
        public IMultiplayerServer server { get; set; }

        public bool isThreadStopping = false;

        public Thread updateAllEntitiesThread;

        public EntityUpdatingManager(IMultiplayerServer server)
        {
            this.server=server;
        }
        public void UpdateThread()
        {
            while (true)
            {
                if (isThreadStopping == true)
                {
                    return;
                }
                Thread.Sleep(50);
                ServerSideEntityManager.TrySpawnNewZombie(server, 0.05f);
                ServerSideEntityManager.UpdateAllEntity(0.05f);
                ServerSideEntityManager.FixedUpdateAllEntity(0.05f);
                ServerSideEntityManager.UpdateCurEntityData();
                NetworkingUtility.EnqueueTodoList(server.serverTodoLists, (null, new MessageProtocol((byte)MessageCommandType.EntityDataBroadcast,EntityDataSerializingUtility.SerializeAllEntityDatas(ServerSideEntityManager.allEntityDatas))));
           //     NetworkingUtility.EnqueueTodoList(server.serverTodoLists, (null, new MessageProtocol((byte)MessageCommandType.UserDataBroadcast, new byte[] { })));

            }
        }

        public void Start()
        {
            ServerSideEntityManager.ReadEntityData();
            ServerSideEntityManager.InitEntityList();
            ServerSideEntityManager.SpawnEntityFromData(server);
            updateAllEntitiesThread =new Thread(UpdateThread);
            updateAllEntitiesThread.Start();


        }

        public void Stop()
        {
            ServerSideEntityManager.SaveWorldEntityData();
            isThreadStopping = true;
            updateAllEntitiesThread.Join();
            ServerSideEntityManager.pathfindingManager.QuitThread();
        }
    }
}
