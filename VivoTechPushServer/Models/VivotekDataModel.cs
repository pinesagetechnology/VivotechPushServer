using System.Text.Json;

namespace VivoTechPushServer.Models
{
    /// <summary>
    /// Generic model to handle any JSON payload from Vivotek devices
    /// Since we don't know the exact structure, we'll store the raw JSON
    /// </summary>
    public class VivotekDataModel
    {
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public string? RawJson { get; set; }
        public JsonDocument? JsonDocument { get; set; }
        public Dictionary<string, object>? ParsedData { get; set; }
    }

    /// <summary>
    /// Generic model to handle any JSON log payload from Vivotek devices
    /// Since we don't know the exact structure, we'll store the raw JSON
    /// </summary>
    public class VivotekLogModel
    {
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public string? RawJson { get; set; }
        public JsonDocument? JsonDocument { get; set; }
        public Dictionary<string, object>? ParsedData { get; set; }
    }
}
