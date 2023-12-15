using System.Net;
using System.Net.Sockets;
using Common;
using CommsServer.Services;
using DataAccess.Repositories;
using Domain;
using Exceptions;
using static Common.Definitions;

namespace CommsServer.Communications
{
    public class CommunicationsHandler
    {
        private const int _maxPendingCommunications = 10;

        private object _serverLocker = new object();
        private object _communicationsLocker = new object();
        private List<Communication> _communications;
        private NodeIPState _serverState;
        private readonly TcpListener _tcpListener;
        private LogService _logService;

        public CommunicationsHandler(ServerSetting settings)
        {
            IPAddress serverIP = settings.ServerIP;
            int serverPort = settings.ServerPort;
            _tcpListener = new TcpListener(serverIP, serverPort);

            _logService = new LogService(settings);

            SetAdmin(settings);
            _communications = new List<Communication>();
            _serverState = NodeIPState.Off;
        }

        public async Task ListenAsync()
        {
            _tcpListener.Start(_maxPendingCommunications);
            _serverState = NodeIPState.On;

            while (IsServerOn())
            {
                try
                {
                    TcpClient pendingClient = await _tcpListener.AcceptTcpClientAsync();
                    Communication clientCommunication = new Communication(pendingClient, _logService);

                    Task clientTask = new Task(async () => await HandleCommunication(clientCommunication));
                    AddIncomingCommunication(clientCommunication);
                    clientTask.Start();

                    Console.WriteLine("Client joined");
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Error while accepting connection {0}", e.Message);
                    ShutDownCommunications();

                    Console.WriteLine("All communications have been disconnected");
                }
            }
        }

        private async Task HandleCommunication(Communication clientCommunication)
        {
            try
            {
                await clientCommunication.StartAsync();
            }
            catch (SocketException)
            {
                RemoveClient(clientCommunication);
            }
            catch (ProtocolException)
            {
                RemoveClient(clientCommunication);
            }
        }

        private void RemoveClient(Communication clientCommunication)
        {
            lock (_communicationsLocker)
            {
                try
                {
                    clientCommunication.ShutDown();
                    _communications.Remove(clientCommunication);
                }
                catch (ObjectDisposedException) { }
            }

            Console.WriteLine("Client disconnected");
        }

        public void ShuttingDown()
        {
            lock (_serverLocker)
            {
                _serverState = NodeIPState.ShuttingDown;
                _tcpListener.Stop();
            }
        }

        #region Helpers

        private void ShutDownCommunications()
        {
            lock (_serverLocker)
            {
                _serverState = NodeIPState.Off;
                lock (_communicationsLocker)
                {
                    for (int i = _communications.Count; i > 0; i--)
                    {
                        try
                        {
                            Communication communication = _communications[i - 1];
                            communication.ShutDown();
                            _communications.RemoveAt(i - 1);
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
            }
        }

        private void AddIncomingCommunication(Communication communication)
        {
            lock (_communicationsLocker)
            {
                _communications.Add(communication);
            };
        }

        private bool IsServerOn()
        {
            lock (_serverLocker)
            {
                return _serverState == NodeIPState.On;
            }
        }

        private void SetAdmin(ServerSetting settings)
        {
            var Name = settings.AdminName;
            var Password = settings.AdminPassword;
            var admin = new Mechanic() { Name = Name, Password = Password };
            var _mechanicRepository = MechanicRepository.GetInstance();
            _mechanicRepository.Store(admin);
        }

        #endregion
    }
}
