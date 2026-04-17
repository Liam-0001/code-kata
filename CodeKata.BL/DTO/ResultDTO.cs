using System.Text.Json.Serialization;

namespace CodeKata.API.DTO;

public class ResultDTO
{
    [JsonPropertyName("task")]
    public string Task { get; set; }

    [JsonPropertyName("resource")]
    public string Resource { get; set; }

    [JsonPropertyName("time")]
    public TimeOnly Time { get; set; }
}
