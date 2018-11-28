using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace nServer
{
    public class ServerObject
    {
        private int port; //Порт Сервера (можно изменить)
        private static TcpListener listener;
        private List<ClientObject> clients = new List<ClientObject>();

        public ServerObject()
        {
           port = GetPort();
        }

        public void Listen()
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


        public void AddConnection(ClientObject clientObject)
        {
            if(clientObject == null)
            {
                throw new ArgumentNullException(nameof(clientObject));
            }
            clients.Add(clientObject);
        }

        public void RemoveConnection(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                clients.Remove(client);
                Console.WriteLine("[{0}]: [{1}] Отключен сервером.", DateTime.Now.ToString(), id);
            }
        }

        public void SendMoney(string sendId, decimal sendSum)
        {
            if (string.IsNullOrWhiteSpace(sendId))
            {
                throw new ArgumentNullException(nameof(sendId));
            }

            if(sendSum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sendId), "Невозможно передать отрицательную или нулевую сумму");
            }

            foreach (ClientObject client in clients)
            {
                if (client.Id == sendId)
                {
                    client.Writer.Write(sendSum);
                    client.Writer.Flush();
                    Console.WriteLine("[{0}]: На счет [{1}] было отправлено [{2}].", DateTime.Now.ToString(), sendId, sendSum);
                }
            }
        }

        private void Disconnect()
        {
            listener.Stop();
            foreach (ClientObject client in clients)
            {
                RemoveConnection(client.Id);
                client.Close();
            }
            Console.WriteLine("[{0}]: Сервер остановлен.", DateTime.Now.ToString());
        }

        private int GetPort()
        {
            Random random = new Random();
            int port = random.Next(1000, 2000);
            bool isAvailable = true;
            do
            {
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                {
                    if (tcpi.LocalEndPoint.Port == port)
                    {
                        isAvailable = false;
                        port++;
                        break;
                    }
                }
            } while (!isAvailable);
            return port;
        }
    }
}
