using System.Net;

namespace Common
{
    public class ServerSetting
    {
        public IPAddress ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string AdminName { get; set; }
        public string AdminPassword { get; set; }
        public string RabbitMQServerIP { get; set; }
        public string RabbitMQServerPort { get; set; }
        public string LogsQueueName { get; set; }
    }
}
