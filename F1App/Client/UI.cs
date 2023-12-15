using Client.Communications;
using Protocol;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client
{
    public class UI
    {
        private CommunicationsHandler _communicationsHandler;

        public UI()
        {
            _communicationsHandler = new CommunicationsHandler();
        }

        public async Task StartClient()
        {
            await _communicationsHandler.ConnectToServer();
            Console.WriteLine("Client connected to server!\n");

            await LogInAsync();

            while (_communicationsHandler.IsClientStateOn())
            {
                Frame request = null;
                DisplayMenu();

                Command option = GetOption();
                switch (option)
                {
                    case Command.CreateMechanic:
                        request = CommandHandler.CreateMechanic(option);
                        break;
                    case Command.SendMessage:
                        request = CommandHandler.SendMessage(option);
                        break;
                    case Command.ShowAllMessages:
                        request = CommandHandler.GetAllMessages(option);
                        break;
                    case Command.ReadNewMessages:
                        request = CommandHandler.ReadNewMessages(option);
                        break;
                    case Command.CreateReplacement:
                        request = CommandHandler.CreateReplacement(option);
                        break;
                    case Command.CreateReplacementCategory:
                        request = CommandHandler.CreateReplacementCategory(option);
                        break;
                    case Command.AddPhotoToReplacement:
                        request = CommandHandler.AddPhotoToReplacement(option);
                        break;
                    case Command.ShowReplacement:
                        request = CommandHandler.GetReplacement(option);
                        break;
                    case Command.ShowReplacementCategories:
                        request = CommandHandler.GetReplacementCategories(option);
                        break;
                    case Command.DownloadReplacementPhoto:
                        request = CommandHandler.GetReplacementPhoto(option);
                        break;
                    case Command.ShowAllReplacements:
                        request = CommandHandler.GetAllReplacements(option);
                        break;
                    case Command.ShowReplacementsByKeywords:
                        request = CommandHandler.SearchReplacementByKeywords(option);
                        break;
                    case Command.LogOut:
                        _communicationsHandler.ShutDown();
                        break;
                    default:
                        Console.WriteLine("No valid command received");
                        break;
                }

                if (request != null)
                {
                    request.Header = FrameHeader.Req;
                    Frame response = await _communicationsHandler.SendRequest(request);

                    Console.WriteLine("\n-------------------------------------\n");
                    Console.WriteLine(response.ShowResponse());
                    Console.WriteLine("\n-------------------------------------\n");

                    if (response.Data.Contains("Server is down"))
                    {
                        _communicationsHandler.ShutDown();
                    }
                }
            }
        }

        #region Helpers

        private async Task LogInAsync()
        {
            bool authenticated = false;

            while (!authenticated)
            {
                Console.WriteLine("\nLogin with your account");

                Frame request = CommandHandler.LogIn();
                Frame response = await _communicationsHandler.SendRequest(request);
                
                if (response.Command == Command.LogIn)
                    authenticated = true;

                Console.WriteLine("\n" + response.Data);
            }
        }

        private void DisplayMenu()
        {
            Array commands = Enum.GetValues(typeof(Command));

            Console.WriteLine("\n===== F1 User menu =====:");
            foreach (int c in commands)
            {
                if (IsDisplayableOption(c))
                {
                    string enumName = Enum.GetName(typeof(Command), c);
                    Console.WriteLine($"{c} - {SplitEnumName(enumName)}");
                }
            }

            DisplayLogOut();
            Console.WriteLine("\nChoose an option:");
        }

        private void DisplayLogOut()
        {
            string enumName = Enum.GetName(typeof(Command), Command.LogOut);
            int logoutValue = (int) Command.LogOut;
            Console.WriteLine($"\n{logoutValue} - {SplitEnumName(enumName)}");
        }

        private string SplitEnumName(string enumName)
        {
            return Regex.Replace(enumName, "(?<=[^A-Z])(?=[A-Z])", " ");
        }

        private Command GetOption()
        {
            int option;

            try
            {
                option = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                option = -1;
            }

            return (Command) option;
        }

        private bool IsDisplayableOption(int option)
        {
            return option != (int)Command.LogOut && option != (int)Command.Error && option != (int)Command.LogIn && IsNotApiCommand(option);
        }

        private bool IsNotApiCommand(int option)
        {
            return option != (int)Command.UpdateReplacement && option != (int)Command.RemoveReplacement && option != (int)Command.RemoveReplacementPhoto;
        }

        #endregion
    }
}
