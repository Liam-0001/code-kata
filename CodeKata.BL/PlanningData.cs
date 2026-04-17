using System.Text.Json.Serialization;
using CodeKata.BL.DOMAIN;
using Task = CodeKata.BL.DOMAIN.Task;

namespace CodeKata.BL;

public class PlanningData
{
    [JsonPropertyName("tasks")]
    public List<Task> Tasks { get; set; }
    [JsonPropertyName("resources")]
    public List<Resource> Resources { get; set; }
}