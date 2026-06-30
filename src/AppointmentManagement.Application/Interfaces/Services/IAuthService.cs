using AppointmentManagement.Application.DTOs.Auth;

namespace AppointmentManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task<string> Register(RegisterDto dto);
    Task<string> Login(LoginDto dto);
}