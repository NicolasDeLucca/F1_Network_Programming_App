using System;

namespace Exceptions
{
    public class FileStreamException : Exception
    {
        public FileStreamException(string message) : base(message) { }
    }
}
