using AppointmentManagement.Domain.ValueObjects;

namespace AppointmentManagement.Domain.Entities;


public class FinancialResponsible
{
	public int Id {  get; set; }
	public required string Name { get; set; }
	public required string Phone { get; set; }
	public required string CPF { get; set; }
}