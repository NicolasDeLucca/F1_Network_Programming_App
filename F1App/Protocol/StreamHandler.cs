using Common.Files;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Protocol
{
    public class StreamHandler
    {
        private readonly SocketHandler _socketHandler;

        public StreamHandler(TcpClient tcpClient)
        {
            _socketHandler = new SocketHandler(tcpClient);
        }

        public async Task SendFileByStream(long fileSize, string path)
        {
            long fileParts = Constants.GetNumberOfChunks(fileSize);
            long offset = 0;
            long currentChunk = 1;

            while (offset < fileSize)
            {
                byte[] data;
                if (currentChunk == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = await FileStreamHandler.ReadAsync(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await FileStreamHandler.ReadAsync(path, offset, Constants.MaxDataSize);
                    offset += Constants.MaxDataSize;
                }
                await _socketHandler.SendAsync(data);
                currentChunk++;
            }
        }

        public async Task ReceiveFileByStream(long fileSize, string fileName)
        {
            long fileParts = Constants.GetNumberOfChunks(fileSize);
            long offset = 0;
            long currentChunk = 1;

            while (offset < fileSize)
            {
                byte[] data;
                if (currentChunk == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = await _socketHandler.ReceiveAsync(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await _socketHandler.ReceiveAsync(Constants.MaxDataSize);
                    offset += Constants.MaxDataSize;
                }

                await FileStreamHandler.WriteAsync(fileName, data);
                currentChunk++;
            }
        }
    }
}
