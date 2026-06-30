using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Application.DTOs.Invoice;

public class InvoiceSummaryDto
{
	public int Id { get; set; }
	public InvoiceStatus Status { get; set; }
	public string? InvoiceNumber { get; set; }
	public string? PdfUrl { get; set; }
	public DateTime? IssuedAt { get; set; }
}