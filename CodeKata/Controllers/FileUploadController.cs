using Microsoft.AspNetCore.Mvc;

namespace CodeKata.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : Controller
{
    
    [HttpPost]
    public IActionResult Upload(IFormFile file)
    {
        return Ok(file);
    }
}