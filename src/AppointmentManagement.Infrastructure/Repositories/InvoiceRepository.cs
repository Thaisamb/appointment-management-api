using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagement.Infrastructure.Repositories;

public class InvoiceRepository(AppDbContext db) : IInvoiceRepository
{
	public async Task<int> CountEmittedByUserAsync(int userId, int month, int year) =>
		await db.Invoices
			.CountAsync(i => i.UserId == userId
						  && i.Month == month
						  && i.Year == year
						  && i.Status == InvoiceStatus.Issued);

	public async Task AddAsync(Invoice invoice) =>
		await db.Invoices.AddAsync(invoice);
}