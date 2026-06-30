using AppointmentManagement.Application.DTOs.ValueObjects;
using AppointmentManagement.Domain.Entities; 

namespace AppointmentManagement.Application.DTOs.Client;

public class ClientResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public decimal PricePerSession { get; set; }
    public string? Notes { get; set; }
    public DateTime? BirthDate { get; set; }
    public AddressDto? Address { get; set; }
    public List<EmergencyContactDto>? EmergencyContacts { get; set; }

    public static ClientResponseDto FromEntity(Domain.Entities.Client c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.InvoiceIssuer?.Email ?? string.Empty,
        Phone = c.Phone,
        CPF = c.CPF,
        PricePerSession = c.PricePerSession,
        Notes = c.Notes,
        BirthDate = c.BirthDate,
        Address = c.InvoiceIssuer?.Address is null ? null : new AddressDto
        {
            Street = c.InvoiceIssuer.Address.Street,
            District = c.InvoiceIssuer.Address.District,
            City = c.InvoiceIssuer.Address.City,
            State = c.InvoiceIssuer.Address.State,
            ZipCode = c.InvoiceIssuer.Address.ZipCode,
            Number = c.InvoiceIssuer.Address.Number
        },
        EmergencyContacts = c.EmergencyContacts?.Select(ec => new EmergencyContactDto
        {
            Name = ec.Name,
            Phone = ec.Phone,
            Relationship = ec.Relationship
        }).ToList() ?? []
    };
}
