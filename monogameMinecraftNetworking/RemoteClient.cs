using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;

namespace monogameMinecraftNetworking
{
    public class RemoteClient
    {
        public Socket socket;
        public bool isUserDataLoaded = false;
        public UserData curUserData;
        public IMultiplayerServer server;
        public MessageParser messageParser;
        public RemoteClient(Socket s, UserData data, IMultiplayerServer server)
        {

            socket = s;
            curUserData = data;
            this.server = server;
            messageParser = new MessageParser(server,this);
            messageParser.Start();
          
        }

        public void Close()
        {
            isUserDataLoaded = false;
            messageParser.Stop();
            socket.Close();
           
        }
    }
}
