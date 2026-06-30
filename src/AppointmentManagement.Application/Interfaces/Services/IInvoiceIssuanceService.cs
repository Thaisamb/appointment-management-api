using AppointmentManagement.Application.DTOs.Invoice;

namespace AppointmentManagement.Application.Interfaces.Services;

public interface IInvoiceIssuanceService
{
    Task<InvoiceIssuanceDto?> GetAsync();
    Task SaveAsync(SaveInvoiceIssuanceDto dto);
    Task UploadCertificateAsync(UploadCertificateDto dto);
}