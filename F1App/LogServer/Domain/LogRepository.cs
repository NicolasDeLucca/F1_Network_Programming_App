using Protocol;

namespace LogServer.Domain
{
    public class LogRepository
    {
        private List<Log> _logs;
        private object _logsLocker;
        private static object _instanceLocker = new object();
        private static LogRepository _instance;

        private LogRepository()
        {
            _logs = new List<Log>();
            _logsLocker = new object();
        }

        public static LogRepository GetInstance()
        {
            lock (_instanceLocker)
            {
                if (_instance == null)
                    _instance = new LogRepository();

                return _instance;
            }
        }

        public List<Log> GetBy(LogSearchCriteria searchCriteria)
        {
            lock (_logsLocker)
            {
                return _logs.FindAll(l => searchCriteria.MatchesCriteria(l));
            }
        }

        public void Store(Log log)
        {
            lock (_logsLocker)
            {
                Log newLog = new Log()
                {
                    Command = log.Command,
                    CreatedAt = log.CreatedAt,
                    Entity = log.Entity,
                    EntityType = log.EntityType
                };
                _logs.Add(newLog);
            }
        }
    }
}
