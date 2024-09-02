using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;
using monogameMinecraftShared.World;

namespace monogameMinecraftNetworking.Updateables
{
    public class WorldTimeUpdatingManager:IUpdatingManager
    {
        public IMultiplayerServer server { get; set; }

        public WorldTimeUpdatingManager(IMultiplayerServer server)
        {
            this.server = server;
            gameTimeManager = new GameTimeManager(null,true);
        }
        public bool isThreadStopping = false;
        public Thread updateThread;
        public GameTimeManager gameTimeManager;
        public void UpdateThread()
        {
            while (true)
            {
                if (isThreadStopping == true)
                {
                    return;
                }
                Thread.Sleep(50);
                /*   lock (server.todoListLock)
                   {
                       if (server.serverTodoLists.Count> 0)
                       {
                           server.serverTodoLists[0].value.Enqueue((null, new MessageProtocol((byte)MessageCommandType.UserDataRequest, new byte[] { })),0);
                           server.serverTodoLists[0].value.Enqueue((null, new MessageProtocol((byte)MessageCommandType.UserDataBroadcast, new byte[] { })), 0);
                       }

                   }*/
                gameTimeManager.Update(0.05f);
                NetworkingUtility.EnqueueTodoList(server.serverTodoLists, (null, new MessageProtocol((byte)MessageCommandType.WorldTimeDataBroadcast, MessagePackSerializer.Serialize(gameTimeManager.dateTime))));
          

            }

        }

        public void Start()
        {
            isThreadStopping = false;
            updateThread = new Thread(UpdateThread);
            updateThread.Start();
        }

        public void Stop()
        {
            isThreadStopping = true;
            updateThread.Join();
        }
    }
}
