using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Domain.Entities;

namespace AppointmentManagement.Domain.Entities;

public class Session
{
    public int Id { get; set; }
	public int UserId { get; set; }
	public User? User { get; set; }
	public int ClientId { get; set; }
    public Client? Client { get; set; }

    public DateTime DateTime { get; set; }
    public int Duration { get; set; }

    public SessionStatus Status { get; set; }

    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    // Grupo de repetição — todas as sessões geradas juntas
    // compartilham o mesmo RepetitionGroupId
    public Guid? RepetitionGroupId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public decimal PricePerSession{ get; set; }
}