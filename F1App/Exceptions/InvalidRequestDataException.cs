using System;

namespace Exceptions
{
    public class InvalidRequestDataException : Exception
    {
        public InvalidRequestDataException(string message) : base(message) { }
    }
}
