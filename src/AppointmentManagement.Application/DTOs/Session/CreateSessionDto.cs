using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Application.DTOs.Session;

public class CreateSessionDto
{
    public int ClientId { get; set; }
    public DateTime DateTime { get; set; }
    public int Duration { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Scheduled;
    public RepetitionDto? Repetition { get; set; }
    public decimal? PricePerSession { get; set; }
}

public class RepetitionDto
{
    // "none" | "weekly" | "biweekly" | "monthly"
    public string Type { get; set; } = "none";
    public DateTime? EndDate { get; set; }

    // Semanal e quinzenal: lista de dias da semana (0=Dom..6=Sáb)
    // Quinzenal aceita só 1 elemento
    public List<DayOfWeek>? WeekDays { get; set; }

    // Mensal: qual semana (1..5 onde 5=última) e qual dia
    public int? WeekOfMonth { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
}