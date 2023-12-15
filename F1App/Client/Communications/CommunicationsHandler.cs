using System.Net.Sockets;
using System.Net;
using Protocol;
using System.IO;
using static Common.Definitions;
using Common.Managers;
using Exceptions;
using System;
using System.Threading.Tasks;

namespace Client.Communications
{
    public class CommunicationsHandler
    {
        private static readonly SettingManager _settingManager = new SettingManager();
        private readonly TcpClient _tcpClient;

        private NodeIPState _clientState;
        private IPEndPoint _serverEndPoint;
        private ProtocolHandler _protocolHandler;

        public CommunicationsHandler()
        {
            IPAddress serverIP = IPAddress.Parse(_settingManager.ReadSettings(ClientConfig.ServerIPKey));
            int serverPort = int.Parse(_settingManager.ReadSettings(ClientConfig.ServerPortKey));
            IPAddress clientIP = IPAddress.Parse(_settingManager.ReadSettings(ClientConfig.ClientIPKey));

            _serverEndPoint = new IPEndPoint(serverIP, serverPort);

            IPEndPoint clientEndPoint = new IPEndPoint(clientIP, 0);
            _tcpClient = new TcpClient(clientEndPoint);
            _protocolHandler = new ProtocolHandler(_tcpClient);

            _clientState = NodeIPState.Off;
        }

        public async Task ConnectToServer()
        {
            await _tcpClient.ConnectAsync(_serverEndPoint.Address, _serverEndPoint.Port);
            _clientState = NodeIPState.On;
        }

        public void ShutDown()
        {
            _clientState = NodeIPState.ShuttingDown;
            _tcpClient.Close();
            _clientState = NodeIPState.Off;
        }

        public bool IsClientStateOn()
        {
            return _clientState == NodeIPState.On;
        }

        public async Task<Frame> SendRequest(Frame requestFrame)
        {
            Frame response;

            try
            {
                await _protocolHandler.SendAsync(requestFrame);
                response = await _protocolHandler.ReceiveAsync();
            }
            catch (IOException)
            {
                response = new Frame(Command.Error) {Data = "Server is down"};
            }
            catch (InvalidRequestDataException e)
            {
                response = new Frame(Command.Error) {Data = e.Message};
            }
            catch (ResourceNotFoundException e)
            {
                response = new Frame(Command.Error) {Data = e.Message};
            }
            catch (FileStreamException e)
            {
                response = new Frame(Command.Error) {Data = e.Message};
            }
            catch (FormatException)
            {
                response = new Frame(Command.Error) {Data = "Incorrect input, please write a number"};
            }
            catch (ProtocolException e)
            {
                response = new Frame(Command.Error) {Data = e.Message};
            }

            return response;
        }
    }
}
