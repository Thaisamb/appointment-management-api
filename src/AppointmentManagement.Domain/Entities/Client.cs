using AppointmentManagement.Domain.ValueObjects;

namespace AppointmentManagement.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public int FinancialResponsibleId { get; set; }
    public int InvoiceIssuerId { get; set; }
    
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string CPF { get; set; }
    public required decimal PricePerSession { get; set; }
    public required DateTime BirthDate { get; set; }
    public required DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public string? Notes { get; set; }
    

    public virtual User User { get; set; } = null!;
    public virtual FinancialResponsible FinancialResponsible { get; set; } = null!;
    public virtual InvoiceIssuer InvoiceIssuer { get; set; } = null!;

    public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}