using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace nServer
{
    /// <summary>
    ///     Объект сервера.
    /// </summary>
    public class ServerObject
    {
        #region Static fields

        /// <summary>
        ///     Класс TcpListener прослушивает входящие подключения по определенному порту.
        /// </summary>
        private static TcpListener _listener;

        #endregion

        #region Private fields

        /// <summary>
        ///     Список классов для создания клиентской программы, работающей по протоколу TCP.
        /// </summary>
        private readonly List<ClientObject> _clients = new List<ClientObject>();

        /// <summary>
        ///     Порт.
        /// </summary>
        private readonly int _port = 1717; //Порт Сервера (можно изменить)

        #endregion

        #region Constructor

        /// <summary>
        ///     Конструктор.
        /// </summary>
        public ServerObject()
        {
            //_port = GetPort();
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Отключить сервер.
        /// </summary>
        private void Disconnect()
        {
            _listener.Stop();
            foreach (ClientObject client in _clients)
            {
                RemoveConnection(client.Id);
                client.Close();
            }

            Console.WriteLine("[{0}]: Сервер остановлен.", DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Получить свободный порт на компьютере.
        /// </summary>
        /// <returns>Порт</returns>
        private int GetPort()
        {
            Random random = new Random();
            int port = random.Next(1000, 2000);
            bool isAvailable = true;
            do
            {
                IPGlobalProperties ipGlobalProperties =
                    IPGlobalProperties.GetIPGlobalProperties();

                TcpConnectionInformation[] tcpConnInfoArray =
                    ipGlobalProperties.GetActiveTcpConnections();

                foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                {
                    if (tcpi.LocalEndPoint.Port == port)
                    {
                        isAvailable = false;
                        port++;
                        break;
                    }
                }
            }
            while (!isAvailable);

            return port;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Сервер начинает прослушивать сообщения.
        /// </summary>
        public void Listen()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
                _listener.Start();
                Console.WriteLine("[{0}]: Сервер запущен. Ожидание подключений...",
                    DateTime.Now.ToString(CultureInfo.InvariantCulture));

                while (true)
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    NetworkStream stream = tcpClient.GetStream();
                    BinaryReader reader = new BinaryReader(stream);

                    string idClient = reader.ReadString();
                    Console.WriteLine("[{0}]: [{1}] Вошел в систему.",
                        DateTime.Now.ToString(CultureInfo.InvariantCulture), idClient);

                    ClientObject clientObject =
                        new ClientObject(idClient, tcpClient, this);

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

        /// <summary>
        ///     Добавить подключение.
        /// </summary>
        /// <param name="clientObject">Клиент.</param>
        public void AddConnection(ClientObject clientObject)
        {
            if (clientObject == null)
            {
                throw new ArgumentNullException(nameof(clientObject));
            }

            _clients.Add(clientObject);
        }

        /// <summary>
        ///     Разрываем подключение.
        /// </summary>
        /// <param name="id">ИД пользователя.</param>
        public void RemoveConnection(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            ClientObject client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                _clients.Remove(client);
                Console.WriteLine("[{0}]: [{1}] Отключен сервером.",
                    DateTime.Now.ToString(CultureInfo.InvariantCulture), id);
            }
        }

        /// <summary>
        ///     Отправить деньги.
        /// </summary>
        /// <param name="sendId">ИД клиента.</param>
        /// <param name="sendSum">Сумма.</param>
        public void SendMoney(string sendId, decimal sendSum)
        {
            if (string.IsNullOrWhiteSpace(sendId))
            {
                throw new ArgumentNullException(nameof(sendId));
            }

            if (sendSum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sendId),
                    "Невозможно передать отрицательную или нулевую сумму");
            }

            foreach (ClientObject client in _clients)
            {
                if (client.Id == sendId)
                {
                    client.Writer.Write(sendSum);
                    client.Writer.Flush();
                    Console.WriteLine("[{0}]: На счет [{1}] было отправлено [{2}].",
                        DateTime.Now.ToString(CultureInfo.InvariantCulture), sendId, sendSum);
                }
            }
        }

        #endregion
    }
}