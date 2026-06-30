using AppointmentManagement.Application.DTOs.ValueObjects;

namespace AppointmentManagement.Application.DTOs.Client;

public class CreateClientDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public decimal PricePerSession { get; set; }
    public required DateTime BirthDate { get; set; }
    public string? Notes { get; set; }

    public string FinancialResponsibleName { get; set; }
    public string FinancialResponsibleCPF { get; set; }
    public string FinancialResponsiblePhone {  get; set; }

    public bool ClientAsIssuer { get; set; }

    public ICollection<EmergencyContactDto> EmergencyContacts { get; set; }
    public AddressDto Address { get; set; }
}

