using System.Text.Json.Serialization;

namespace CodeKata.BL.DOMAIN;

public class PlanningData
{
    [JsonPropertyName("tasks")]
    public List<Task> Tasks { get; set; }
    [JsonPropertyName("resources")]
    public List<Resource> Resources { get; set; }

    [JsonPropertyName("day")]
    public Day Day { get; set; }
}