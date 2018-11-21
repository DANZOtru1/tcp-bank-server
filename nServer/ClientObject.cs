using System;
using System.IO;
using System.Net.Sockets;

namespace nServer
{
    class ClientObject
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
    }
}
