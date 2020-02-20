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
        private readonly ArgumentService _siteSetting;
        private readonly IHubContext<LoggerHub> _loggerHub;
        public ArgumentController(ArgumentService siteSetting, IHubContext<LoggerHub> loggerHub)
        {
            _siteSetting = siteSetting;
            _loggerHub = loggerHub;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSettings() =>
            Ok(_siteSetting.Get().Arguments);

        [HttpPut]
        public ActionResult UpdateSettings(ArgumentModel setting)
        {
            _siteSetting.Update(setting);
            _loggerHub.Clients.All.SendAsync("ReloadSettings", setting);
            return NoContent();
        }

        [HttpPost]
        [Route("reset")]
        public ActionResult ResetSettings()
        {
            _siteSetting.Init();
            return NoContent();
        }
    }
}