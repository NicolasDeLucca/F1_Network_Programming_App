using Protocol;

namespace LogServer.Models
{
    public class LogResponseModel
    {
        public string Tag { get; set; }
        public DateTime CreatedAt { get; set; }

        public LogResponseModel(Log log)
        {
            Tag = log.ToString();
            CreatedAt = log.CreatedAt;
        }
    }
}
