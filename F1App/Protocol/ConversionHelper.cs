using System.Text;
using System;

namespace Protocol
{
    public static class ConversionHelper
    {
        public static Command ConvertBytesToCommand(byte[] data)
        {
            return (Command) ConvertBytesToInt(data);
        }

        public static byte[] ConvertCommandToBytes(Command command)
        {
            return ConvertIntToBytes((int)command);
        }

        public static FrameHeader ConvertBytesToHeader(byte[] data)
        {
            return (FrameHeader)ConvertBytesToInt(data);
        }

        public static byte[] ConvertHeaderToBytes(FrameHeader header)
        {
            return ConvertIntToBytes((int)header);
        }

        public static byte[] ConvertStringToBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ConvertBytesToString(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        public static byte[] ConvertIntToBytes(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static int ConvertBytesToInt(byte[] value)
        {
            return BitConverter.ToInt32(value, 0);
        }

        public static byte[] ConvertLongToBytes(long value)
        {
            return BitConverter.GetBytes(value);
        }

        public static long ConvertBytesToLong(byte[] value)
        {
            return BitConverter.ToInt64(value, 0);
        }
    }
}
