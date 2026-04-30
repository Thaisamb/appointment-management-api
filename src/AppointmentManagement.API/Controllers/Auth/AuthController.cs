using Microsoft.AspNetCore.Mvc;
using AppointmentManagement.Application.Services.Auth;
using AppointmentManagement.Application.DTOs.Auth;

namespace AppointmentManagement.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var token = await _service.Register(dto);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _service.Login(dto);
        return Ok(new { token });
    }
}