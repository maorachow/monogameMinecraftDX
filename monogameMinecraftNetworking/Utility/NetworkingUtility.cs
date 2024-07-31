using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;

namespace monogameMinecraftNetworking.Utility
{
    public class NetworkingUtility
    {
        public static object sendToClientsLock=new object();
        public static void SendToClient(RemoteClient remoteClient,MessageProtocol msg)
        {
            try
            {
                lock (sendToClientsLock)
                {
                    remoteClient.socket.Send(msg.GetBytes());
                }
              
                // s.Send(System.Text.Encoding.Default.GetBytes("&"));
            }
            catch (Exception ex)
            {
              Debug.WriteLine("Message sending failed: " + ex);
            }
        }
        public static void SendToClient(Socket remoteClientSocket, MessageProtocol msg)
        {
            try
            {
                lock (sendToClientsLock)
                {
                    remoteClientSocket.Send(msg.GetBytes());
                }

                // s.Send(System.Text.Encoding.Default.GetBytes("&"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Message sending failed: " + ex);
            }
        }
        public static void CastToAllClients(IMultiplayerServer server, MessageProtocol msg)
        {


            try
            {
                if (server is not MultiplayerServer)
                {
                    Debug.WriteLine("not a valid server");
                    return;
                }
                
                lock (sendToClientsLock)
                {
                
                   lock (server.remoteClientsLock)
                   {
                       for (int i = 0; i < server.remoteClients.Count; i++)
                       {
                           if (server.remoteClients[i].socket.Poll(1000, SelectMode.SelectError))
                           {
                               Console.WriteLine("casting failed:polled socket disconnected");
                               continue;
                           }
                            if (!server.remoteClients[i].socket.Poll(1000, SelectMode.SelectRead) || server.remoteClients[i].socket.Available > 0)
                           {
                               server.remoteClients[i].socket.Send(msg.GetBytes());
                           }
                            else
                            {
                                Console.WriteLine("casting failed:polled socket disconnected");
                            }
                            
                       }
                   }
                  

                   //   socket.Send(System.Text.Encoding.Default.GetBytes("&"));
                }
            }
            finally
            {
                 
            }
        }
        public static void CastToAllClients(IMultiplayerServer server, MessageProtocol msg,bool loadedUsersOnly=false)
        {


            try
            {
                if (server is not MultiplayerServer)
                {
                    Debug.WriteLine("not a valid server");
                    return;
                }

                lock (sendToClientsLock)
                {

                    lock (server.remoteClientsLock)
                    {
                        for (int i = 0; i < server.remoteClients.Count; i++)
                        {
                            if (loadedUsersOnly)
                            {
                                if (server.remoteClients[i].isUserDataLoaded == false)
                                {
                                    continue;
                                }
                            }
                            if (server.remoteClients[i].socket.Poll(1000, SelectMode.SelectError))
                            {
                                Console.WriteLine("casting failed:polled socket disconnected");
                                continue;
                            }
                            if (!server.remoteClients[i].socket.Poll(1000, SelectMode.SelectRead) || server.remoteClients[i].socket.Available > 0)
                            {
                                server.remoteClients[i].socket.Send(msg.GetBytes());
                            }
                            else
                            {
                                Console.WriteLine("casting failed:polled socket disconnected");
                            }

                        }
                    }


                    //   socket.Send(System.Text.Encoding.Default.GetBytes("&"));
                }
            }
            finally
            {

            }
        }
        public static void RemoveDisconnectedClients(IMultiplayerServer server)
        {

            lock (server.remoteClientsLock)
            {
                for (int i = 0; i < server.remoteClients.Count; i++)
                {

                    if (server.remoteClients[i].messageParser.isMessageParsingThreadRunning == false)
                    {
                        Console.WriteLine("removed client" + server.remoteClients[i].socket.RemoteEndPoint);
                        server.remoteClients.RemoveAt(i);
                        i--;
                    }
                }
            }
                
                


                //   socket.Send(System.Text.Encoding.Default.GetBytes("&"));
            
        }
        public static void UserLogout(RemoteClient client,IMultiplayerServer server)
        {
            if (client == null||client.socket.Connected==false)
            {
                return;
            }

            if (server.remoteClients.Contains(client))
            {
            
              //  allUserData.RemoveAt(index);
                Console.WriteLine(client.socket.RemoteEndPoint.ToString() + "  logged out");
                CastToAllClients( server ,new MessageProtocol(135, MessagePackSerializer.Serialize(server.allUserDatas)));
                lock (server.remoteClientsLock)
                {
                    server.remoteClients.Remove(client);
                }
                client.Close();
           

                Console.WriteLine("current socket clients count:"+server.remoteClients.Count);
            }
        }


        public static void UserLogin(RemoteClient client, byte[] data, IMultiplayerServer server)
        {
            if (!server.remoteClients.Contains(client))
            {
                return;
            }
            UserData u = MessagePackSerializer.Deserialize<UserData>(data);
            int idx = server.remoteClients.FindIndex(delegate (RemoteClient rl)
            {
                if (rl.isUserDataLoaded == false) return false;
                
                return rl.curUserData.userName == u.userName;

            });
            if (idx != -1)
            {
                Console.WriteLine("same username detected");
                SendToClient(client, new MessageProtocol((byte)MessageCommandType.UserLoginReturn, MessagePackSerializer.Serialize("Failed")));
                lock (server.remoteClientsLock)
                {
                    server.remoteClients.Remove(client);
                }
                //      client.socket.Close();
                client.Close();
              

                //   client.Close();
                Console.WriteLine("current socket clients count:" + server.remoteClients.Count);
            }
            else
            {

                client.curUserData = u;
                client.isUserDataLoaded=true;
                Console.WriteLine(client.socket.RemoteEndPoint + "Logged in");
                SendToClient(client, new MessageProtocol((byte)MessageCommandType.UserLoginReturn, MessagePackSerializer.Serialize("Success")));
             //   allClientSocketsOnline.Add(s);
              //  allUserData.Add(u);
                
                //     SendToClient(s, new Message("userCount", MessagePackSerializer.Serialize(allClientSocketsOnline.Count.ToString())));
                CastToAllClients(server, new MessageProtocol(135, MessagePackSerializer.Serialize(server.allUserDatas)));
            }
        }
    }
}
