namespace AppointmentManagement.Application.DTOs.Invoice;

public class BatchInvoiceDto
{
	public List<int> ClientIds { get; set; } = [];
	public int Month { get; set; }
	public int Year { get; set; }
}