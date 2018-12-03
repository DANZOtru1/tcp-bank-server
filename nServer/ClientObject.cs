using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace nServer
{
    /// <summary>
    ///     Класс клиента.
    /// </summary>
    public class ClientObject
    {
        #region Private fields

        /// <summary>
        ///     Класс для создания клиентской программы, работающей по протоколу TCP.
        /// </summary>
        private readonly TcpClient _client;

        /// <summary>
        ///     Объект сервера к которому прикреплен клиент.
        /// </summary>
        private readonly ServerObject _server;

        #endregion

        #region Properties

        /// <summary>
        ///     ИД.
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Основной поток данных для доступа к сети.
        /// </summary>
        public NetworkStream Stream { get; private set; }

        /// <summary>
        ///     Бинарный считыватель.
        /// </summary>
        public BinaryReader Reader { get; private set; }

        /// <summary>
        ///     Бинарный отправитель.
        /// </summary>
        public BinaryWriter Writer { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="id">ИД.</param>
        /// <param name="tcpClient">Клиент.</param>
        /// <param name="serverObject">Сервер.</param>
        public ClientObject(string id, TcpClient tcpClient, ServerObject serverObject)
        {
            Id = id;
            _client = tcpClient;
            _server = serverObject;
            _server.AddConnection(this);
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Получить данные.
        /// </summary>
        /// <param name="sendId">ИД отправителя.</param>
        /// <param name="sendSum">Получить отправленную сумму.</param>
        private void GetSendIdSum(out string sendId, out decimal sendSum)
        {
            do
            {
                sendId = Reader.ReadString();
                sendSum = Reader.ReadDecimal();
            }
            while (Stream.DataAvailable);
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Работа клиента, ожидает деньги.
        /// </summary>
        public void Process()
        {
            try
            {
                Stream = _client.GetStream();
                Reader = new BinaryReader(Stream);
                Writer = new BinaryWriter(Stream);
                string sendId;
                decimal sendSum;

                while (true)
                {
                    try
                    {
                        GetSendIdSum(out sendId, out sendSum);
                        _server.SendMoney(sendId, sendSum);
                    }
                    catch
                    {
                        Console.WriteLine("[{0}]: [{1}] Покинул сервер.",
                            DateTime.Now.ToString(CultureInfo.InvariantCulture), Id);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        /// <summary>
        ///     Завершить работу клиента.
        /// </summary>
        public void Close()
        {
            if (Stream != null)
            {
                Stream.Close();
            }

            if (_client != null)
            {
                _client.Close();
            }

            if (Writer != null)
            {
                Writer.Dispose();
            }

            if (Reader != null)
            {
                Reader.Dispose();
            }
        }

        #endregion
    }
}