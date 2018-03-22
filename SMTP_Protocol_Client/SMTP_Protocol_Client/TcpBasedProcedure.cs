using Me;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SMTP_Protocol_Client
{
    public class TcpBasedProcedure
    {
        public const int MessageSizeWidth = 8;
        public void SendMessage(Socket socket, MessageEntity me)
        {
            IFormatter serializer = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, me);
            byte[] data=ms.ToArray();
            long length = data.Length;
            if (length == 0)
                return;
            socket.Send(BitConverter.GetBytes(length));
            socket.Send(data);
        }
        public MessageEntity ReceiveMessage(Socket socket)
        {
            byte[] data = new byte[1024];
            int readCount=socket.Receive(data, MessageSizeWidth, SocketFlags.None);
            long length=BitConverter.ToInt64(data, 0);
            if (length <=0)
                return null;
            int rev = 0;
            MemoryStream ms = new MemoryStream();
            int size = (int)length - rev;
            while ((readCount = socket.Receive(data, size, SocketFlags.None)) != 0)
            {
                ms.Write(data, 0, readCount);//将数据从缓冲区读到流中去
                rev += readCount;
                if (rev >= length)//完整的读完一条消息后退出循环
                    break;
                size = (int)length - rev;
            }
            IFormatter serializer = new BinaryFormatter();
            ms.Position = 0;
            MessageEntity me = serializer.Deserialize(ms) as MessageEntity;
            ms.Close();
            return me;
        }

        public void DoProcedureCore(Socket _socket)
        {
            byte[] data = new byte[1024];
            SendMessage(_socket, new MessageEntity(MessageEntity.CommandType.ServerReady, null));
            MessageEntity hello = ReceiveMessage(_socket);
            System.Diagnostics.Debug.WriteLine("Received:{0}\n",hello.Command);

            SendMessage(_socket, new MessageEntity(MessageEntity.CommandType.OK, null));
            MessageEntity r = null;

            while (true)
            {
                r = ReceiveMessage(_socket);
                if (r.Command == MessageEntity.CommandType.Quit)
                    break;
                else if (r.Command == MessageEntity.CommandType.Data)
                {
                    string str = (string)r.Data;
                    System.Diagnostics.Debug.WriteLine("Received:{0}\n", str);
                }
            }
            if (r.Command == MessageEntity.CommandType.Quit)
                SendMessage(_socket, new MessageEntity(MessageEntity.CommandType.AgreeDisconnect, null));
            System.Diagnostics.Debug.WriteLine("Received；{0}\n", r.Command);
        }
    }
}
