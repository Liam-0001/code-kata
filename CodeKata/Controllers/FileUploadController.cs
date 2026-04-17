using CodeKata.BL;
using CodeKata.BL.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CodeKata.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IFileHandler fileHandler, IPlanningEngineService planningService) : Controller
{
    private readonly IFileHandler _fileHandler = fileHandler;
    private readonly IPlanningEngineService _planningService = planningService;

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var content = _fileHandler.DeserializeFromStream(file.OpenReadStream());

        var solution = await _planningService.CreatePlanningOptimal2(content.Tasks, content.Resources);
        var result = new Result() { Results = solution.ToList() };
        return Ok(result);
    }
}
