using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Application.DTOs.Session;

public class UpdateSessionStatusDto
{
    public SessionStatus Status { get; set; }
}