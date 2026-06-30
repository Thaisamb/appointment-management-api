using AppointmentManagement.Application.DTOs.Session;
using AppointmentManagement.Domain.Entities;

namespace AppointmentManagement.Application.Interfaces.Repositories;

public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(int id);
    Task<Session?> GetByIdWithClientAsync(int id);
    Task<IReadOnlyList<Session>> GetByUserAsync(int userId, SessionFilterDto filter);
    Task<IReadOnlyList<Session>> GetByGroupIdAsync(Guid groupId);
    Task AddRangeAsync(IEnumerable<Session> sessions);
    void Remove(Session session);
    void RemoveRange(IEnumerable<Session> sessions);
}