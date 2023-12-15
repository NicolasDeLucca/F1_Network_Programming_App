using System.IO;
using System.Threading.Tasks;
using Exceptions;

namespace Common.Files
{
    public static class FileStreamHandler
    {
        public static async Task<byte[]> ReadAsync(string path, long offset, int length)
        {
            if (FileHelper.FileExists(path))
            {
                var data = new byte[length];

                using (var filestream = new FileStream(path, FileMode.Open) { Position = offset })
                {
                    var bytesRead = 0;
                    while (bytesRead < length)
                    {
                        var read = await filestream.ReadAsync(data, bytesRead, length - bytesRead);
                        if (read == 0)
                            throw new FileStreamException("Error reading file");
                        bytesRead += read;
                    }
                }
                return data;
            }

            throw new FileStreamException("File does not exist");
        }

        public static async Task WriteAsync(string fileName, byte[] data)
        {
            var fileMode = FileHelper.FileExists(fileName) ? FileMode.Append : FileMode.Create;
            using (var filestream = new FileStream(fileName, fileMode))
            {
                await filestream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
