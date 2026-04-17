using System.Text.Json;
using System.Text.Json.Serialization;

// ============================================================
//  INPUT MODEL
// ============================================================

public class ScheduleInput
{
    [JsonPropertyName("day")]
    public DayInfo Day { get; set; }

    [JsonPropertyName("tasks")]
    public List<TaskItem> Tasks { get; set; }

    [JsonPropertyName("resources")]
    public List<Resource> Resources { get; set; }
}

public class DayInfo
{
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public string EndTime { get; set; }
}

public class TaskItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("durationMinutes")]
    public int DurationMinutes { get; set; }

    [JsonPropertyName("priority")]
    public string Priority { get; set; }

    [JsonPropertyName("deadline")]
    public string Deadline { get; set; }

    [JsonPropertyName("requiredSkill")]
    public string RequiredSkill { get; set; }
}

public class Resource
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("availableFrom")]
    public string AvailableFrom { get; set; }

    [JsonPropertyName("availableUntil")]
    public string AvailableUntil { get; set; }

    [JsonPropertyName("skills")]
    public List<string> Skills { get; set; }

    [JsonPropertyName("maxWorkMinutes")]
    public int MaxWorkMinutes { get; set; }
}

// ============================================================
//  OUTPUT MODEL
// ============================================================

public class ScheduleOutput
{
    [JsonPropertyName("results")]
    public List<ScheduleResult> Results { get; set; }
}

public class ScheduleResult
{
    [JsonPropertyName("task")]
    public string Task { get; set; }

    [JsonPropertyName("resource")]
    public string Resource { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }
}

// ============================================================
//  USAGE EXAMPLE
// ============================================================

public static class JsonSerializationExample
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    // Deserialize the input JSON
    public static ScheduleInput DeserializeInput(string json)
        => JsonSerializer.Deserialize<ScheduleInput>(json, Options);

    // Serialize back to JSON
    public static string SerializeOutput(ScheduleOutput output)
        => JsonSerializer.Serialize(output, Options);
}