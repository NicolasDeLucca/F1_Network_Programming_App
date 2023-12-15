using AdminServer.Models;
using Common.Managers;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("adminapi")]
    public class ReplacementController : ControllerBase
    {
        private readonly Replacement.ReplacementClient _client;
        private string _grpcURL;
        private static readonly SettingManager _settingManager = new SettingManager();

        public ReplacementController()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var serverIP = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerIP").Value;
            var serverPort = config.GetSection("AdminServerConfiguration").GetSection("GrpcServerApiHttpPort").Value;

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            _grpcURL = $"http://{serverIP}:{serverPort}";
            using var channel = GrpcChannel.ForAddress(_grpcURL);
            _client = new Replacement.ReplacementClient(channel);
        }

        [HttpPost("replacements")]
        public async Task<IActionResult> PostReplacement([FromBody] ReplacementModel replacement)
        {
            CreateReplacementRequest request = new CreateReplacementRequest()
            {
                Name = replacement.Name,
                Provider = replacement.Provider,
                Brand = replacement.Brand
            };

            CreateReplacementReply reply = await _client.CreateReplacementAsync(request);

            if (reply.Ok)
            {
                return Ok(reply.Message);
            }
            else
            {
                return BadRequest(reply.Message);
            }
        }

        [HttpPut("replacements/{id}")]
        public async Task<IActionResult> UpdateReplacement(int id, [FromBody] ReplacementModel replacement)
        {
            UpdateReplacementRequest request = new UpdateReplacementRequest()
            {
                Id = id,
                Name = replacement.Name,
                Provider = replacement.Provider,
                Brand = replacement.Brand
            };

            UpdateReplacementReply reply = await _client.UpdateReplacementAsync(request);

            if (reply.Ok)
            {
                return Ok(reply.Message);
            }
            else
            {
                return BadRequest(reply.Message);
            }
        }

        [HttpDelete("replacements/{id}")]
        public async Task<IActionResult> DeleteReplacement(int id)
        {
            DeleteReplacementRequest request = new DeleteReplacementRequest(){ Id = id };
            DeleteReplacementReply reply = await _client.DeleteReplacementAsync(request);

            if (reply.Ok)
            {
                return Ok(reply.Message);
            }
            else
            {
                return BadRequest(reply.Message);
            }
        }

        [HttpDelete("replacements/{id:int}/photo")]
        public async Task<IActionResult> DeleteReplacementPhoto(int id)
        {
            DeleteReplacementPhotoRequest request = new DeleteReplacementPhotoRequest() { Id = id };
            DeleteReplacementPhotoReply reply = await _client.DeleteReplacementPhotoAsync(request);

            if (reply.Ok)
            {
                return Ok(reply.Message);
            }
            else
            {
                return BadRequest(reply.Message);
            }
        }
    }
}