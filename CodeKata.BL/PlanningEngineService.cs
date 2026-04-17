using CodeKata.API.DTO;
using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public class PlanningEngineService : IPlanningEngineService
{
    public ResultDTO CreatePlanning(List<Task> task, List<Resource> resources)
    {
        return new ResultDTO
        {
            Resource = resources.FirstOrDefault()?.Name ?? throw new NotImplementedException(),
            Task = task.FirstOrDefault()?.Name ?? throw new NotImplementedException(),
            Time = resources.FirstOrDefault()?.AvailableFrom ?? throw new NotImplementedException(),
        };
}
