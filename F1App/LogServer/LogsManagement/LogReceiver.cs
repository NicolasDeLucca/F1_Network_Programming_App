using LogServer.Domain;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Protocol;

namespace LogServer.LogsManagement
{
    public class LogReceiver
    {
        private IModel _channel;
        private string _queueName;
        private LogRepository _logRepository;
        private LogProcessor _logProcessor;

        public LogReceiver(LogsServerConfiguration configuration)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = configuration.RabbitMQServerIP,
                Port = int.Parse(configuration.RabbitMQServerPort)
            };
            IConnection connection = connectionFactory.CreateConnection();
            _queueName = configuration.LogsQueueName;
            _channel = connection.CreateModel();
            _channel.QueueDeclare(_queueName, false, false, false, null);
            _logRepository = LogRepository.GetInstance();
            _logProcessor = new LogProcessor();
        }

        public void ReceiveServerLogs()
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Log processedLog = _logProcessor.ProcessLog(message);

                _logRepository.Store(processedLog);
                Console.WriteLine(processedLog.ToString());
            };

            _channel.BasicConsume(_queueName, true, consumer);
        }

    }
}
