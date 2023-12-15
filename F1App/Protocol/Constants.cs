namespace Protocol
{
    public static class Constants
    {
        public const string DataSeparator = "|";

        public static readonly int FixedCommandSize = 4;
        public static readonly int FixedHeaderSize = 4;
        public static readonly int FixedDataLenghtSize = 4; 
        public static readonly int FixedFileSize = 8;
        public static readonly int MaxDataSize = 8192;

        public static long GetNumberOfChunks(long fileSize)
        {
            long numberOfChunks = fileSize / MaxDataSize;
            if (numberOfChunks * MaxDataSize < fileSize)
                numberOfChunks++;

            return numberOfChunks;
        }
    }
}
