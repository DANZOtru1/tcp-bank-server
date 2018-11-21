using System;
using System.Threading.Tasks;

namespace nServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerObject server = new ServerObject();
            Task task = Task.Run(new Action(server.Listen));
            task.Wait();
        }
    }
}
