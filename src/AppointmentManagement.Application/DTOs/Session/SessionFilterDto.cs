using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Application.DTOs.Session;

public class SessionFilterDto
{
    public int?           ClientId  { get; set; }
    public SessionStatus? Status    { get; set; }
    public DateOnly?      DateFrom  { get; set; }
    public DateOnly?      DateTo    { get; set; }
}