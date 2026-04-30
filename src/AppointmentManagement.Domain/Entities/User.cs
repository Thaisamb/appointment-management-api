namespace AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.ValueObjects;

public class User
{
    public int Id { get; set; }

    // 🔐 autenticação
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    // 👤 dados pessoais
    public required string Name { get; set; }
    public required string CPF { get; set; }
    public Address Address { get; set; }

    // 🏢 opcional (empresa)
    public string? CNPJ { get; set; }
    public string? CompanyName { get; set; }

    // 📊 relacionamento
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}