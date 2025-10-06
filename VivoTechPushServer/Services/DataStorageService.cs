using System.Text.Json;
using VivoTechPushServer.Models;

namespace VivoTechPushServer.Services
{
    public class DataStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataStorageService> _logger;

        public DataStorageService(IConfiguration configuration, ILogger<DataStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SaveDataAsync(VivotekDataModel data)
        {
            try
            {
                var dataFolderPath = _configuration["DataStorage:DataFolderPath"];
                if (string.IsNullOrEmpty(dataFolderPath))
                {
                    throw new InvalidOperationException("DataStorage:DataFolderPath is not configured in appsettings.json");
                }

                // Ensure directory exists
                Directory.CreateDirectory(dataFolderPath);

                // Create filename with timestamp
                var fileName = $"data_{data.ReceivedAt:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.json";
                var filePath = Path.Combine(dataFolderPath, fileName);

                // Create a wrapper object that includes both the raw JSON and metadata
                var wrapper = new
                {
                    ReceivedAt = data.ReceivedAt,
                    RawJson = data.RawJson,
                    ParsedData = data.ParsedData
                };

                // Serialize wrapper to JSON
                var jsonData = JsonSerializer.Serialize(wrapper, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                // Write to file
                await File.WriteAllTextAsync(filePath, jsonData);

                _logger.LogInformation("Data saved successfully to: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving data to file");
                throw;
            }
        }

        public async Task SaveRawDataAsync(string data)
        {
            try
            {
                var dataFolderPath = _configuration["DataStorage:DataFolderPath"];
                if (string.IsNullOrEmpty(dataFolderPath))
                {
                    throw new InvalidOperationException("DataStorage:DataFolderPath is not configured in appsettings.json");
                }

                // Ensure directory exists
                Directory.CreateDirectory(dataFolderPath);

                // Create filename with timestamp
                var fileName = $"data_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.json";
                var filePath = Path.Combine(dataFolderPath, fileName);

                // Write to file
                await File.WriteAllTextAsync(filePath, data);

                _logger.LogInformation("Data saved successfully to: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving data to file");
                throw;
            }
        }

        public async Task SaveLogAsync(VivotekLogModel log)
        {
            try
            {
                var logsFolderPath = _configuration["DataStorage:LogsFolderPath"];
                if (string.IsNullOrEmpty(logsFolderPath))
                {
                    throw new InvalidOperationException("DataStorage:LogsFolderPath is not configured in appsettings.json");
                }

                // Ensure directory exists
                Directory.CreateDirectory(logsFolderPath);

                // Create filename with timestamp
                var fileName = $"log_{log.ReceivedAt:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.json";
                var filePath = Path.Combine(logsFolderPath, fileName);

                // Create a wrapper object that includes both the raw JSON and metadata
                var wrapper = new
                {
                    ReceivedAt = log.ReceivedAt,
                    RawJson = log.RawJson,
                    ParsedData = log.ParsedData
                };

                // Serialize wrapper to JSON
                var jsonLog = JsonSerializer.Serialize(wrapper, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                // Write to file
                await File.WriteAllTextAsync(filePath, jsonLog);

                _logger.LogInformation("Log saved successfully to: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving log to file");
                throw;
            }
        }
    }
}
