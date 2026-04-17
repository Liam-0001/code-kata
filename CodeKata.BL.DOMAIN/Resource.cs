using System.Text.Json.Serialization;

namespace CodeKata.BL.DOMAIN;


public class Resource
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("availableFrom")]
    public TimeOnly StartTime  { get; set; }
    [JsonPropertyName("availableUntil")]
    public TimeOnly EndTime  { get; set; }
    [JsonPropertyName("skills")]
    public IEnumerable<string> Skills { get; set; } =  new List<string>();
    [JsonPropertyName("maxWorkMinutes")]
    public int MaxWorkMinutes { get; set; }
}