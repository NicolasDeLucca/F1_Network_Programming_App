using Protocol;
using System;

namespace Client
{
    public class CommandHandler
    {

        public static Frame CreateMechanic(Command option)
        {
            Console.Clear();
            string data;

            Console.WriteLine("Mechanic creation (admin user only)\n");

            Console.WriteLine("Choose a name for the mechanic");
            data = BuildDataHelper.ReceiveData();

            Console.WriteLine("Choose a password");
            data = BuildDataHelper.BuildData(data);

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame SendMessage(Command option)
        {
            Console.Clear();
            string data;

            Console.WriteLine("Write message for mechanic\n");

            Console.WriteLine("Write mechanic id to send message to");
            data = BuildDataHelper.ReceiveData();

            Console.WriteLine("Write message");
            data = BuildDataHelper.BuildData(data);

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame GetAllMessages(Command option)
        {
            Console.Clear();
            Console.WriteLine("Showing all messages of mechanic in inbox\n");
            
            return new Frame(option);
        }

        public static Frame GetReplacementCategories(Command option)
        {
            Console.Clear();

            Console.WriteLine("Get replacement categories info\n");

            Console.WriteLine("Write replacement id");
            string data = BuildDataHelper.ReceiveData();

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame GetReplacementPhoto(Command option)
        {
            Console.Clear();

            Console.WriteLine("Download replacement photo\n");

            Console.WriteLine("Write replacement id");
            string data = BuildDataHelper.ReceiveData();

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame ReadNewMessages(Command option)
        {
            Console.Clear();
            Console.WriteLine("Showing all unread messages in inbox\n");

            return new Frame(option);
        }

        public static Frame CreateReplacement(Command option)
        {
            Console.Clear();
            string data;

            Console.WriteLine("Create new replacement\n");

            Console.WriteLine("Write name for new replacement");
            data = BuildDataHelper.ReceiveData();

            Console.WriteLine("Write provider for new replacement");
            data = BuildDataHelper.BuildData(data);

            Console.WriteLine("Write brand for new replacement");
            data = BuildDataHelper.BuildData(data);

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame CreateReplacementCategory(Command option)
        {
            Console.Clear();
            string data;

            Console.WriteLine("Create new replacement category\n");

            Console.WriteLine("Write replacement id to add category to");
            data = BuildDataHelper.ReceiveData();

            Console.WriteLine("Write category name");
            data = BuildDataHelper.BuildData(data);

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame AddPhotoToReplacement(Command option)
        {
            Console.Clear();
            string data;

            Console.WriteLine("Add photo to replacement\n");

            Console.WriteLine("Write replacement id to add photo to");
            data = BuildDataHelper.ReceiveData();

            Console.WriteLine("Write photo file path");
            data = BuildDataHelper.BuildData(data);

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame GetReplacement(Command option)
        {
            Console.Clear();

            Console.WriteLine("Get replacement info\n");

            Console.WriteLine("Write replacement id");
            string data = BuildDataHelper.ReceiveData();

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame GetAllReplacements(Command option)
        {
            Console.Clear();
            Console.WriteLine("Showing all unread messages in inbox\n");

            return new Frame(option);
        }

        public static Frame SearchReplacementByKeywords(Command option)
        {
            Console.Clear();
            Console.WriteLine("Get replacements by keywords\n");

            Console.WriteLine("Write comma separated keywords");
            string data = BuildDataHelper.ReceiveData();

            Frame requestFrame = new Frame(option);
            requestFrame.Data = data;

            return requestFrame;
        }

        public static Frame LogIn()
        {
            string data;
            Console.WriteLine("Username:");
            data = BuildDataHelper.ReceiveData();

            Console.WriteLine("Password:");
            data = BuildDataHelper.BuildData(data);

            Frame requestFrame = new Frame(Command.LogIn);
            requestFrame.Header = FrameHeader.Req;
            requestFrame.Data = data;

            return requestFrame;
        }
    }
}
