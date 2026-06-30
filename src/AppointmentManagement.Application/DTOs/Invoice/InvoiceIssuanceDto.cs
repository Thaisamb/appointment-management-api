using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Application.DTOs.Invoice;

public class InvoiceIssuanceDto
{
    public TypeInvoiceIssuer IssuerType { get; set; }
    public string? GovBrLogin { get; set; }
    public bool HasGovBrPassword { get; set; }   // nunca retorna a senha
    public string? CNPJ { get; set; }
    public bool IsMei { get; set; }
    public bool HasCertificate { get; set; }     // nunca retorna o path
    public DateTime? CertificateExpiry { get; set; }
    public string? MunicipalRegistration { get; set; }
    public string? ServiceCode { get; set; }
}