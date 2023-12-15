using LogServer.Domain;
using LogServer.Models;
using Microsoft.AspNetCore.Mvc;
using Protocol;

namespace LogServer.Controllers
{
    [ApiController]
    [Route("logapi")]
    public class LogsController : ControllerBase
    {
        [HttpGet("logs")]
        public IActionResult GetLogsBy([FromBody] LogRequestModel request)
        {
            LogSearchCriteria searchCriteria = new LogSearchCriteria() { KeyWord = request.keyword, FromDate = request.fromDate };
            List<Log> logsToReturn = LogRepository.GetInstance().GetBy(searchCriteria);
            List<LogResponseModel> response = logsToReturn.Select(l => new LogResponseModel(l)).ToList();

            return Ok(response);
        }
    }
}