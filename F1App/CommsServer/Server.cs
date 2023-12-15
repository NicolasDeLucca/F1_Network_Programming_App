using Common;
using CommsServer.Communications;

namespace CommsServer
{
    public class Server
    {
        private static ServerSetting _setting;

        public Server(ServerSetting setting)
        {
            _setting = setting;
        }

        public void StartServerCommunications()
        {
            Console.WriteLine("Launching server..");

            CommunicationsHandler communicationsHandler = new CommunicationsHandler(_setting);
            Task listenTask = new Task(async () => await communicationsHandler.ListenAsync());
            listenTask.Start();

            Console.WriteLine("Press any key to stop the server");
            Console.ReadLine();
            communicationsHandler.ShuttingDown();
        }
    }
}