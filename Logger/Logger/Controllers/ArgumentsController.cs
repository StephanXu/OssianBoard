using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Logger.Services;
using Logger.Models;
using Microsoft.AspNetCore.Authorization;

namespace Logger.Controllers
{
    [Authorize(Roles = "Admin,Member")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArgumentController : ControllerBase
    {
        private readonly ArgumentService _siteSetting;

        public ArgumentController(ArgumentService siteSetting)
        {
            _siteSetting = siteSetting;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSettings() =>
            Ok(_siteSetting.Get().Arguments);

        [HttpPut]
        public ActionResult UpdateSettings(ArgumentModel setting)
        {
            _siteSetting.Update(setting);
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