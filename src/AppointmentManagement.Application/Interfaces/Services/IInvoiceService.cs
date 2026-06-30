using AppointmentManagement.Application.DTOs.Invoice;

namespace AppointmentManagement.Application.Interfaces.Services;

public interface IInvoiceService
{
    Task<InvoiceSummaryDto> EmitirAsync(CreateInvoiceDto dto);
    Task<BatchInvoiceResultDto> EmitirBatchAsync(BatchInvoiceDto dto);
    Task<int> GetQuotaUsedAsync(int month, int year);
}