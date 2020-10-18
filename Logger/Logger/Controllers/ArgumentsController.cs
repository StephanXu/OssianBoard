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
        private readonly IArgumentsService _argumentsService;
        private readonly IHubContext<LoggerHub> _loggerHub;

        public ArgumentController(IArgumentsService argumentsService, IHubContext<LoggerHub> loggerHub)
        {
            _argumentsService = argumentsService;
            _loggerHub = loggerHub;
        }

        [HttpGet]
        public ActionResult ListArguments() => Ok(_argumentsService.ListArguments());

        [HttpGet("{argId}")]
        [AllowAnonymous]
        public ActionResult GetSingleArgument([FromRoute] string argId, [FromQuery(Name = "pt")] bool pureText)
        {
            var arg = _argumentsService.GetSingleArgument(argId);
            return pureText ? Ok(arg.Content) : Ok(arg);
        }
            

        [HttpGet("{argId}/snapshot")]
        public ActionResult ListSnapshot([FromRoute] string argId) =>
            Ok(_argumentsService.ListSnapshot(argId));

        [HttpGet("snapshot/{snapshotId}")]
        [AllowAnonymous]
        public ActionResult GetSingleSnapshot([FromRoute] string snapshotId) =>
            Ok(_argumentsService.GetSingleSnapshot(snapshotId));

        [HttpPut("{argId}")]
        public ActionResult UpdateArgumentContent([FromRoute] string argId, [FromBody] ArgumentUpdateRequest argument)
        {
            var arg = _argumentsService.GetSingleArgument(argId);
            if (arg == null)
            {
                return NotFound();
            }

            arg.Content = argument.Content;
            _argumentsService.UpdateArguments(argId, arg);
            _loggerHub.Clients.All.SendAsync("ReloadSettings", argument.Content);
            return NoContent();
        }

        [HttpPost]
        public ActionResult CreateArgument(CreateArgumentRequest createRequest) =>
            Ok(_argumentsService.CreateArgument(createRequest.Name, createRequest.Schema, createRequest.Content));

        [HttpPost("{argId}/snapshot")]
        [AllowAnonymous]
        public ActionResult CreateSnapshot(
            [FromRoute] string argId,
            [FromBody] CreateSnapshotRequest createRequest,
            [FromQuery] string logId)
        {
            var snapshot = _argumentsService.CreateSnapshot(argId, createRequest.Name);
            if (logId != null)
            {
                _argumentsService.BindSnapshotForLog(logId, snapshot.Id);
            }
            return Ok(snapshot);
        }

        [HttpDelete("snapshot/{snapshotId}")]
        public ActionResult DeleteSnapshot([FromRoute] string snapshotId)
        {
            _argumentsService.RemoveSnapshot(snapshotId);
            return Ok();
        }

        [HttpDelete("{argId}")]
        public ActionResult DeleteArgument([FromRoute] string argId)
        {
            _argumentsService.RemoveArguments(argId);
            return Ok();
        }

        [HttpPost("snapshot/{snapshotId}/tag")]
        public ActionResult TagSnapshot([FromRoute] string snapshotId, TagSnapshotRequest tag)
        {
            _argumentsService.TagSnapshot(snapshotId, tag.Tag);
            return Ok();
        }

        [HttpDelete("snapshot/{snapshotId}/tag")]
        public ActionResult UntagSnapshot([FromRoute] string snapshotId)
        {
            _argumentsService.TagSnapshot(snapshotId, null);
            return Ok();
        }
    }
}