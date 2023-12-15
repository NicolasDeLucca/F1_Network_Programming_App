using Domain;
using Exceptions;
using Protocol;

namespace CommsServer.Services
{
    public class ServiceRouter
    {
        private MechanicService _mechanicService;
        private ReplacementService _replacementService;
        private LogService _logService;
        private static object _instanceLocker = new object();
        private static ServiceRouter _instance;

        public ServiceRouter(LogService logService)
        {
            _mechanicService = new MechanicService(logService);
            _replacementService = new ReplacementService(logService);
            _logService = logService;
        }

        public static ServiceRouter GetInstance(LogService logService)
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                    _instance = new ServiceRouter(logService);

                return _instance;
            }
        }

        public Frame GetResponse(Frame requestFrame, Mechanic currentUser)
        {
            Frame responseFrame = null;

            try
            {
                switch (requestFrame.Command)
                {
                    case Command.CreateMechanic:
                        responseFrame = _mechanicService.PostMechanic(requestFrame, currentUser.Id);
                        break;
                    case Command.SendMessage:
                        responseFrame = _mechanicService.PostMechanicMessage(requestFrame, currentUser.Id);
                        break;
                    case Command.ShowAllMessages:
                        responseFrame = _mechanicService.IndexMechanicMessages(requestFrame, currentUser.Id);
                        break;
                    case Command.ReadNewMessages:
                        responseFrame = _mechanicService.ReadNewMessages(requestFrame, currentUser.Id);
                        break;
                    case Command.CreateReplacement:
                        responseFrame = _replacementService.PostReplacement(requestFrame);
                        break;
                    case Command.CreateReplacementCategory:
                        responseFrame = _replacementService.PostReplacementCategory(requestFrame);
                        break;
                    case Command.AddPhotoToReplacement:
                        responseFrame = _replacementService.PostReplacementPhoto(requestFrame);
                        break;
                    case Command.ShowReplacement:
                        responseFrame = _replacementService.ShowReplacement(requestFrame);
                        break;
                    case Command.ShowAllReplacements:
                        responseFrame = _replacementService.IndexReplacements(requestFrame);
                        break;
                    case Command.ShowReplacementCategories:
                        responseFrame = _replacementService.ShowReplacementCategories(requestFrame);
                        break;
                    case Command.DownloadReplacementPhoto:
                        responseFrame = _replacementService.ShowReplacementPhoto(requestFrame);
                        break;
                    case Command.ShowReplacementsByKeywords:
                        responseFrame = _replacementService.IndexReplacementsByKeyWords(requestFrame);
                        break;
                    default:
                        responseFrame = new Frame(Command.Error) { Data = "Incorrect Command" };
                        break;
                }
            }
            catch (IOException)
            {
                responseFrame = new Frame(Command.Error) { Data = "Communication has been shutted down\n" };
            }
            catch (ObjectDisposedException)
            {
                responseFrame = new Frame(Command.Error) { Data = "" };
            }
            catch (InvalidRequestDataException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }
            catch (ResourceNotFoundException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }
            catch (FormatException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }

            if (responseFrame != null && responseFrame.Command == Command.Error)
            {
                _logService.EmitEntityLog(responseFrame.Data, responseFrame.Command);
            }

            return responseFrame;
        }

        public Frame GetApiResponse(Frame requestFrame)
        {
            Frame responseFrame = null;

            try
            {
                switch (requestFrame.Command)
                {
                    case Command.CreateReplacement:
                        responseFrame = _replacementService.PostReplacement(requestFrame);
                        break;
                    case Command.UpdateReplacement:
                        responseFrame = _replacementService.UpdateReplacement(requestFrame);
                        break;
                    case Command.RemoveReplacement:
                        responseFrame = _replacementService.RemoveReplacement(requestFrame);
                        break;
                    case Command.RemoveReplacementPhoto:
                        responseFrame = _replacementService.RemoveReplacementPhoto(requestFrame);
                        break;
                    default:
                        responseFrame = new Frame(Command.Error) { Data = "Incorrect Command" };
                        break;
                }
            }
            catch (IOException)
            {
                responseFrame = new Frame(Command.Error) { Data = "Communication has been shutted down\n" };
            }
            catch (ObjectDisposedException)
            {
                responseFrame = new Frame(Command.Error) { Data = "" };
            }
            catch (InvalidRequestDataException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }
            catch (ResourceNotFoundException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }
            catch (FormatException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }

            if (responseFrame != null && responseFrame.Command == Command.Error)
            {
                _logService.EmitEntityLog(responseFrame.Data, responseFrame.Command);
            }

            return responseFrame;
        }

        public Frame Authenticate(Frame loginFrame)
        {
            Frame responseFrame;

            try
            {
                responseFrame = _mechanicService.Authenticate(loginFrame);
            }
            catch (InvalidRequestDataException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }
            catch (ResourceNotFoundException e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message };
            }

            return responseFrame;
        }
    }
}
