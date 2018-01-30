using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Server
{
    class Program
    {
        static byte[] buffer = new byte[1024];
        private static int count = 0;

        static void Main(string[] args)
        {
            WriteLine("server:ready", ConsoleColor.Green); //绿色  

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
                socket.Send(Encoding.UTF8.GetBytes("server received data")); //默认Unicode  
                //接收下一个消息(因为这是一个递归的调用，所以这样就可以一直接收消息）异步  
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

        #region 扩展方法  
        public static void WriteLine(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[{0}] {1}", DateTime.Now.ToString("MM-dd HH:mm:ss"), str);
        }
        #endregion
    }
}