using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Logger.Hubs;
using Logger.Protos;
using Microsoft.AspNetCore.SignalR;

namespace Logger.Services
{
    public class OnlineLogServiceImpl : OnlineLogService.OnlineLogServiceBase
    {
        private readonly ILogService _logService;
        private readonly IArgumentsService _argumentsService;
        private readonly IHubContext<LogViewerHub> _logViewerHub;
        private readonly IBackgroundTaskQueue _taskQueue;

        public OnlineLogServiceImpl(
            ILogService logService,
            IArgumentsService argumentsService,
            IHubContext<LogViewerHub> logViewerHub,
            IBackgroundTaskQueue taskQueue)
        {
            _logService = logService;
            _argumentsService = argumentsService;
            _logViewerHub = logViewerHub;
            _taskQueue = taskQueue;
        }

        public override async Task<CreateLogResponse> CreateLog(CreateLogRequest request, ServerCallContext context)
        {
            var log = await _logService.CreateLog(request.Name, request.Description);
            await _logViewerHub.Clients.All.SendAsync(
                "RefreshLogsList",
                _logService.ListLogs());
            return new CreateLogResponse
            {
                LogId = log.Id
            };
        }


        public override async Task<AddLogResponse> AddLog(AddLogRequest request, ServerCallContext context)
        {
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                var increment = await _logService.AddRecord(request.LogId, request.Log);
                await _logViewerHub.Clients.Group($"Listen{request.LogId}")
                    .SendAsync("ReceiveLog", increment);
            });
            return new AddLogResponse();
        }
    }
}