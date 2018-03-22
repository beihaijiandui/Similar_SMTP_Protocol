using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SMTP_Protocol
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpManager tm = new TcpManager("127.0.0.1", 13000);
            tm.ConnectionEstablished += Test;
            tm.startServer(10);
            Console.ReadKey();
        }
        public static void Test(object sender, SocketEventArgs e)
        {
            Socket s = e.Socket;
            TcpBasedProcedure tp = new TcpBasedProcedure();
            tp.DoProcedureCore(s);
        }
    }
}
