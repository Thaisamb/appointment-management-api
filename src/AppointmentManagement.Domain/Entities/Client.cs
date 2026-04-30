namespace AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.ValueObjects;

public class Client
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string CPF { get; set; }

    public DateTime? BirthDate { get; set; }

    public required decimal PricePerSession { get; set; }

    public Address? Address { get; set; }
    public ICollection<EmergencyContact>? EmergencyContacts { get; set; } = new List<EmergencyContact>();

    public string? Notes { get; set; }

    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}