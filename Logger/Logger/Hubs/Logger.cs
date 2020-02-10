using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Logger.Services;
using Microsoft.AspNetCore.Authorization;

namespace Logger.Hubs
{
    public class Logger : Hub
    {
        public async Task AddLog(string varName, float value, float time)
        {
            await Clients.Group("Viewer").SendAsync(
                "ReceiveLog",
                Context.ConnectionId,
                varName,
                new List<float> {value},
                new List<float> {time});
        }

        public async Task AddToViewer()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Viewer");
        }
    }
}