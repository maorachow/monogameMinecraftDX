using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using System.Diagnostics;
using System.Net;
using monogameMinecraftNetworking.World;

public class Program
{
    public static void Main()
    {
        //Console.WriteLine("hello world!");

        MultiplayerServer server = new MultiplayerServer();

        Console.WriteLine("input server IP");
        string? s=Console.ReadLine();
        while (s == null ||(IPAddress.TryParse(s, out _) == false) )
        {
            Console.WriteLine("invalid IP");
            s = Console.ReadLine();
        }
        Console.WriteLine("input server port");
        string? portString = Console.ReadLine();
        while (portString == null || (int.TryParse(portString, out _) == false))
        {
            Console.WriteLine("invalid port");
            portString = Console.ReadLine();
        }

        int port = 11111;
      
        if (portString != null)
        {
             port = int.Parse(portString);
        }

        server.Initialize(s,port);
        server.Start();

        bool isGoingToQuit=false;
        while (true)
        {
            if (isGoingToQuit)
            {
                Console.WriteLine("quit main thread");

                break;
            }
            Console.WriteLine("Enter Operation Type: 1 query user, 2 query chunks, 3 loaded client sockets, 4 ban username, 5 unban username, 6 get banned players, X shutdown server");
             char a = Console.ReadKey().KeyChar;
            switch (a)
            {
                case '1':
                    Console.WriteLine(" ");
                    Console.WriteLine("loaded users");
                    Console.WriteLine(server.allUserDatas.Count);
                    foreach (var item in server.allUserDatas)
                    {
                        Console.WriteLine(item.userName);
                    }
                    /*        userData.posX = float.Parse(Console.ReadLine());
                            userData.posY = float.Parse(Console.ReadLine());
                            userData.posZ = float.Parse(Console.ReadLine());
                            userData.rotY = float.Parse(Console.ReadLine());*/
                
                    break;
                case '2':
                    Console.WriteLine(" ");
                    Console.WriteLine("loaded chunks");
                    //    BlockModifyData b = new BlockModifyData(float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), Int32.Parse(Console.ReadLine()));
                    foreach (var world in ServerSideVoxelWorld.voxelWorlds)
                    {
                        Console.WriteLine("world :"+world.worldID+" loaded chunks:" +world.chunks.Count);
                    }
 
                    break;

                case '3':
                    Console.WriteLine(" ");
                    Console.WriteLine("loaded client sockets");
                    //    BlockModifyData b = new BlockModifyData(float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), Int32.Parse(Console.ReadLine()));

                    Console.WriteLine(server.remoteClients.Count);
                    break;
                case '4':
                    Console.WriteLine(" ");
                    Console.WriteLine("ban username");
                    string? userName = Console.ReadLine();
                    if (userName != null)
                    {
                        server.userAccessControllingManager.BanUser(userName);
                    }

                  

                    break;
                case '5':
                    Console.WriteLine(" ");
                    Console.WriteLine("unban username");
                    string? userName1 = Console.ReadLine();
                    if (userName1 != null)
                    {
                        server.userAccessControllingManager.UnbanUser(userName1);
                    }

                    break;

                case '6':
                    Console.WriteLine(" ");
                    Console.WriteLine("currently banned usernames:");
                    foreach (var item in server.userAccessControllingManager.GetBannedUsers())
                    {
                        Console.WriteLine(item);
                    }
                   
                    break;
                case 'X':
                    Console.WriteLine(" ");
                    Console.WriteLine("are you sure you are shutting down the server? enter Y to confirm");
                    char c = Console.ReadKey().KeyChar;
                    if (c == 'Y')
                    {
                        isGoingToQuit = true;
                        server.ShutDown();
                    }
                    //    BlockModifyData b = new BlockModifyData(float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), Int32.Parse(Console.ReadLine()));

                    
                    break;
            }
            //    clientSocket.Send(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Message(Console.ReadLine(), Console.ReadLine()))));
            Console.WriteLine(" ");
        }

       
        Console.ReadKey();

    }
}