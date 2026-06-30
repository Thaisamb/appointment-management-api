using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagement.Infrastructure.Repositories;

public class ClientRepository(AppDbContext db) : IClientRepository
{
	public async Task<Client?> GetByIdAsync(int id) =>
		await db.Clients.FindAsync(id);

	public async Task<Client?> GetByIdWithDetailsAsync(int id) =>
		await db.Clients
			.Include(c => c.EmergencyContacts)
			.Include(c => c.FinancialResponsible)
			.Include(c => c.InvoiceIssuer)
            .FirstOrDefaultAsync(c => c.Id == id);

	public async Task<IReadOnlyList<Client>> GetAllByUserAsync(int userId) =>
		await db.Clients
			.Where(c => c.UserId == userId)
			.OrderBy(c => c.Name)
			.ToListAsync();

	public async Task AddAsync(Client client) =>
		await db.Clients.AddAsync(client);

	public void Remove(Client client) =>
		db.Clients.Remove(client);

    public async Task<bool> ExistsByCpfAsync(string cpf, int userId)
    {
        // CORRIGIDO: Se usar o Primary Constructor acima, a variável se chama "context" (sem o underline)
        return await db.Clients
            .AnyAsync(c => c.CPF == cpf && c.UserId == userId);
    }
}