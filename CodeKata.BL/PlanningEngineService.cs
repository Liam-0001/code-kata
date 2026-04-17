using CodeKata.API.DTO;
using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public class PlanningEngineService : IPlanningEngineService
{
    public async Task<IEnumerable<ResultDTO>> CreatePlanningOptimal(List<DOMAIN.Task> tasks, List<Resource> resources, Day day)
    {
        var bestResults = new List<ResultDTO>();
        double bestScore = double.MinValue;

        void Backtrack(int taskIndex, List<ResultDTO> currentResults, List<ResourceTracker> currentTrackers, double currentScore)
        {
            if (taskIndex == tasks.Count)
            {
                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestResults = new List<ResultDTO>(currentResults);
                }
                return;
            }

            var task = tasks[taskIndex];
            bool planned = false;

            foreach (var tracker in currentTrackers.Where(r => r.Resource.Skills.Contains(task.RequiredSkill)))
            {
                var actualStart = tracker.TimeRange.Start < day.StartTime ? day.StartTime : tracker.TimeRange.Start;
                var taskEnd = actualStart.AddMinutes(task.Minutes);
                var dayEndLimit = tracker.TimeRange.End < day.EndTime ? tracker.TimeRange.End : day.EndTime;

                if (taskEnd <= dayEndLimit && actualStart >= tracker.TimeRange.Start)
                {
                    planned = true;
                    var previousRange = tracker.TimeRange;

                    double penalty = taskEnd > task.Deadline ? (taskEnd - task.Deadline).TotalMinutes * 20 : 0;
                    double priorityWeight = task.Priority == Priority.High ? 5000 : (task.Priority == Priority.Normal ? 2000 : 500);
                    double stepScore = priorityWeight - (taskEnd.ToTimeSpan().TotalMinutes) - penalty;

                    currentResults.Add(
                        new ResultDTO
                        {
                            Resource = tracker.Resource.Name,
                            Task = task.Name,
                            Time = actualStart,
                        }
                    );
                    tracker.TimeRange = new TimeRange(taskEnd, tracker.TimeRange.End);

                    Backtrack(taskIndex + 1, currentResults, currentTrackers, currentScore + stepScore);

                    tracker.TimeRange = previousRange;
                    currentResults.RemoveAt(currentResults.Count - 1);
                }
            }

            if (!planned)
            {
                Backtrack(taskIndex + 1, currentResults, currentTrackers, currentScore - 100000);
            }
        }

        var trackers = resources
            .Select(r => new ResourceTracker { Resource = r, TimeRange = new TimeRange(r.StartTime, r.EndTime) })
            .ToList();

        Backtrack(0, new List<ResultDTO>(), trackers, 0);

        var plannedTaskNames = bestResults.Select(r => r.Task).ToHashSet();
        foreach (var task in tasks.Where(t => !plannedTaskNames.Contains(t.Name)))
        {
            Console.WriteLine($"Warning: task {task.Name} could not be planned");
        }

        return bestResults;
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
