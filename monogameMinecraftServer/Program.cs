using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using System.Diagnostics;
using monogameMinecraftNetworking.World;

public class Program
{
    public static void Main()
    {
        //Console.WriteLine("hello world!");

        MultiplayerServer server = new MultiplayerServer();
        server.Initialize();
        server.Start();

        bool isGoingToQuit=false;
        while (true)
        {
            if (isGoingToQuit)
            {
                Console.WriteLine("quit main thread");

                break;
            }
            Console.WriteLine("Enter Message Type: 1 query user 2 query chunks 3 loaded client sockets X shutdown server");
            char a = Console.ReadKey().KeyChar;
            switch (a)
            {
                case '1':
                    Console.WriteLine(" ");
                    Console.WriteLine("loaded users");
                    Console.WriteLine(server.allUserDatas.Count);
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