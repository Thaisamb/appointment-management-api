using AppointmentManagement.Domain.Enums;
using AppointmentManagement.Application.DTOs.Invoice;

namespace AppointmentManagement.Application.DTOs.Session;

public class SessionResponseDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public int Duration { get; set; }
    public SessionStatus Status { get; set; }
    public decimal PricePerSession{ get; set; }
    public InvoiceSummaryDto? Invoice { get; set; }
    public Guid? RepetitionGroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}