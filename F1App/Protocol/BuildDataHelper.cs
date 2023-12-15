using System;

namespace Protocol
{
    public static class BuildDataHelper
    {
        public static string BuildData(string data)
        {
            var newData = ReceiveData();

            if (newData == "") return data;

            return data + Constants.DataSeparator + newData;
        }

        public static string[] GetDataParams(string data)
        {
            return data.Split(new string[] {Constants.DataSeparator}, StringSplitOptions.None);
        }

        public static string ReceiveData()
        {
            var exit = false;
            var returnData = "";

            while (!exit)
            {
                returnData = Console.ReadLine();

                if (returnData == "")
                {
                    Console.WriteLine("Can't be empty");
                    continue;
                }
                if (returnData.Contains(Constants.DataSeparator))
                {
                    Console.WriteLine("Can't be {0} (reserved character)", Constants.DataSeparator);
                    continue;
                }

                exit = true;
            }

            return returnData;
        }
    }
}
