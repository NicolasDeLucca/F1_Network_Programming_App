using Common.Managers;
using Common;
using CommsServer;
using System.Net;

namespace MainServer
{
    public class Program
    {
        private static readonly SettingManager _settingManager = new SettingManager();

        public static void Main(string[] args)
        {
            var setting = GetSettings();
            Server server = new Server(setting);
            StartCommsServer(server);


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<Services.ReplacementService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }

        public static async Task StartCommsServer(Server server)
        {
            Console.WriteLine("Server will start accepting connections from the clients");
            await Task.Run(() => server.StartServerCommunications());
        }

        private static ServerSetting GetSettings()
        {
            IPAddress ServerIP = IPAddress.Parse(_settingManager.ReadSettings(ServerConfig.ServerIPKey));
            int ServerPort = int.Parse(_settingManager.ReadSettings(ServerConfig.ServerPortKey));
            string AdminName = _settingManager.ReadSettings(ServerConfig.AdminNameKey);
            string AdminPassword = _settingManager.ReadSettings(ServerConfig.AdminPasswordKey);
            string RabbitMQServerIP = _settingManager.ReadSettings(ServerConfig.RabbitMQServerIP);
            string RabbitMQServerPort = _settingManager.ReadSettings(ServerConfig.RabbitMQServerPort);
            string LogsQueueName = _settingManager.ReadSettings(ServerConfig.LogsQueueName);

            return new ServerSetting() {
                ServerIP = ServerIP,
                ServerPort = ServerPort,
                AdminName = AdminName,
                AdminPassword = AdminPassword,
                RabbitMQServerIP = RabbitMQServerIP,
                RabbitMQServerPort = RabbitMQServerPort,
                LogsQueueName = LogsQueueName,
            };
        }
    }
}