namespace AppointmentManagement.Domain.ValueObjects;

public record EmergencyContact(
    string? Name,
    string? Relation,
    string? Phone
);