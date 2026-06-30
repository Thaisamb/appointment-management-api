using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Application.DTOs.Invoice;

public class SaveInvoiceIssuanceDto
{
    public TypeInvoiceIssuer IssuerType { get; set; }

    // CPF path
    public string? GovBrLogin { get; set; }
    public string? GovBrPassword { get; set; }  // plain — service criptografa

    // CNPJ path
    public string? CNPJ { get; set; }
    public bool IsMei { get; set; }
    public string? MunicipalRegistration { get; set; }
    public string? ServiceCode { get; set; }
}