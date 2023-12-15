using System;

namespace Exceptions
{
    public class ProtocolException : Exception
    {
        public ProtocolException(string message) : base(message) { }
    }
}
