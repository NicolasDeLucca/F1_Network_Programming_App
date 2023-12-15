namespace LogServer
{
    public class LogsServerConfiguration
    {
        public string RabbitMQServerIP { get; set; }
        public string RabbitMQServerPort { get; set; }
        public string LogsQueueName { get; set; }
    }
}
