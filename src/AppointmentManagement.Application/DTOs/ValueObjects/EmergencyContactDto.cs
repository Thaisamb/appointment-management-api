namespace AppointmentManagement.Application.DTOs.ValueObjects;

public class EmergencyContactDto
{
	public string Name { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string Relationship { get; set; } = string.Empty;
}