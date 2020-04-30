using Logger.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Logger.Services;
using Logger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Logger.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArgumentController : ControllerBase
    {
        private readonly IArgumentsService _siteSetting;
        private readonly IHubContext<LoggerHub> _loggerHub;
        public ArgumentController(IArgumentsService siteSetting, IHubContext<LoggerHub> loggerHub)
        {
            _siteSetting = siteSetting;
            _loggerHub = loggerHub;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSettings() =>
            Ok(_siteSetting.GetArguments().Arguments);

        [HttpPut]
        public ActionResult UpdateSettings(ArgumentModel setting)
        {
            _siteSetting.UpdateArguments(setting);
            _loggerHub.Clients.All.SendAsync("ReloadSettings", setting);
            return NoContent();
        }
    }
}