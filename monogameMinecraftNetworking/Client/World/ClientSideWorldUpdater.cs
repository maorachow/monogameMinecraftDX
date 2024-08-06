using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Client.World
{
    public class ClientSideWorldUpdater
    {
        public ConcurrentQueue<Action> chunkMainThreadUpdateActions=new ConcurrentQueue<Action>();


        public ClientSideWorldUpdater() { }

        public void MainThreadUpdate(float deltaTime)
        {
            while (chunkMainThreadUpdateActions.Count > 0)
            {

                Action result;
                var item = chunkMainThreadUpdateActions.TryDequeue(out result);
                if (result != null && item == true)
                {
                    result();
                }
            }
        }
    }
}
