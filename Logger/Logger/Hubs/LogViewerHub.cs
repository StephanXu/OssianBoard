using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logger.Models;
using Logger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Logger.Hubs
{
    [Authorize]
    public class LogViewerHub : Hub
    {
        private readonly ILogService _logService;

        public LogViewerHub(ILogService logService)
        {
            _logService = logService;
        }

        public IEnumerable<LogModel> ListLogs() => _logService.ListLogs();

        public IEnumerable<RecordModel> GetLog(string logId) => _logService.GetLog(logId);

        public async Task RemoveLog(string logId)
        {
            await _logService.RemoveLog(logId);
            await Clients.All.SendAsync(
                "RefreshLogsList",
                _logService.ListLogs());
        }

        public async Task ListenLog(string logId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Listen" + logId);
        }

        public async Task UnListenLog(string logId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Listen" + logId);
        }
    }
}