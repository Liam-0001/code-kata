using CodeKata.API.DTO;
using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public class PlanningEngineService : IPlanningEngineService
{
    public async Task<IEnumerable<ResultDTO>> CreatePlanning(List<DOMAIN.Task> tasks, List<Resource> resources)
    {
        List<ResultDTO> results = [];
        var availableResources = resources
            .Select(r => new ResourceTracker { Resource = r, TimeRange = new TimeRange(r.StartTime, r.EndTime) })
            .ToList();
        foreach (var task in tasks.OrderBy(t => t.Deadline).OrderBy(t => t.Priority))
        {
            var possibleResources = availableResources.Where(r =>
                r.Resource.Skills.Contains(task.RequiredSkill) && r.TimeRange.Start <= task.Deadline
            );

            if (!possibleResources.Any())
            {
                Console.WriteLine("Warning: no perfect resource found");
                possibleResources = availableResources.Where(r => r.Resource.Skills.Contains(task.RequiredSkill));
            }

            if (!possibleResources.Any())
            {
                Console.WriteLine("Warning: no resource with required skill found");
                continue;
            }

            var bestResource = possibleResources.OrderBy(r => r.TimeRange.Start).First();
            availableResources.Remove(bestResource);

            results.Add(
                new ResultDTO
                {
                    Resource = bestResource.Resource.Name,
                    Task = task.Name,
                    Time = bestResource.TimeRange.Start,
                }
            );
            bestResource.TimeRange = new TimeRange(bestResource.TimeRange.Start.AddMinutes(task.Minutes), bestResource.TimeRange.End);
        }

        results.Add(
            new ResultDTO
            {
                Resource = resources.FirstOrDefault()?.Name ?? throw new NotImplementedException(),
                Task = tasks.FirstOrDefault()?.Name ?? throw new NotImplementedException(),
                Time = resources.FirstOrDefault()?.StartTime ?? throw new NotImplementedException(),
            }
        );
        return results;
    }
}
