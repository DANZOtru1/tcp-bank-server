using System;
using System.Threading.Tasks;

namespace nServer
{
    internal class Program
    {
        #region Private methods

        private static void Main(string[] args)
        {
            ServerObject server = new ServerObject();
            Task task = Task.Run(new Action(server.Listen)); // спросить
            task.Wait();
        }

        #endregion
    }
}