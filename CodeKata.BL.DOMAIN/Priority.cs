using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CodeKata.BL.DOMAIN;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Priority
{
    [JsonStringEnumMemberName("Hoog")]
    High,
    
    [JsonStringEnumMemberName("Normaal")]
    Normal,
    
    [JsonStringEnumMemberName("Laag")]
    Low
}