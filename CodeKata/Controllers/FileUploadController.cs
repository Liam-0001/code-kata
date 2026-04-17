using CodeKata.BL;
using Microsoft.AspNetCore.Mvc;

namespace CodeKata.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IFileHandler fileHandler) : Controller
{
    private readonly IFileHandler  _fileHandler = fileHandler;

    [HttpPost]
    public IActionResult Upload(IFormFile file)
    {
     var content =   _fileHandler.DeserializeFromStream(file.OpenReadStream());
    
     return Ok(content);
    }
}