using Common;
using Newtonsoft.Json;
using Protocol;
using RabbitMQ.Client;
using System.Text;

namespace CommsServer.Services
{
    public class LogService
    {
        private IModel _channel;
        private string _queueName;

        public LogService(ServerSetting setting)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = setting.RabbitMQServerIP,
                Port = Int32.Parse(setting.RabbitMQServerPort)
            };
            IConnection connection = connectionFactory.CreateConnection();
            _queueName = setting.LogsQueueName;
            _channel = connection.CreateModel();
            _channel.QueueDeclare(_queueName, false, false, false, null);
        }

        public void EmitEntityLog(dynamic entity, Command command)
        {
            EmitLog(JsonConvert.SerializeObject(entity), command);
        }

        public void EmitLog(string logMessage, Command command)
        {
            string messageToSend = $"{DateTime.Now.Ticks}{Protocol.Constants.DataSeparator}{command}{Protocol.Constants.DataSeparator}{logMessage}";
            byte[] body = Encoding.UTF8.GetBytes(messageToSend);

            _channel.BasicPublish("", _queueName, null, body);
        }
    }
}
