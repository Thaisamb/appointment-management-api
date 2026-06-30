using AppointmentManagement.Domain.ValueObjects;

namespace AppointmentManagement.Domain.Entities;

public class InvoiceIssuer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string CPF { get; set; }
    public required string Email {  get; set; }
    public required Address Address { get; set; }
}