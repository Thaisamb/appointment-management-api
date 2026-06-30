namespace AppointmentManagement.Application.DTOs.Invoice;

public class CreateInvoiceDto
{
    public int ClientId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalAmount { get; set; }
}