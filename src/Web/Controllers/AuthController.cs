using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    public IActionResult Register()
    {
        return Ok();
    }

    public IActionResult Login()
    {
        return Ok();
    }
}