using Common;
using Common.Managers;
using CommsServer.Services;
using Grpc.Core;
using Protocol;
using System.Net;

namespace MainServer.Services
{
    public class ReplacementService : Replacement.ReplacementBase
    {
        private static readonly SettingManager _settingManager = new SettingManager();
        private readonly ServiceRouter _serviceRouter;
        private const int adminId = 0;

        public ReplacementService()
        {
            ServerSetting settings = GetSettings();
            LogService logService = new LogService(settings); 
            _serviceRouter = ServiceRouter.GetInstance(logService);
        }

        public override Task<CreateReplacementReply> CreateReplacement(CreateReplacementRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame(Command.CreateReplacement);
            string dataSeparator = Constants.DataSeparator;
            requestFrame.Data = $"{request.Name}{dataSeparator}{request.Provider}{dataSeparator}{request.Brand}";
            requestFrame.Header = FrameHeader.Req;

            Frame responseFrame = _serviceRouter.GetApiResponse(requestFrame);

            if (responseFrame.Command == Command.Error)
                return Task.FromResult(new CreateReplacementReply { Message = responseFrame.Data, Ok = false });

            return Task.FromResult(new CreateReplacementReply { Message = responseFrame.Data, Ok = true });
        }

        public override Task<UpdateReplacementReply> UpdateReplacement(UpdateReplacementRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame(Command.UpdateReplacement);
            string dataSeparator = Constants.DataSeparator;
            requestFrame.Data = $"{request.Id}{dataSeparator}{request.Name}{dataSeparator}{request.Provider}{dataSeparator}{request.Brand}";
            requestFrame.Header = FrameHeader.Req;

            Frame responseFrame = _serviceRouter.GetApiResponse(requestFrame);

            if (responseFrame.Command == Command.Error)
                return Task.FromResult(new UpdateReplacementReply { Message = responseFrame.Data, Ok = false });

            return Task.FromResult(new UpdateReplacementReply { Message = responseFrame.Data, Ok = true });
        }

        public override Task<DeleteReplacementReply> DeleteReplacement(DeleteReplacementRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame(Command.RemoveReplacement);
            requestFrame.Data = request.Id.ToString();
            requestFrame.Header = FrameHeader.Req;

            Frame responseFrame = _serviceRouter.GetApiResponse(requestFrame);

            if (responseFrame.Command == Command.Error)
                return Task.FromResult(new DeleteReplacementReply { Message = responseFrame.Data, Ok = false });

            return Task.FromResult(new DeleteReplacementReply { Message = responseFrame.Data, Ok = true });
        }

        public override Task<DeleteReplacementPhotoReply> DeleteReplacementPhoto(DeleteReplacementPhotoRequest request, ServerCallContext context)
        {
            Frame requestFrame = new Frame(Command.RemoveReplacementPhoto);
            requestFrame.Data = request.Id.ToString();
            requestFrame.Header = FrameHeader.Req;

            Frame responseFrame = _serviceRouter.GetApiResponse(requestFrame);

            if (responseFrame.Command == Command.Error)
                return Task.FromResult(new DeleteReplacementPhotoReply { Message = responseFrame.Data, Ok = false });

            return Task.FromResult(new DeleteReplacementPhotoReply { Message = responseFrame.Data, Ok = true });
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

            return new ServerSetting()
            {
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