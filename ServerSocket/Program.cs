using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerSocket
{
    class Program
    {
        //public static void SendMessage()
        //{
        //    Socket socket = serverSocket.Accept();
        //    Console.WriteLine("Connected a client:{0}", socket.RemoteEndPoint);
        //    socket.Send(Encoding.ASCII.GetBytes("welcome to server"));
        //    //Thread thread = new Thread(ReceiveMessage);
        //    // thread.Start();
        //}

        //public static void ReceiveMessage(object obj)
        //{
        //    Socket socket = (Socket)obj;
        //    byte[] data = new byte[1024];
        //    int len = socket.Receive(data);
        //    string dataString = Encoding.ASCII.GetString(data, 0, len);
        //    Console.WriteLine("Receive Data:{0} from {1}", dataString, socket.RemoteEndPoint);
        //    //Thread thread = new Thread(SendMessage);
        //    //thread.Start(socket);
        //}


        //static Socket serverSocket;
        static void Main(string[] args)
        {

            int i = 0;
        }
    }
}
