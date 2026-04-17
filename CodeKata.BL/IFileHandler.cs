using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public interface IFileHandler
{
    public PlanningData? DeserializeFromStream(Stream stream);
}