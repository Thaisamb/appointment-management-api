using AppointmentManagement.Domain.Entities;

namespace AppointmentManagement.Application.Interfaces.Repositories;

public interface IInvoiceRepository
{
	Task AddAsync(Invoice invoice);
	Task<int> CountEmittedByUserAsync(int userId, int month, int year);
}