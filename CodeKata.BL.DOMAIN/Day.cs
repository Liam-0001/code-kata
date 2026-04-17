using System.Text.Json.Serialization;

namespace CodeKata.BL.DOMAIN;

public class Day
{
    [JsonPropertyName("starttime")]
    public TimeOnly StartTime { get; set; }
    [JsonPropertyName("endtime")]
    public TimeOnly EndTime { get; set; }
}