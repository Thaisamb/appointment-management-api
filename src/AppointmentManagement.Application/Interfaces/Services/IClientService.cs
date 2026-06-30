using AppointmentManagement.Application.DTOs.Client;

namespace AppointmentManagement.Application.Interfaces.Services;

public interface IClientService
{
    Task<IEnumerable<ClientResponseDto>> GetAllAsync();
    Task<ClientResponseDto?> GetByIdAsync(int id);
    Task<ClientResponseDto> CreateAsync(CreateClientDto dto);
    Task<ClientResponseDto?> UpdateAsync(int id, UpdateClientDto dto);
    Task<bool> DeleteAsync(int id);
}