using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Application.DTOs.Session;

public class UpdateSessionDto
{
    public DateTime DateTime { get; set; }
    public int Duration { get; set; }
    public SessionStatus StatusSession {  get; set; }
    public decimal PricePerSession { get; set; }
}
