using Microsoft.AspNetCore.Mvc;

namespace CodeKata.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : Controller
{
    public IActionResult Upload(IFormFile file)
    {
        return Ok(file.Name);
    }
}