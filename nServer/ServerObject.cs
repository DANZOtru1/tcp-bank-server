using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace nServer
{
    class ServerObject
    {
        private const int port = 1717; //Порт Сервера (можно изменить)
        private static TcpListener listener;
        private List<ClientObject> clients = new List<ClientObject>();

        internal void Listen()
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Console.WriteLine("[{0}]: Сервер запущен. Ожидание подключений...", DateTime.Now.ToString());

                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    NetworkStream stream = tcpClient.GetStream();
                    BinaryReader reader = new BinaryReader(stream);

                    string idClient = reader.ReadString();
                    Console.WriteLine("[{0}]: [{1}] Вошел в систему.", DateTime.Now.ToString(), idClient);
                    ClientObject clientObject = new ClientObject(idClient, tcpClient, this);

                    Task task = new Task(clientObject.Process);
                    task.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
    }
}
