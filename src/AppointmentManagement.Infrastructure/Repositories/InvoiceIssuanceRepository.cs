using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagement.Infrastructure.Repositories;

public class InvoiceIssuanceRepository(AppDbContext db) : IInvoiceIssuanceRepository
{
    public async Task<InvoiceIssuance?> GetByUserAsync(int userId) =>
        await db.InvoiceIssuances.FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task AddAsync(InvoiceIssuance config) =>
        await db.InvoiceIssuances.AddAsync(config);
}