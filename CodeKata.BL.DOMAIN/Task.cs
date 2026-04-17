using System.Text.Json.Serialization;

namespace CodeKata.BL.DOMAIN;

public class Task
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("durationMinutes")]
    public int Minutes { get; set; }
    [JsonPropertyName("deadline")]
    public TimeOnly Deadline { get; set; }
    [JsonPropertyName("priority")]
    public Priority Priority { get; set; }
    [JsonPropertyName("requiredSkill")]
    public string  RequiredSkill { get; set; }
}