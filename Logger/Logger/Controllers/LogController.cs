using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logger.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Logger.Services;
using Logger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Logger.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly IHubContext<LogViewerHub> _logViewerHub;

        public LogController(ILogService logService, IHubContext<LogViewerHub> logViewerHub)
        {
            _logService = logService;
            _logViewerHub = logViewerHub;
        }

        [HttpGet]
        public IEnumerable<LogModel> GetLogList() => _logService.ListLogs();

        [HttpDelete("{logId}")]
        public async Task RemoveLog(string logId)
        {
            await _logService.RemoveLog(logId);
            await _logViewerHub.Clients.All.SendAsync(
                "RefreshLogsList",
                _logService.ListLogs());
        }

        [HttpGet("{logId}/plot")]
        public IEnumerable<PlotRequest> GetPlots(string logId) => _logService.GetPlots(logId);

        [HttpGet("{logId}")]
        public IEnumerable<RecordModel> GetLog(
            [FromRoute] string logId,
            [FromQuery(Name = "page")] int page,
            [FromQuery(Name = "items-per-page")] int itemsPerPage) =>
            _logService.GetLog(logId, page, itemsPerPage);

        [HttpGet("{logId}/time-navigation")]
        public RawLogNavigator GetLogByTime(
            [FromRoute] string logId,
            [FromQuery(Name = "time")] long time,
            [FromQuery(Name = "items-per-page")] int itemsPerPage) =>
            _logService.GetLogByTime(logId, itemsPerPage, DateTimeOffset.FromUnixTimeMilliseconds(time).UtcDateTime);
    }
}