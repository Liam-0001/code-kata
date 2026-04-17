using CodeKata.API.DTO;
using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public class PlanningEngineService : IPlanningEngineService
{
    public async Task<IEnumerable<ResultDTO>> CreatePlanningOptimal(List<DOMAIN.Task> tasks, List<Resource> resources)
    {
        var bestResults = new List<ResultDTO>();
        double minTotalDelay = double.MaxValue;

        var permutations = GetPermutations(tasks);

        foreach (var taskOrder in permutations)
        {
            var currentResults = new List<ResultDTO>();
            var resourceClocks = resources.ToDictionary(r => r.Id, r => r.StartTime);
            double currentDelay = 0;

            foreach (var task in taskOrder)
            {
                var bestResource = resources
                    .Where(r => r.Skills.Select(s => s.ToString()).Contains(task.RequiredSkill.ToString()))
                    .Where(r => resourceClocks[r.Id].AddMinutes(task.Minutes) <= r.EndTime)
                    .OrderBy(r =>
                        resourceClocks[r.Id].AddMinutes(task.Minutes) > task.Deadline
                            ? (resourceClocks[r.Id].AddMinutes(task.Minutes).ToTimeSpan() - task.Deadline.ToTimeSpan()).TotalMinutes
                            : 0
                    )
                    .ThenBy(r => resourceClocks[r.Id])
                    .FirstOrDefault();

                if (bestResource != null)
                {
                    var startTime = resourceClocks[bestResource.Id];
                    var endTime = startTime.AddMinutes(task.Minutes);

                    currentResults.Add(
                        new ResultDTO
                        {
                            Resource = bestResource.Name,
                            Task = task.Name,
                            Time = startTime,
                        }
                    );
                    resourceClocks[bestResource.Id] = endTime;

                    if (endTime > task.Deadline)
                        currentDelay += (endTime.ToTimeSpan() - task.Deadline.ToTimeSpan()).TotalMinutes;
                }
                else
                {
                    Console.WriteLine($"Warning: Task {task.Name} could not be planned due to unavailable resources.");
                }
            }

            if (currentDelay < minTotalDelay)
            {
                minTotalDelay = currentDelay;
                bestResults = currentResults;
            }
        }

        return await System.Threading.Tasks.Task.FromResult(bestResults);
    }

    private IEnumerable<IEnumerable<DOMAIN.Task>> GetPermutations(List<DOMAIN.Task> list)
    {
        var items = list.OrderBy(t => t.Priority).ToList();
        return Generate(items.Count, items);

        IEnumerable<IEnumerable<DOMAIN.Task>> Generate(int n, List<DOMAIN.Task> curr)
        {
            if (n == 1)
                yield return new List<DOMAIN.Task>(curr);
            else
            {
                for (int i = 0; i < n; i++)
                {
                    foreach (var p in Generate(n - 1, curr))
                        yield return p;
                    var temp = curr[n % 2 == 0 ? 0 : i];
                    curr[n % 2 == 0 ? 0 : i] = curr[n - 1];
                    curr[n - 1] = temp;
                }
            }
        }
    }

    public async Task<IEnumerable<ResultDTO>> CreatePlanning(List<DOMAIN.Task> tasks, List<DOMAIN.Resource> resources)
    {
        List<ResultDTO> results = [];
        var availableResources = resources
            .Select(r => new DOMAIN.ResourceTracker { Resource = r, TimeRange = new DOMAIN.TimeRange(r.StartTime, r.EndTime) })
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
            bestResource.TimeRange = new DOMAIN.TimeRange(
                bestResource.TimeRange.Start.AddMinutes(task.Minutes),
                bestResource.TimeRange.End
            );
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
