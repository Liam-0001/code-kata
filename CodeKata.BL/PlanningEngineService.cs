using CodeKata.API.DTO;
using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public class PlanningEngineService : IPlanningEngineService
{
    public async Task<IEnumerable<ResultDTO>> CreatePlanningOptimal2(List<DOMAIN.Task> tasks, List<Resource> resources)
    {
        try
        {
            List<ResultDTO> results = [];
            var resourceTrackers = resources
                .Select(r => new ResourceTracker { Resource = r, TimeRange = new TimeRange(r.StartTime, r.EndTime) })
                .ToList();

            // We sort tasks by priority first (High > Medium > Low), then by deadline (earlier first) as a tiebreaker
            var sortedTasks = tasks
                .OrderBy(t => t.Priority) // Enum order: High=0, Medium=1, Low=2
                .ThenBy(t => t.Deadline)
                .ToList();

            foreach (var task in sortedTasks)
            {
                // Find all resources with required skill and available time slot that fits the task duration
                var suitableResources = resourceTrackers
                    .Where(rt => rt.Resource.Skills.Contains(task.RequiredSkill.ToString()))
                    .Select(rt =>
                    {
                        // Find the earliest possible start time within the resource's availability
                        var start = rt.TimeRange.Start;
                        var end = start.AddMinutes(task.Minutes);

                        // Check if it fits within working hours and before deadline (if applicable)
                        if (end > rt.TimeRange.End || end > task.Deadline)
                            return null; // Not feasible for deadline or working hours

                        return new
                        {
                            ResourceTracker = rt,
                            Start = start,
                            End = end,
                        };
                    })
                    .Where(x => x != null)
                    .ToList();

                // If no resource can meet the deadline, relax the deadline constraint (soft constraint)
                if (!suitableResources.Any())
                {
                    Console.WriteLine($"Warning: No resource found for task '{task.Name}' within deadline. Relaxing deadline.");

                    suitableResources = resourceTrackers
                        .Where(rt => rt.Resource.Skills.Contains(task.RequiredSkill.ToString()))
                        .Select(rt =>
                        {
                            var start = rt.TimeRange.Start;
                            var end = start.AddMinutes(task.Minutes);

                            if (end > rt.TimeRange.End)
                                return null; // Still needs to fit in working hours

                            return new
                            {
                                ResourceTracker = rt,
                                Start = start,
                                End = end,
                            };
                        })
                        .Where(x => x != null)
                        .ToList();
                }

                // If still no suitable resource, skip the task
                if (!suitableResources.Any())
                {
                    Console.WriteLine($"Warning: No resource with required skill for task '{task.Name}'. Skipping.");
                    continue;
                }

                // Pick the best candidate: prefer earlier start time (maximizes future flexibility)
                var best = suitableResources.OrderBy(x => x.Start).First();

                // Assign task
                results.Add(
                    new ResultDTO
                    {
                        Resource = best.ResourceTracker.Resource.Name,
                        Task = task.Name,
                        Time = best.Start,
                    }
                );

                // Update the resource’s available time range (move start forward)
                best.ResourceTracker.TimeRange = new TimeRange(best.End, best.ResourceTracker.TimeRange.End);
            }

            return results;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

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
