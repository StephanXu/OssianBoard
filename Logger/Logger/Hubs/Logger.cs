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
        public async Task AddLog(string clientId, string varName, float value)
        {
            System.Console.WriteLine(varName + " = " + value.ToString());
            await Clients.Group("Viewer").SendAsync("ReceiveLog", clientId, varName, value);
        }

        public async Task AddToViewer()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Viewer");
        }
    }
}
