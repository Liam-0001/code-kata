using CodeKata.BL;
using Microsoft.AspNetCore.Mvc;

namespace CodeKata.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IFileHandler fileHandler, IPlanningEngineService planningService) : Controller
{
    private readonly IFileHandler _fileHandler = fileHandler;
    private readonly IPlanningEngineService _planningService = planningService;

    [HttpPost]
    public IActionResult Upload(IFormFile file)
    {
        var content = _fileHandler.DeserializeFromStream(file.OpenReadStream());

        var solution = _planningService.CreatePlanningOptimal(content.Tasks, content.Resources, content.Day);

        return Ok(content);
    }
}
