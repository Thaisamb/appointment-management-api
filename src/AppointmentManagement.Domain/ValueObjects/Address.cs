namespace AppointmentManagement.Domain.ValueObjects;

public record Address(
    string? Street,
    string? District,
    string? City,
    string? State,
    string? ZipCode,
    string? Number,
    string? Complement
);