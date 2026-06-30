namespace AppointmentManagement.Application.DTOs.Session;

public class SessionFormDataDto
{
    public IReadOnlyList<ClientOptionDto> Clients { get; set; } = [];
    public IReadOnlyList<StatusOptionDto> Statuses { get; set; } = [];
    public IReadOnlyList<DurationOptionDto> Durations { get; set; } = [];
    public IReadOnlyList<RepetitionOptionDto> Repetitions { get; set; } = [];
}

public record ClientOptionDto(int Id, string Name, decimal Price);

public record StatusOptionDto(int Value, string Label);

public record DurationOptionDto(int Value, string Label);

public record RepetitionOptionDto(string Value, string Label);