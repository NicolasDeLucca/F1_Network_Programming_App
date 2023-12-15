using Protocol;

namespace LogServer.Domain
{
    public class LogSearchCriteria
    {
        public DateTime? FromDate { get; set; }
        public string? KeyWord { get; set; }

        public bool MatchesCriteria(Log log)
        {
            if (FromDate == null && (KeyWord == "" || KeyWord == null))
                return true;

            bool matchesCriteria = false;

            if (FromDate != null)
            {
                matchesCriteria = matchesCriteria || log.CreatedAt.CompareTo(FromDate) >= 0;
            }

            if (KeyWord != "" && KeyWord != null)
            {
                matchesCriteria = matchesCriteria || log.ToString().Contains(KeyWord);
            }

            return matchesCriteria;
        }
    }
}
