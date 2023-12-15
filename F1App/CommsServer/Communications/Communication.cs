using System.Net.Sockets;
using Protocol;
using CommsServer.Services;
using static Common.Definitions;
using Domain;
using DataAccess.Repositories;
using Exceptions;

namespace CommsServer.Communications
{
    public class Communication
    {
        private object _communicationLocker;
        private Mechanic _mechanic;
        private IRepository<Mechanic> _mechanicRepository;
        private readonly TcpClient _tcpClient;
        private NodeIPState _communicationState;
        private readonly ProtocolHandler _protocolHandler;
        private readonly ServiceRouter _serviceRouter;
        private bool _isAuthenticated;

        public Communication(TcpClient tcpClient, LogService logService)
        {
            _tcpClient = tcpClient;
            _protocolHandler = new ProtocolHandler(_tcpClient);
            _serviceRouter = ServiceRouter.GetInstance(logService);
            _mechanicRepository = MechanicRepository.GetInstance();
            _communicationState = NodeIPState.Off;
            _communicationLocker = new object();
            _isAuthenticated = false;
        }

        public async Task StartAsync()
        {
            _communicationState = NodeIPState.On;

            while (!_isAuthenticated)
            {
                await HandleAuthentication();
            }

            while (ConnectionIsUp())
            {
                await HandleRequests();
            }
        }

        public void ShutDown()
        {
            _tcpClient.Close();

            lock (_communicationLocker)
            {
                _communicationState = NodeIPState.Off;
            }
        }

        #region Helpers

        private bool ConnectionIsUp()
        {
            lock (_communicationLocker)
            {
                return _communicationState == NodeIPState.On;
            }
        }

        private async Task HandleAuthentication()
        {
            Frame requestFrame = await _protocolHandler.ReceiveAsync();
            Frame responseFrame = _serviceRouter.Authenticate(requestFrame);
            responseFrame.Header = FrameHeader.Res;

            try
            {
                if (responseFrame.Command == Command.LogIn)
                {
                    int mechanicId = int.Parse(responseFrame.Data);
                    _mechanic = _mechanicRepository.Get(mechanicId);
                    _isAuthenticated = true;
                    responseFrame.Data = "Successfully logged in as " + _mechanic.Name;
                }
            }
            catch (FormatException)
            {
                responseFrame = new Frame(Command.Error) { Data = "Incorrect input, please write a number", Header = FrameHeader.Res };
            }
            catch (ResourceNotFoundException)
            {
                responseFrame = new Frame(Command.Error) { Data = "Error while trying to fetch user", Header = FrameHeader.Res };
            }
            catch (Exception e)
            {
                responseFrame = new Frame(Command.Error) { Data = e.Message, Header = FrameHeader.Res };
            }

            await _protocolHandler.SendAsync(responseFrame);
        }

        private async Task HandleRequests()
        {
            try
            {
                Frame requestFrame = await _protocolHandler.ReceiveAsync();
                Frame responseFrame = _serviceRouter.GetResponse(requestFrame, _mechanic);
                responseFrame.Header = FrameHeader.Res;

                await _protocolHandler.SendAsync(responseFrame);
            }
            catch (FileStreamException e)
            {
                Frame responseFrame = new Frame(Command.Error) { Data = e.Message, Header = FrameHeader.Res };
                await _protocolHandler.SendAsync(responseFrame);
            }
        }

        #endregion
    }
}
