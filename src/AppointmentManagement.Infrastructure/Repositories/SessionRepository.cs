using AppointmentManagement.Application.DTOs.Session;
using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagement.Infrastructure.Repositories;

public class SessionRepository(AppDbContext db) : ISessionRepository
{
    public async Task<Session?> GetByIdAsync(int id) =>
        await db.Sessions.FindAsync(id);

    public async Task<Session?> GetByIdWithClientAsync(int id) =>
        await db.Sessions
            .Include(s => s.Client)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IReadOnlyList<Session>> GetByUserAsync(int userId, SessionFilterDto filter)
    {
        var query = db.Sessions
            .Include(s => s.Client)
            .Where(s => s.UserId == userId);

        if (filter.ClientId.HasValue)
            query = query.Where(s => s.ClientId == filter.ClientId.Value);

        if (filter.Status.HasValue)
            query = query.Where(s => s.Status == filter.Status.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(s => DateOnly.FromDateTime(s.DateTime) >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(s => DateOnly.FromDateTime(s.DateTime) <= filter.DateTo.Value);

        return await query.OrderBy(s => s.DateTime).ToListAsync();
    }

    public async Task<IReadOnlyList<Session>> GetByGroupIdAsync(Guid groupId) =>
        await db.Sessions
            .Where(s => s.RepetitionGroupId == groupId)
            .ToListAsync();

    public async Task AddRangeAsync(IEnumerable<Session> sessions) =>
        await db.Sessions.AddRangeAsync(sessions);

    public void Remove(Session session) =>
        db.Sessions.Remove(session);

    public void RemoveRange(IEnumerable<Session> sessions) =>
        db.Sessions.RemoveRange(sessions);
}