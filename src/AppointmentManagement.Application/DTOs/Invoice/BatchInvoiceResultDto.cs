namespace AppointmentManagement.Application.DTOs.Invoice;

public class BatchInvoiceResultDto
{
    public int TotalSelected { get; set; }
    public int TotalEmitted { get; set; }
    public int TotalSkipped { get; set; }
    public int QuotaUsed { get; set; }
    public int QuotaRemaining { get; set; }
    public List<SkippedInvoiceDto> Skipped { get; set; } = [];
}

public class SkippedInvoiceDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}