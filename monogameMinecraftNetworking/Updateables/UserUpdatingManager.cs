using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftNetworking.Utility;

namespace monogameMinecraftNetworking.Updateables
{
    public class UserUpdatingManager:IUpdatingManager
    {
        public IMultiplayerServer server { get; set; }

        public UserUpdatingManager(IMultiplayerServer server)
        {
            this.server= server;
        }
        public bool isThreadStopping=false;
        public Thread userUpdatingThread;

        public void Start()
        {
            isThreadStopping = false;
            userUpdatingThread = new Thread(UpdateThread);
            userUpdatingThread.Start();
        }

        public void Stop()
        {
            isThreadStopping = true;
            userUpdatingThread.Join();
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
             /*   lock (server.todoListLock)
                {
                    if (server.serverTodoLists.Count> 0)
                    {
                        server.serverTodoLists[0].value.Enqueue((null, new MessageProtocol((byte)MessageCommandType.UserDataRequest, new byte[] { })),0);
                        server.serverTodoLists[0].value.Enqueue((null, new MessageProtocol((byte)MessageCommandType.UserDataBroadcast, new byte[] { })), 0);
                    }
                   
                }*/
             NetworkingUtility.EnqueueTodoList(server.serverTodoLists, (null, new MessageProtocol((byte)MessageCommandType.UserDataRequest, new byte[] { })));
             NetworkingUtility.EnqueueTodoList(server.serverTodoLists, (null, new MessageProtocol((byte)MessageCommandType.UserDataBroadcast, new byte[] { })));

            }

        }
    }
}
