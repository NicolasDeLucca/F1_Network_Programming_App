using Common.Files;
using Exceptions;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Protocol
{
    public class ProtocolHandler
    { 
        private readonly SocketHandler _socketHandler;
        private readonly StreamHandler _streamHandler; 

        public ProtocolHandler(TcpClient tcpClient)
        {
            _socketHandler = new SocketHandler(tcpClient);
            _streamHandler = new StreamHandler(tcpClient);
        }

        public async Task SendAsync(Frame frame)
        {           
            if (frame.IsPhotoRequest() || frame.IsPhotoResponse())
            {
                string[] dataParams = frame.GetDataParams();
                string photoPath = dataParams[1];

                if (FileHelper.FileExists(photoPath))
                {
                    await SendFrame(frame);
                    await SendFile(photoPath);
                }
                else
                {
                    throw new InvalidRequestDataException("File does not exist");
                }
            }
            else
            {
                await SendFrame(frame);
            }
        }

        public async Task<Frame> ReceiveAsync()
        {
            byte[] receivedCommand = await _socketHandler.ReceiveAsync(Constants.FixedCommandSize);
            Command command = ConversionHelper.ConvertBytesToCommand(receivedCommand);

            byte[] receivedHeader = await _socketHandler.ReceiveAsync(Constants.FixedHeaderSize);
            FrameHeader header = ConversionHelper.ConvertBytesToHeader(receivedHeader);

            byte[] receivedDataLength = await _socketHandler.ReceiveAsync(Constants.FixedDataLenghtSize);
            int dataLength = ConversionHelper.ConvertBytesToInt(receivedDataLength);
            byte[] receivedData = await _socketHandler.ReceiveAsync(dataLength);
            string data = ConversionHelper.ConvertBytesToString(receivedData);

            Frame requestFrame = new Frame(command);
            requestFrame.Data = data;
            requestFrame.Header = header;

            if (requestFrame.IsPhotoRequest() || requestFrame.IsPhotoResponse())
            {
                await ReceiveFile();
            }

            return requestFrame;
        }

        private async Task SendFrame(Frame frame)
        {
            byte[] command = ConversionHelper.ConvertCommandToBytes(frame.Command);
            byte[] header = ConversionHelper.ConvertHeaderToBytes(frame.Header);
            byte[] data = ConversionHelper.ConvertStringToBytes(frame.Data);
            byte[] dataLength = ConversionHelper.ConvertIntToBytes(data.Length);

            await _socketHandler.SendAsync(command);
            await _socketHandler.SendAsync(header);
            await _socketHandler.SendAsync(dataLength);
            await _socketHandler.SendAsync(data);
        }

        private async Task SendFile(string path)
        {
            if (FileHelper.FileExists(path))
            {
                var fileName = FileHelper.GetFileName(path);

                await _socketHandler.SendAsync(ConversionHelper.ConvertIntToBytes(fileName.Length));
                await _socketHandler.SendAsync(ConversionHelper.ConvertStringToBytes(fileName));

                long fileSize = FileHelper.GetFileSize(path);
                var convertedFileSize = ConversionHelper.ConvertLongToBytes(fileSize);

                await _socketHandler.SendAsync(convertedFileSize);
                await _streamHandler.SendFileByStream(fileSize, path);
            }
            else
            {
                throw new InvalidRequestDataException("File does not exist");
            }
        }

        private async Task ReceiveFile()
        {
            byte[] receivedFileNameSize = await _socketHandler.ReceiveAsync(Constants.FixedDataLenghtSize);
            int fileNameSize = ConversionHelper.ConvertBytesToInt(receivedFileNameSize);

            byte[] receivedFileName = await _socketHandler.ReceiveAsync(fileNameSize);
            string fileName = ConversionHelper.ConvertBytesToString(receivedFileName);

            byte[] receivedFileSize = await _socketHandler.ReceiveAsync(Constants.FixedFileSize);
            long fileSize = ConversionHelper.ConvertBytesToLong(receivedFileSize);

            await _streamHandler.ReceiveFileByStream(fileSize, fileName);
        }
    }
}
