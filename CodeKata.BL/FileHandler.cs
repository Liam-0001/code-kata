using System.Text;
using System.Text.Json;
using CodeKata.API.DTO;
using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public class FileHandler: IFileHandler
{
    public PlanningData? DeserializeFromStream(Stream stream)
    {
        var planningData = JsonSerializer.Deserialize<PlanningData>(stream);
        
        return planningData;
    }
}