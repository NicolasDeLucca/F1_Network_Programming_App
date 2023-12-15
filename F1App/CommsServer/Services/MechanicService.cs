using DataAccess.Repositories;
using Domain;
using Exceptions;
using Protocol;

namespace CommsServer.Services
{
    public class MechanicService
    {
        private LogService _logService;
        private MechanicRepository _mechanicRepository;
        private const int adminId = 0;

        public MechanicService(LogService logService)
        {
            _mechanicRepository = MechanicRepository.GetInstance();
            _logService = logService;
        }

        public Frame PostMechanic(Frame requestFrame, int userId)
        {
            if (userId == adminId)
            {
                string[] dataFrame = requestFrame.GetDataParams();
                string mechanicName = dataFrame[0];
                string mechanicPassword = dataFrame[1];

                Mechanic createdMechanic = new Mechanic
                {
                    Name = mechanicName,
                    Password = mechanicPassword
                };

                _mechanicRepository.Store(createdMechanic);
                _logService.EmitEntityLog(createdMechanic, requestFrame.Command);

                return new Frame(requestFrame.Command) { Data = "Mechanic successfully created" };
            }
            else
            {
                throw new InvalidRequestDataException("Only the admin can create mechanics");
            }
        }

        public Frame PostMechanicMessage(Frame requestFrame, int mechanicId)
        {
            string[] dataFrame = requestFrame.GetDataParams();
            int recieverMechanicId = int.Parse(dataFrame[0]);
            string textMessage = dataFrame[1];

            Message createdMessage = new Message(mechanicId) { Text = textMessage };

            _mechanicRepository.StoreNewMessage(recieverMechanicId, createdMessage);
            _logService.EmitEntityLog(createdMessage, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = "Message sent!" };
        }

        public Frame IndexMechanicMessages(Frame requestFrame, int mechanicId)
        {
            var retrievedMessages = _mechanicRepository.GetAllMessages(mechanicId);
            _logService.EmitEntityLog(retrievedMessages, requestFrame.Command);

            var showedMessages = retrievedMessages.Select(m => m.ToString()).ToArray();
            string data = string.Join("\n", showedMessages);

            return new Frame(requestFrame.Command) { Data = data };
        }

        public Frame ReadNewMessages(Frame requestFrame, int mechanicId)
        {
            var retrievedMessages = _mechanicRepository.GetAllMessages(mechanicId);
            var unReadMessages = retrievedMessages.AsEnumerable().Where(m => m.Read == false).ToList();

            foreach (var message in unReadMessages)
            {
                _mechanicRepository.ReadStoredMessage(mechanicId, message.Id);
            }

            _logService.EmitEntityLog(unReadMessages, requestFrame.Command);

            var showedMessages = unReadMessages.Select(m => m.ToString()).ToArray();
            string data = string.Join("\n", showedMessages);

            return new Frame(requestFrame.Command) { Data = data };
        }

        public Frame Authenticate(Frame requestFrame)
        {
            if (requestFrame.Command != Command.LogIn)
                throw new InvalidRequestDataException("Not valid command for login");

            string[] dataFrame = requestFrame.GetDataParams();
            string loginName = dataFrame[0];
            string loginPassword = dataFrame[1];

            Mechanic mechanic = _mechanicRepository.GetAll().Find(m => m.Name == loginName);

            if (mechanic == null)
                throw new InvalidRequestDataException("User name not found");

            if (mechanic.Password != loginPassword)
                throw new InvalidRequestDataException("Invalid password");

            return new Frame(requestFrame.Command) { Data = mechanic.Id.ToString() };
        }
    }
}

