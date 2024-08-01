using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Protocol;

namespace monogameMinecraftNetworking.Utility
{
    public class ServerTodoList
    {
        public ConcurrentQueue<(RemoteClient sourceClient, MessageProtocol message)> value;
        public object queueLock=new object();

        public ServerTodoList()
        {
            this.value = new ConcurrentQueue<(RemoteClient sourceClient, MessageProtocol message)>();
        }
    }
}
