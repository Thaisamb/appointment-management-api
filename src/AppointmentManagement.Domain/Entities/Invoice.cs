using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    public string? InvoiceNumber { get; set; }
    public string? PdfUrl { get; set; }
    public string? XmlUrl { get; set; }
    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? IssuedAt { get; set; }

    public ICollection<Session> Sessions { get; set; } = [];
}