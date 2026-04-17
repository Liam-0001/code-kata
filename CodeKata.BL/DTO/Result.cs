using CodeKata.API.DTO;
using System.Text.Json.Serialization;

namespace CodeKata.BL.DTO;

public class Result
{
    [JsonPropertyName("results")]
    public List<ResultDTO> Results { get; set; }
}
