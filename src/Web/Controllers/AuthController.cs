using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost]
    public IActionResult Register()
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult Login()
    {
        return Ok();
    }
}