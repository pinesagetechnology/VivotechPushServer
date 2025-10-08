using Microsoft.AspNetCore.Mvc;
using VivoTechPushServer.Models;
using VivoTechPushServer.Services;
using System.Text.Json;
using System.Xml.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VivoTechPushServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VivotekController : ControllerBase
    {
        private readonly ILogger<VivotekController> _logger;
        private readonly DataStorageService _dataStorageService;

        public VivotekController(ILogger<VivotekController> logger, DataStorageService dataStorageService)
        {
            _logger = logger;
            _dataStorageService = dataStorageService;
        }

        [HttpPost("push")]
        public async Task<IActionResult> Push()
        {
            try
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var payload = await reader.ReadToEndAsync();

                await _dataStorageService.SaveRawDataAsync(payload);

                var timestamp = DateTime.Now.ToString("o"); // ISO 8601 format

                _logger.LogInformation("Push received at {Timestamp}", timestamp);

                return Ok("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing push request");
                return Ok("OK"); 
            }
        }
        
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
