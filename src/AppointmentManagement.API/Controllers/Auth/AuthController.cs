using Microsoft.AspNetCore.Mvc;
using AppointmentManagement.Application.DTOs.Auth;
using AppointmentManagement.Application.Interfaces.Services;
using AppointmentManagement.Infrastructure.Services;

namespace AppointmentManagement.API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var token = await service.Register(dto);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await service.Login(dto);
        return Ok(new { token });
    }
}