using Exceptions;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Protocol
{
    public class SocketHandler
    {
        private readonly TcpClient _tcpClient;

        public SocketHandler(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public async Task SendAsync(byte[] data)
        {
            int offset = 0;
            int size = data.Length;

            try
            {
                var networkStream = _tcpClient.GetStream();
                await networkStream.WriteAsync(data, offset, size);
            }
            catch (Exception)
            {
                throw new ProtocolException("Connection lost");
            }
        }

        public async Task<byte[]> ReceiveAsync(int length)
        {
            int offset = 0;
            var data = new byte[length];

            try
            {
                var networkStream = _tcpClient.GetStream();

                while (offset < length)
                {
                    int received = await networkStream.ReadAsync(data, offset, length - offset);
                    offset += received;
                }
            }
            catch (Exception)
            {
                throw new ProtocolException("Connection lost");
            }
            
            return data;
        }
    }
}
