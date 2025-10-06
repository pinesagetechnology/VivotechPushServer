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
    [Route("api/[controller]")]
    public class VivotekController : ControllerBase
    {
        private readonly ILogger<VivotekController> _logger;
        private readonly DataStorageService _dataStorageService;

        public VivotekController(ILogger<VivotekController> logger, DataStorageService dataStorageService)
        {
            _logger = logger;
            _dataStorageService = dataStorageService;
        }

        /// <summary>
        /// Endpoint to receive actual data from Vivotek device
        /// Configure this as Server URI: /api/vivotek/data in Vivotek app
        /// </summary>
        [HttpPost("receiveData")]
        public async Task<IActionResult> ReceiveData()
        {
            try
            {
                // Read the raw JSON from the request body
                using var reader = new StreamReader(Request.Body);
                var rawJson = await reader.ReadToEndAsync();

                _logger.LogInformation("Received data from Vivotek device at {Timestamp}", DateTime.UtcNow);
                _logger.LogInformation("Raw JSON payload: {RawJson}", rawJson);

                // Create our model with the raw JSON
                var data = new VivotekDataModel
                {
                    RawJson = rawJson,
                    ReceivedAt = DateTime.UtcNow
                };

                // Try to parse the JSON for better logging
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

                return Ok(new { message = "Data received successfully", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing data from Vivotek device");
                return BadRequest(new { error = "Failed to process data", message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint to receive logs from Vivotek device
        /// Configure this as Server URI: /api/vivotek/logs in Vivotek app
        /// </summary>
        [HttpPost("logs")]
        public async Task<IActionResult> ReceiveLogs()
        {
            try
            {
                // Read the raw JSON from the request body
                using var reader = new StreamReader(Request.Body);
                var rawJson = await reader.ReadToEndAsync();

                _logger.LogInformation("Received log from Vivotek device at {Timestamp}", DateTime.UtcNow);
                _logger.LogInformation("Raw JSON payload: {RawJson}", rawJson);

                // Create our model with the raw JSON
                var log = new VivotekLogModel
                {
                    RawJson = rawJson,
                    ReceivedAt = DateTime.UtcNow
                };

                // Try to parse the JSON for better logging
                try
                {
                    log.JsonDocument = JsonDocument.Parse(rawJson);
                    log.ParsedData = JsonSerializer.Deserialize<Dictionary<string, object>>(rawJson);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to parse JSON payload, but will still save raw data");
                }

                // Process the log here
                await ProcessLogAsync(log);

                return Ok(new { message = "Log received successfully", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing log from Vivotek device");
                return BadRequest(new { error = "Failed to process log", message = ex.Message });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        [HttpPost("data")]
        public async Task<IActionResult> Data()
        {
            // Read raw body
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var raw = await reader.ReadToEndAsync();

            // Content-Type tells you what the camera sent
            var ct = Request.ContentType?.ToLowerInvariant();

            if (!string.IsNullOrEmpty(raw) && ct?.Contains("xml") == true || raw.TrimStart().StartsWith("<"))
            {
                // ONVIF (SOAP/XML). Parse minimally:
                var doc = XDocument.Parse(raw);

                // (actual ONVIF payloads vary by topic/rule)
                var topic = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Topic")?.Value;
                //var msg = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Message")?.Value;

                // Ensure msg is not null before calling SaveRawDataAsync
                if (!string.IsNullOrEmpty(raw))
                {
                    await _dataStorageService.SaveRawDataAsync(raw);
                }
                else
                {
                    _logger.LogWarning("Message content is null or empty. Skipping SaveRawDataAsync.");
                }
            }
            else
            {
                // JSON (Static body)
                Console.WriteLine($"JSON Event: {raw}");
                // dynamic evt = JsonSerializer.Deserialize<dynamic>(raw);
                if (!string.IsNullOrEmpty(raw))
                {
                    await _dataStorageService.SaveRawDataAsync(raw);
                }
                else
                {
                    _logger.LogWarning("Message content is null or empty. Skipping SaveRawDataAsync.");
                }
            }

            return Ok();
        }

        private async Task ProcessDataAsync(VivotekDataModel data)
        {
            _logger.LogInformation("Processing data received at {ReceivedAt}", data.ReceivedAt);

            // Save data to configured folder
            await _dataStorageService.SaveDataAsync(data);
            
            _logger.LogInformation("Data processing completed at {Timestamp}", DateTime.UtcNow);
        }

        private async Task ProcessLogAsync(VivotekLogModel log)
        {
            _logger.LogInformation("Processing log received at {ReceivedAt}", log.ReceivedAt);

            // Save log to configured folder
            await _dataStorageService.SaveLogAsync(log);
            
            _logger.LogInformation("Log processing completed at {Timestamp}", DateTime.UtcNow);
        }
    }
}
