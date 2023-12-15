using LogServer.LogsManagement;

namespace LogServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            LogsServerConfiguration logsServerConfiguration = new LogsServerConfiguration()
            {
                RabbitMQServerIP = config.GetSection("LogsServerConfiguration").GetSection("RabbitMQServerIP").Value,
                RabbitMQServerPort = config.GetSection("LogsServerConfiguration").GetSection("RabbitMQServerPort").Value,
                LogsQueueName = config.GetSection("LogsServerConfiguration").GetSection("LogsQueueName").Value
            };

            SetupLogListener(logsServerConfiguration);

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        public static void SetupLogListener(LogsServerConfiguration configuration)
        {
            LogReceiver logReceiver = new LogReceiver(configuration);
            logReceiver.ReceiveServerLogs();
        }
    }
}