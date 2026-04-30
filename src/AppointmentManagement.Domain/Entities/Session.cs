namespace AppointmentManagement.Domain.Entities;

using AppointmentManagement.Domain.Enums;

public class Session
{
    public int Id { get; set; }
	public int UserId { get; set; }
	public required User User { get; set; }
	public int ClientId { get; set; }
    public required Client Client { get; set; }

    public DateTime Date { get; set; }
    public int Duration { get; set; }

    public SessionStatus Status { get; set; }

    public decimal Price { get; set; }

    public string? InvoiceNumber { get; set; }
}