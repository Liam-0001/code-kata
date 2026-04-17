using System.Text;
using System.Text.Json;
using CodeKata.API.DTO;

namespace CodeKata.BL;

public class FileHandler: IFileHandler
{
    public string DeserializeFromStream(Stream stream)
    {
        try
        {
            var planningData = JsonSerializer.Deserialize<PlanningData>(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       

        return "test";
    }
}