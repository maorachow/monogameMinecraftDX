using monogameMinecraftNetworking.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Client
{
    public class NetworkingUtility
    {
        public static object sendToServerLock = new object();
        public static bool SendMessageToServer(MessageProtocol m,Socket socket)
        {
            try
            {
                lock (sendToServerLock)
                {

                    socket.Send(m.GetBytes());
                }

                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
