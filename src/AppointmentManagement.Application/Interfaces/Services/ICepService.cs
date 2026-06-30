using AppointmentManagement.Application.DTOs.ValueObjects;
namespace AppointmentManagement.Application.Interfaces.Services;

public interface ICepService
{
    Task<CepResponseDto?> BuscarCepAsync(string cep);
}
