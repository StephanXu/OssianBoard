using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logger.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Logger.Services;
using Logger.Models;
using Logger.Protos;
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
        private readonly IArgumentsService _argumentsService;
        private readonly IHubContext<LogViewerHub> _logViewerHub;

        public LogController(
            ILogService logService,
            IArgumentsService argumentsService,
            IHubContext<LogViewerHub> logViewerHub)
        {
            _logService = logService;
            _argumentsService = argumentsService;
            _logViewerHub = logViewerHub;
        }

        [HttpGet]
        public IEnumerable<LogModel> GetLogList() => _logService.ListLogs();

        [HttpDelete("{logId}")]
        public async Task RemoveLog([FromRoute] string logId)
        {
            await _logService.RemoveLog(logId);
        }

        [HttpPut("{logId}")]
        public async Task UpdateLog(
            [FromRoute] string logId,
            [FromBody] UpdateLogRequestModel model) =>
            await _logService.UpdateLogMeta(logId, model.Name, model.Description);

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

        [HttpGet("{logId}/argument")]
        public ArgumentSnapshotModel GetArgumentSnapshot([FromRoute] string logId) =>
            _argumentsService.GetSingleSnapshotFromLogId(logId);
    }
}