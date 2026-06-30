using AppointmentManagement.Domain.Entities;

namespace AppointmentManagement.Application.Interfaces.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task<Client?> GetByIdWithDetailsAsync(int id); // com EmergencyContacts
    Task<IReadOnlyList<Client>> GetAllByUserAsync(int userId);
    Task AddAsync(Client client);
    void Remove(Client client);
    Task<bool> ExistsByCpfAsync(string cpf, int userId);
}