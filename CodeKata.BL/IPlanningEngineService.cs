using CodeKata.API.DTO;
using CodeKata.BL.DOMAIN;

namespace CodeKata.BL;

public interface IPlanningEngineService
{
    Task<IEnumerable<ResultDTO>> CreatePlanning(List<DOMAIN.Task> tasks, List<Resource> resources);
    Task<IEnumerable<ResultDTO>> CreatePlanningOptimal(List<DOMAIN.Task> tasks, List<Resource> resources, Day day);
    Task<IEnumerable<ResultDTO>> CreatePlanningOptimal2(List<DOMAIN.Task> tasks, List<Resource> resources);
}