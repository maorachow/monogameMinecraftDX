
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using MessagePack;
using Microsoft.Xna.Framework;
using monogameMinecraftNetworking;
using monogameMinecraftNetworking.Data;
using monogameMinecraftNetworking.Protocol;
using monogameMinecraftShared.Core;

public class Program
{
    public static UserData userData = new UserData(0, 100, 0, 0, 0, 0, "abc", false);

    static IPAddress ip = IPAddress.Parse("127.0.0.1");
    static int port = 11111;
    static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public static MessageParser messageParser;


    public static void SendMessageToServer(MessageProtocol m)
    {
        try
        {
            clientSocket.Send(m.GetBytes());
            //        clientSocket.Send(System.Text.Encoding.UTF8.GetBytes("&"));
        }
        catch (Exception e)
        {
        Console.WriteLine("Sending message failed " + e);
           

        }
    }
    public static bool isLoggedIn=false;
    public static void MessageParsingThread(Socket s)
    {
        MessageProtocol mp = null;
        int ReceiveLength = 0;
        byte[] staticReceiveBuffer = new byte[102400];  // 接收缓冲区(固定长度)
        byte[] dynamicReceiveBuffer = new byte[] { };  // 累加数据缓存(不定长)
                                                       //  byte[] bb = new byte[102400];
                                                       //  ArraySegment<byte> b= new ArraySegment<byte>(bb);
        while (true)
        {
           
            if (s == null || s.Connected == false)
            {
                Console.WriteLine("Recieve client failed:socket closed");
         //       isMessageParsingThreadRunning = false;
                return;
            }
            try
            {//public int Receive (System.Collections.Generic.IList<ArraySegment<byte>> buffers);

                //     int count =s.Receive(bb);


                // ReceiveLength = s.Receive(staticReceiveBuffer);


                ReceiveLength = s.Receive(staticReceiveBuffer);  // 同步接收数据
                dynamicReceiveBuffer = MathUtility.CombineBytes(dynamicReceiveBuffer, 0, dynamicReceiveBuffer.Length, staticReceiveBuffer, 0, ReceiveLength);  // 将之前多余的数据与接收的数据合并,形成一个完整的数据包
                if (ReceiveLength <= 0)  // 如果接收到的数据长度小于0(通常表示socket已断开,但也不一定,需要进一步判断,此处可以忽略)
                {


                    Console.WriteLine("收到0字节数据");
             //       isMessageParsingThreadRunning = false;
                    //  UserLogout(s);
                    return;  // 终止接收循环
                }
                else if (dynamicReceiveBuffer.Length < MessageProtocol.HEADLENGTH)  // 如果缓存中的数据长度小于协议头长度,则继续接收
                {
                    continue;  // 跳过本次循环继续接收数据
                }
                else  // 缓存中的数据大于等于协议头的长度(dynamicReadBuffer.Length >= 6)
                {
                    var headInfo = MessageProtocol.GetHeadInfo(dynamicReceiveBuffer);  // 解读协议头的信息
                    while (dynamicReceiveBuffer.Length - MessageProtocol.HEADLENGTH >= headInfo.DataLength)  // 当缓存数据长度减去协议头长度大于等于实际数据的长度则进入循环进行拆包处理
                    {
                        mp = new MessageProtocol(dynamicReceiveBuffer);  // 拆包
                                                                         //       mainForm.LogOnTextbox("Message:"+mp.Command);
                        dynamicReceiveBuffer = mp.moreData;  // 将拆包后得出多余的字节付给缓存变量,以待下一次循环处理数据时使用,若下一次循环缓存数据长度不能构成一个完整的数据包则不进入循环跳到外层循环继续接收数据并将本次得出的多余数据与之合并重新拆包,依次循环。
                        headInfo = MessageProtocol.GetHeadInfo(dynamicReceiveBuffer);  // 从缓存中解读出下一次数据所需要的协议头信息,已准备下一次拆包循环,如果数据长度不能构成协议头所需的长度,拆包结果为0,下一次循环则不能成功进入,跳到外层循环继续接收数据合并缓存形成一个完整的数据包

                        //   parsedMessages.Enqueue(new MessageProtocol(mp.command,(byte[])mp.messageData.Clone()));
                 /*       lock (server.todoListLock)
                        {
                            if (server.serverTodoLists.Count > 0)
                            {
                                server.serverTodoLists[0].value.Enqueue(new ValueTuple<RemoteClient, MessageProtocol>(remoteClient, new MessageProtocol(mp.command, (byte[])mp.messageData.Clone())), 0);
                            }
                        }*/
                    Console.WriteLine("Message received:"+(MessageCommandType)mp.command);

                    if (mp.command == (byte)MessageCommandType.UserLoginReturn)
                    {
                        string result = MessagePackSerializer.Deserialize<string>(mp.messageData);
                        Console.WriteLine("login return:"+result);
                        if (result == "Success")
                        {
                            isLoggedIn = true;
                        }
                    }

                    } // 拆包循环结束
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection stopped : " + ex.ToString());
             
                break;
            }
        }
    }
    public static void Main()
    {
        //Console.WriteLine("hello world!");

        if (Console.ReadKey() != null)
        {
            clientSocket.Connect(ip, port);
         //   currentPlayer = new UserData(rand.NextSingle() * 10f, 100f, rand.NextSingle() * 10f, rand.NextSingle() * 10f, clientUserName);
    //        SendMessageToServer(new Message("Login", JsonConvert.SerializeObject(currentPlayer)));
       //     SendMessageToServer(new Message("ChunkGen", "null"));
            Thread thread = new Thread(() => { MessageParsingThread(clientSocket);});
            thread.Start();
        }
        while (true)
        {
            if (clientSocket.Connected == false)
            {
                Console.WriteLine("Disconnected");
                break;
            }

            //     Console.WriteLine("Enter Message");
            Console.WriteLine("Enter Message Type: 1 update user 2 set block 3 update world");
            char a = Console.ReadKey().KeyChar;
            switch (a)
            {
                case '1':
                   Console.WriteLine("user login");
                    Debug.WriteLine((byte)MessageCommandType.UserLogin);
            /*        userData.posX = float.Parse(Console.ReadLine());
                    userData.posY = float.Parse(Console.ReadLine());
                    userData.posZ = float.Parse(Console.ReadLine());
                    userData.rotY = float.Parse(Console.ReadLine());*/
            if (!isLoggedIn)
            {
                SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserLogin, MessagePackSerializer.Serialize(userData)));
                    }
            else
            {
                Console.WriteLine("already logged in");
            }
                   
                    break;
                case '2':
                    Console.WriteLine("user logout");
                //    BlockModifyData b = new BlockModifyData(float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), float.Parse(Console.ReadLine()), Int32.Parse(Console.ReadLine()));


                    SendMessageToServer(new MessageProtocol((byte)MessageCommandType.UserLogout,new byte[]{}));
                    break;
                case '3':
            //        SendMessageToServer(new Message("ChunkGen", "null"));
                    break;
            }
            //    clientSocket.Send(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Message(Console.ReadLine(), Console.ReadLine()))));
            Console.WriteLine("Message sent");
        }
       

    }
}