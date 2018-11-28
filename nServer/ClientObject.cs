using System;
using System.IO;
using System.Net.Sockets;

namespace nServer
{
    public class ClientObject
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }
        public BinaryReader Reader { get; private set; }
        public BinaryWriter Writer { get; private set; }
        private TcpClient client;
        private ServerObject server;

        public ClientObject(string id, TcpClient tcpClient, ServerObject serverObject)
        {
            this.Id = id;
            this.client = tcpClient;
            server = serverObject;
            server.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                Reader = new BinaryReader(Stream);
                Writer = new BinaryWriter(Stream);
                string sendId;
                decimal sendSum;

                while (true)
                {
                    try
                    {
                        GetSendIdSum(out sendId, out sendSum);
                        server.SendMoney(sendId, sendSum);
                    }
                    catch
                    {
                        Console.WriteLine("[{0}]: [{1}] Покинул сервер.", DateTime.Now.ToString(), Id);
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
                server.RemoveConnection(this.Id);
                Close();
            }

        }

        private void GetSendIdSum(out string sendId, out decimal sendSum)
        {
            do
            {
                sendId = Reader.ReadString();
                sendSum = Reader.ReadDecimal();
            } while (Stream.DataAvailable);
        }

        public void Close()
        {
            if (Stream != null)
            {
                Stream.Close();
            }
            if (client != null)
            {
                client.Close();
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
    }
}
