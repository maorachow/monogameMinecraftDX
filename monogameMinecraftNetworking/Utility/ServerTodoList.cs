using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Protocol;

namespace monogameMinecraftNetworking.Utility
{
    public class ServerTodoList
    {
        public PriorityQueue<(RemoteClient sourceClient, MessageProtocol message), int> value;
        public object queueLock=new object();

        public ServerTodoList()
        {
            this.value = new PriorityQueue<(RemoteClient sourceClient, MessageProtocol message), int>();
        }
    }
}
