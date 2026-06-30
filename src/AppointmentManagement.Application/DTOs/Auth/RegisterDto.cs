using AppointmentManagement.Application.DTOs.ValueObjects;

namespace AppointmentManagement.Application.DTOs.Auth;

public class RegisterDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public required string CPF { get; set; }
    public string? CNPJ { get; set; }
    public string? CompanyName { get; set; }

    public AddressDto? Address { get; set; }

}