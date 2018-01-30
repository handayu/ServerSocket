using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace TCP_Server
{
    class Program
    {

        private static MCData m_MCDataServer = null;

        static byte[] buffer = new byte[1024];
        private static int count = 0;

        public static void WriteLine(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[{0}] {1}", DateTime.Now.ToString("MM-dd HH:mm:ss"), str);
        }

        static void Main(string[] args)
        {
            #region 测试ESC数据获取
            //ECSDataHook data = new ECSDataHook("003517B2", "TStatusBar");
            //data.Start();

            //while (true)
            //{
            //    string resultInfo = data.GetNextESCEditOutInfo();
            //    WriteLine("ECS Result: " + resultInfo, ConsoleColor.Green); //绿色  

            //    System.Threading.Thread.Sleep(1000);
            //}

            #endregion

            WriteLine("server:ready", ConsoleColor.Green); //绿色  
            //WriteLine("请输入MC的公式编辑器的标题，以及目标输出窗口的类名,用于给客户端传送MC公式编辑器输出窗口中的定时数据！", ConsoleColor.Red); //绿色  
            //WriteLine("MC公式编辑器的标题(spy++获取):", ConsoleColor.Green); //绿色  
            //string strMCEditTitle = Console.ReadLine();
            //WriteLine("目标输出窗口的类名(spy++获取):", ConsoleColor.Green); //绿色  
            //string strTargetEditOut = Console.ReadLine();

            //try
            //{
            //    //m_MCDataServer = new MCData("0018298C", "RichEdit20W");
            //    m_MCDataServer = new MCData(strMCEditTitle, strTargetEditOut);

            //    m_MCDataServer.Start();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            #region 启动程序  
            //①创建一个新的Socket,这里我们使用最常用的基于TCP的Stream Socket（流式套接字）  
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //②将该socket绑定到主机上面的某个端口  
            //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.bind.aspx  
            socket.Bind(new IPEndPoint(IPAddress.Any, 61613));

            //③启动监听，并且设置一个最大的队列长度  
            //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.listen(v=VS.100).aspx  
            socket.Listen(10000);

            //④开始接受客户端连接请求  
            //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.beginaccept.aspx  
            socket.BeginAccept(new AsyncCallback(ClientAccepted), socket);
            Console.ReadLine();
            #endregion
        }

        #region 客户端连接成功  
        /// <summary>  
        /// 客户端连接成功  
        /// </summary>  
        /// <param name="ar"></param>  
        public static void ClientAccepted(IAsyncResult ar)
        {
            #region  
            //设置计数器  
            count++;
            var socket = ar.AsyncState as Socket;
            //这就是客户端的Socket实例，我们后续可以将其保存起来  
            var client = socket.EndAccept(ar);
            //客户端IP地址和端口信息  
            IPEndPoint clientipe = (IPEndPoint)client.RemoteEndPoint;

            WriteLine(clientipe + " is connected，total connects " + count, ConsoleColor.Yellow);

            //接收客户端的消息(这个和在客户端实现的方式是一样的）异步  
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), client);
            //准备接受下一个客户端请求(异步)  
            socket.BeginAccept(new AsyncCallback(ClientAccepted), socket);
            #endregion
        }
        #endregion

        #region 接收客户端的信息  
        /// <summary>  
        /// 接收某一个客户端的消息  
        /// </summary>  
        /// <param name="ar"></param>  
        public static void ReceiveMessage(IAsyncResult ar)
        {
            int length = 0;
            string message = "";
            var socket = ar.AsyncState as Socket;
            //客户端IP地址和端口信息  
            IPEndPoint clientipe = (IPEndPoint)socket.RemoteEndPoint;
            try
            {
                #region  
                //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.endreceive.aspx  
                length = socket.EndReceive(ar);
                //读取出来消息内容  
                message = Encoding.UTF8.GetString(buffer, 0, length);
                //输出接收信息  
                WriteLine(clientipe + " ：" + message, ConsoleColor.White);
                //服务器发送消息  
                while (true)
                {
                    //从MCData队列中取出最新的一条数据，如果没有为""，则重新取，如果有则发送到客户端
                    //string strInfo = m_MCDataServer.GetNextMCEditOutInfo();
                    //if (strInfo == string.Empty)
                    //{
                    //    continue;
                    //}
                    //else
                    //{
                    string strInfo = "Instrument:Cu1805 , OpenPrice:45110 ,LowPrice:45000 ,HighPrice:45300 ,ClosePrice: 45010 ,OpenShares:1 ,Rsi:87.5 ,Quantity:45,001.67";
                        socket.Send(Encoding.UTF8.GetBytes("\n" + strInfo + "\n")); //默认Unicode 
                        WriteLine("测试行情数据已经发送到客户端:" + strInfo, ConsoleColor.Red);

                    System.Threading.Thread.Sleep(1000);

                    //WriteLine("队列取出新的MC数据发送到客户端:" + strInfo, ConsoleColor.Red);

                    //}
                }

                //接收下一个消息(因为这是一个递归的调用，所以这样就可以一直接收消息）异步(因为是异步所以应该和上面的while不干扰)  
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
                #endregion
            }
            catch (Exception ex)
            {
                //设置计数器  
                count--;
                //断开连接  
                WriteLine(clientipe + " is disconnected，total connects " + (count), ConsoleColor.Red);
            }
        }
        #endregion

    }
}
