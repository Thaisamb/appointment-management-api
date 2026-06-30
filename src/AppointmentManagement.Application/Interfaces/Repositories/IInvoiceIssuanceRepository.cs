using AppointmentManagement.Domain.Entities;

namespace AppointmentManagement.Application.Interfaces.Repositories;

public interface IInvoiceIssuanceRepository
{
    Task<InvoiceIssuance?> GetByUserAsync(int userId);
    Task AddAsync(InvoiceIssuance config);
}