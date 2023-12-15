using Domain;
using Newtonsoft.Json;
using Protocol;

namespace LogServer.LogsManagement
{
    public class LogProcessor
    {
        public Log ProcessLog(string receivedMessage)
        {
            string[] messageComponents = BuildDataHelper.GetDataParams(receivedMessage);
            Enum.TryParse(messageComponents[1], true, out Command parsedCommand);
            Type entityType = ExpectedEntityType(parsedCommand);

            return new Log()
            {
                CreatedAt = new DateTime(long.Parse(messageComponents[0])),
                Command = parsedCommand,
                EntityType = entityType,
                Entity = JsonConvert.DeserializeObject(messageComponents[2], entityType)
            };
        }

        private Type ExpectedEntityType(Command command)
        {
            Type expectedType = null;
            switch (command)
            {
                case Command.CreateMechanic:
                    expectedType = typeof(Mechanic);
                    break;
                case Command.SendMessage:
                    expectedType = typeof(Message);
                    break;
                case Command.ShowAllMessages:
                    expectedType = typeof(List<Message>);
                    break;
                case Command.ReadNewMessages:
                    expectedType = typeof(List<Message>);
                    break;
                case Command.CreateReplacement:
                    expectedType = typeof(Replacement);
                    break;
                case Command.CreateReplacementCategory:
                    expectedType = typeof(Category);
                    break;
                case Command.AddPhotoToReplacement:
                    expectedType = typeof(string);
                    break;
                case Command.ShowReplacement:
                    expectedType = typeof(Replacement);
                    break;
                case Command.ShowReplacementCategories:
                    expectedType = typeof(List<Category>);
                    break;
                case Command.DownloadReplacementPhoto:
                    expectedType = typeof(string);
                    break;
                case Command.ShowAllReplacements:
                    expectedType = typeof(List<Replacement>);
                    break;
                case Command.ShowReplacementsByKeywords:
                    expectedType = typeof(List<Replacement>);
                    break;
                case Command.Error:
                    expectedType = typeof(string);
                    break;
            };

            return expectedType;
        }
    }
}
