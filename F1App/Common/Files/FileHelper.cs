using Exceptions;
using System.IO;

namespace Common.Files
{
    public static class FileHelper
    {
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static long GetFileSize(string path)
        {
            if (FileExists(path))
                return new FileInfo(path).Length;

            throw new FileStreamException("File does not exist");
        }

        public static string GetFileName(string path)
        {
            if (FileExists(path))
                return new FileInfo(path).Name;

            throw new FileStreamException("File does not exist");
        }
    }
}
