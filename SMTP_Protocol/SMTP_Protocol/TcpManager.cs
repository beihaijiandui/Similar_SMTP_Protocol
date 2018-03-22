using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SMTP_Protocol
{
    public class TcpManager
    {
        private Socket serverSocket;
        private String IP;
        private int port;
        bool serverRunning;
        public TcpManager(string ipAddress, int port)
        {
            this.IP = ipAddress;
            this.port = port;
        }

        public void startServer(int backlog)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress serverIP = IPAddress.Parse(IP);
            IPEndPoint serverhost = new IPEndPoint(serverIP, port);
            serverSocket.Bind(serverhost);
            serverSocket.Listen(backlog);//侦听队列的长度
            serverRunning = true;
            ThreadPool.QueueUserWorkItem((obj)=>
            {
                while (serverRunning)
                {
                    try
                    {
                        Socket client = serverSocket.Accept();
                        ThreadPool.QueueUserWorkItem(ClientProcess, client);
                        Console.WriteLine("I'm thread:{0},I create a new socket", System.Threading.Thread.CurrentThread.ManagedThreadId);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exit the server");
                        break;
                    }
                }
                serverSocket.Close();
            }
                );
        }

        private void ClientProcess(object socket)
        {
            Socket client = (Socket)socket;
            try
            {
                if (ConnectionEstablished != null)
                {
                    ConnectionEstablished(this.serverSocket, new SocketEventArgs(client));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                client.Close();
            }
        }
        public event EventHandler<SocketEventArgs> ConnectionEstablished;
    }

    public class SocketEventArgs:EventArgs
    {
        public Socket Socket;
        public SocketEventArgs(Socket socket)
        {
            this.Socket = socket;
        }
    } 
}
