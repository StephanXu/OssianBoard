using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Logger.Models;
using Logger.Protos;
using Microsoft.AspNetCore.SignalR;
using Logger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Logger.Hubs
{
    public class LoggerHub : Hub
    {
        private readonly ILogService _logService;
        private readonly IHubContext<LogViewerHub> _logViewerHub;
        private readonly IArgumentsService _argumentsService;

        public LoggerHub(
            ILogService logService,
            IHubContext<LogViewerHub> logViewerHub,
            IArgumentsService argumentsService)
        {
            _logService = logService;
            _logViewerHub = logViewerHub;
            _argumentsService = argumentsService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<string> CreateLog(string name, string description)
        {
            var log = await _logService.CreateLog(name, description);
            await _logViewerHub.Clients.All.SendAsync(
                "RefreshLogsList",
                _logService.ListLogs());
            return log == null ? "error" : log.Id;
        }

        public async Task AddLog(string logId, IEnumerable<string> logs)
        {
            var records = await _logService.AddRecord(logId, logs);
            await _logViewerHub.Clients.Group("Listen" + logId).SendAsync("ReceiveLog", records);
        }

        public void ArchiveConfiguration(string logId, string configuration)
        {
            var config = Configuration.Parser.ParseJson(configuration);
            _argumentsService.ArchiveArguments(logId, config);
        }
    }
}