using System.Threading.Tasks;

namespace Client
{
    public class ClientProgram
    {
        static async Task Main(string[] args)
        {
            UI cliUI = new UI();
            await cliUI.StartClient();
        }
    }
}
