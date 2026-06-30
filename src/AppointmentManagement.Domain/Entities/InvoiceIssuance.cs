using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Domain.Entities;

public class InvoiceIssuance
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    // Preferência de emissão (CPF via gov.br ou CNPJ via certificado)
    public TypeInvoiceIssuer IssuerType { get; set; } = TypeInvoiceIssuer.CPF;

    // CPF — login gov.br (Focus NFe)
    public string? GovBrLogin { get; set; }      // CPF
    public string? GovBrPasswordEncrypted { get; set; }

    // CNPJ — certificado digital
    public string? CNPJ { get; set; }
    public bool IsMei { get; set; }
    public string? CertificatePath { get; set; }      // caminho no servidor
    public string? CertificatePasswordEncrypted { get; set; }
    public DateTime? CertificateExpiry { get; set; }

    // Dados de serviço (necessários para emissão)
    public string? MunicipalRegistration { get; set; }  // Inscrição municipal
    public string? ServiceCode { get; set; }             // Item lista serviço ex: "14.01"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    //API de emissão (ex.: focus)
    public string? FocusNFeCompanyId { get; set; } // retorno do cadastro na Focus
    public bool IsRegisteredOnFocusNFe { get; set; } = false;
}