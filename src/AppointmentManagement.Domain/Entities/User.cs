using AppointmentManagement.Domain.ValueObjects;
using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Domain.Entities;


public class User
{
    public int Id { get; set; }

    // 🔐 autenticação
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    // 👤 dados pessoais
    public required string Name { get; set; }
    public required string CPF { get; set; }
    public Address? Address { get; set; }


    // 🏢 opcional (empresa)
    public string? CNPJ { get; set; }
    public string? CompanyName { get; set; }

    // nota fiscal
    public string? MunicipalRegistration { get; set; }
    public string? ServiceCode { get; set; }      // item lista serviço ex: "14.01"
    public TypeInvoiceIssuer TypeInvoiceIssuer { get; set; } = TypeInvoiceIssuer.CPF;
    public int InvoicesEmitted { get; set; } = 0;      // total emitido
    public int InvoicesFreeQuota { get; set; } = 100;   // cota gratuita

    // 📊 relacionamento
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}