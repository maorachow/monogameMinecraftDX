using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using monogameMinecraftNetworking.Client;
using monogameMinecraftShared.Core;

namespace monogameMinecraftNetworking.Protocol
{
    public class MessageParser
    {
       

        public RemoteClient remoteClient;
 //       public Queue<MessageProtocol> parsedMessages;
        public IMultiplayerServer server;

        public bool isMessageParsingThreadRunning=false;


        public bool isThreadsStopping=false;
        public Thread messageParsingThread;
        public MessageParser(IMultiplayerServer server, RemoteClient remoteClient)
        {
           
            this.server = server;
            this.remoteClient = remoteClient;
            //    parsedMessages=new Queue<MessageProtocol>();
        }

        public void Start()
        {
            messageParsingThread = new Thread(() => {Console.WriteLine("message parsing started"); MessageParsingThread(remoteClient.socket); });
            messageParsingThread.Start();
            isMessageParsingThreadRunning=true;
            isThreadsStopping = false;
        }

        public void Stop()
        {
            isThreadsStopping=true;
         //   messageParsingThread.Join();
        }
        public void MessageParsingThread(Socket s)
        {
            MessageProtocol mp = null;
            int ReceiveLength = 0;
            byte[] staticReceiveBuffer = new byte[102400];  // 接收缓冲区(固定长度)
            byte[] dynamicReceiveBuffer = new byte[] { };  // 累加数据缓存(不定长)
                                                           //  byte[] bb = new byte[102400]
                                                           // //  ArraySegment<byte> b= new ArraySegment<byte>(bb);
          //     s.ReceiveTimeout = 1000;
            while (true)
            {
                if (isThreadsStopping == true)
                {
                    Console.WriteLine("message parsing thread stopped");
                    return;
                }
                if (s == null || s.Connected == false)
                {
                    Console.WriteLine("Recieve client failed:socket closed");
                    isMessageParsingThreadRunning = false;
                    return;
                }
                try
                {//public int Receive (System.Collections.Generic.IList<ArraySegment<byte>> buffers);

                    //     int count =s.Receive(bb);


                    // ReceiveLength = s.Receive(staticReceiveBuffer);
               /*     if ((!s.Poll(1000, SelectMode.SelectRead) || s.Available > 0) == false)
                    {
                        Console.WriteLine("polled socket disconnected");
                        isMessageParsingThreadRunning = false;
                        return;
                    }*/

                    ReceiveLength = s.Receive(staticReceiveBuffer);  // 同步接收数据

              //      Console.WriteLine("data length:"+ReceiveLength);
                    dynamicReceiveBuffer = MathUtility.CombineBytes(dynamicReceiveBuffer, 0, dynamicReceiveBuffer.Length, staticReceiveBuffer, 0, ReceiveLength);  // 将之前多余的数据与接收的数据合并,形成一个完整的数据包
                    if (ReceiveLength <= 0)  // 如果接收到的数据长度小于0(通常表示socket已断开,但也不一定,需要进一步判断,此处可以忽略)
                    {


                        Console.WriteLine("收到0字节数据");
                    //    isMessageParsingThreadRunning = false;
                        //  UserLogout(s);
                  //      return;  // 终止接收循环
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
                            Debug.WriteLine((MessageCommandType)mp.command);
                            //   parsedMessages.Enqueue(new MessageProtocol(mp.command,(byte[])mp.messageData.Clone()));
                       //     lock (server.todoListLock)
                     //    {
                           
                             if (server.serverTodoLists.Count > 0)
                             {
                               Utility.NetworkingUtility.EnqueueTodoList(server.serverTodoLists, new ValueTuple<RemoteClient, MessageProtocol>(remoteClient, new MessageProtocol(mp.command, (byte[])mp.messageData.Clone())));
                             //   server.serverTodoLists[0].value.Enqueue(new ValueTuple<RemoteClient, MessageProtocol>(remoteClient, new MessageProtocol(mp.command, (byte[])mp.messageData.Clone())), 0);
                             }
                      //      } 
                       
                       
                        } // 拆包循环结束
                    }
 

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection stopped : " + ex.ToString());
                    isMessageParsingThreadRunning = false;
                    break;
                }
            }
        }
    }


    public class MessageParserSingleSocket
    {


        public Socket socket;
        //       public Queue<MessageProtocol> parsedMessages;
        public ConcurrentQueue<MessageProtocol> todoList;

        public object todoListLock;
        public bool isMessageParsingThreadRunning = false;


        public bool isThreadsStopping = false;
        public Thread messageParsingThread;
        public MessageParserSingleSocket(ConcurrentQueue<MessageProtocol> todoList, Socket s, object todoListLock)
        {

            this.todoList = todoList;
            this.socket = s;
            this.todoListLock = todoListLock;
            //    parsedMessages=new Queue<MessageProtocol>();
        }

        public void Start()
        {
            messageParsingThread = new Thread(() => { Console.WriteLine("message parsing started"); MessageParsingThread(socket); });
            messageParsingThread.IsBackground=true;
            messageParsingThread.Start();
            isMessageParsingThreadRunning = true;
            isThreadsStopping = false;
        }

        public void Stop()
        {
            isThreadsStopping = true;
            //   messageParsingThread.Join();
        }
        public void MessageParsingThread(Socket s)
        {
            MessageProtocol mp = null;
            int ReceiveLength = 0;
            byte[] staticReceiveBuffer = new byte[102400];  // 接收缓冲区(固定长度)
            byte[] dynamicReceiveBuffer = new byte[] { };  // 累加数据缓存(不定长)
                                                           //  byte[] bb = new byte[102400]
                                                           // //  ArraySegment<byte> b= new ArraySegment<byte>(bb);
                                                           //     s.ReceiveTimeout = 1000;
            while (true)
            {
                if (isThreadsStopping == true)
                {
                    Debug.WriteLine("message parsing thread stopped");
                    return;
                }
                if (s == null || s.Connected == false)
                {
                    Debug.WriteLine("Recieve client failed:socket closed");
                    isMessageParsingThreadRunning = false;
                    return;
                }
                try
                {//public int Receive (System.Collections.Generic.IList<ArraySegment<byte>> buffers);

                    //     int count =s.Receive(bb);


                    // ReceiveLength = s.Receive(staticReceiveBuffer);
          /*          if ((!s.Poll(1000, SelectMode.SelectRead) || s.Available > 0) == false)
                    {
                        Console.WriteLine("polled socket disconnected");
                        return;
                    }*/

                    ReceiveLength = s.Receive(staticReceiveBuffer);  // 同步接收数据

                    //      Console.WriteLine("data length:"+ReceiveLength);
                    dynamicReceiveBuffer = MathUtility.CombineBytes(dynamicReceiveBuffer, 0, dynamicReceiveBuffer.Length, staticReceiveBuffer, 0, ReceiveLength);  // 将之前多余的数据与接收的数据合并,形成一个完整的数据包
                    if (ReceiveLength <= 0)  // 如果接收到的数据长度小于0(通常表示socket已断开,但也不一定,需要进一步判断,此处可以忽略)
                    {


                        Debug.WriteLine("收到0字节数据");
                        isMessageParsingThreadRunning = false;
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

                            if ((MessageCommandType)mp.command == MessageCommandType.WorldData)
                            {
                                Debug.WriteLine("world data");
                            }
                           // Debug.WriteLine((MessageCommandType)mp.command);

                            //   parsedMessages.Enqueue(new MessageProtocol(mp.command,(byte[])mp.messageData.Clone()));
                            

                                
                                 todoList.Enqueue(new MessageProtocol(mp.command, (byte[])mp.messageData.Clone()));
                                 
                            


                        } // 拆包循环结束
                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Connection stopped : " + ex.ToString());
                    isMessageParsingThreadRunning = false;
                    break;
                }
            }
        }
    }
}
