using Me;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SMTP_Protocol_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Connect("127.0.0.1");
            Console.ReadKey();
        }
        static void Connect(string serverIP)
        {
            try
            {
                Int32 port = 13000;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(serverIP);
                IPEndPoint host = new IPEndPoint(ip, port);
                socket.Connect(host);
                TcpBasedProcedure tbp = new TcpBasedProcedure();
                MessageEntity me = tbp.ReceiveMessage(socket);
                Console.WriteLine("received:{0} ready", me.Command);
                me = new MessageEntity(MessageEntity.CommandType.HelloServer, "Hello " + serverIP);
                tbp.SendMessage(socket, me);
                me = tbp.ReceiveMessage(socket);
                Console.WriteLine("received:{0} ok", me.Command);
                string str = null;
                while ((str = Console.ReadLine()) != "close")
                {
                    tbp.SendMessage(socket, new MessageEntity(MessageEntity.CommandType.Data, str));
                    Console.WriteLine("Sent: {0}", str);
                }
                tbp.SendMessage(socket, new MessageEntity(MessageEntity.CommandType.Quit, null));
                me = tbp.ReceiveMessage(socket);
                Console.WriteLine("received:{0} agree close", me.Command);
                Console.WriteLine("close connection...\n Press any key continue");
                socket.Close();
                Console.ReadKey();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
