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

        /// <summary>
        /// Endpoint to receive actual data from Vivotek device
        /// Configure this as Server URI: /api/vivotek/data in Vivotek app
        /// </summary>
        [HttpPost("push/json")]
        public async Task<IActionResult> ReceiveData()
        {
            using var reader = new StreamReader(Request.Body);
            var rawJson = await reader.ReadToEndAsync();

            _logger.LogInformation("Received data from Vivotek device at {Timestamp}", DateTime.UtcNow);
            _logger.LogInformation("Raw JSON payload: {RawJson}", rawJson);

            var data = new VivotekDataModel
            {
                RawJson = rawJson,
                ReceivedAt = DateTime.UtcNow
            };

            try
            {
                data.JsonDocument = JsonDocument.Parse(rawJson);
                data.ParsedData = JsonSerializer.Deserialize<Dictionary<string, object>>(rawJson);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse JSON payload, but will still save raw data");
            }

            // Process the data here
            await ProcessDataAsync(data);

            return Ok("OK");
        }
        
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        [HttpPost("push/xml")]
        public async Task<IActionResult> Data()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var raw = await reader.ReadToEndAsync();

            var ct = Request.ContentType?.ToLowerInvariant();

            var doc = XDocument.Parse(raw);

            var topic = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Topic")?.Value;

            if (!string.IsNullOrEmpty(raw))
            {
                await _dataStorageService.SaveRawDataAsync(raw);
            }
            else
            {
                _logger.LogWarning("Message content is null or empty. Skipping SaveRawDataAsync.");
            }

            return Ok("OK");
        }

        private async Task ProcessDataAsync(VivotekDataModel data)
        {
            _logger.LogInformation("Processing data received at {ReceivedAt}", data.ReceivedAt);

            // Save data to configured folder
            await _dataStorageService.SaveDataAsync(data);
            
            _logger.LogInformation("Data processing completed at {Timestamp}", DateTime.UtcNow);
        }

    }
}
