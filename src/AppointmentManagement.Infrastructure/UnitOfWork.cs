using AppointmentManagement.Application.Interfaces;
using AppointmentManagement.Infrastructure.Data;

namespace AppointmentManagement.Infrastructure;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task CommitAsync() =>
        await db.SaveChangesAsync();
}