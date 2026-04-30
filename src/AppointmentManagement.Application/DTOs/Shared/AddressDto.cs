namespace AppointmentManagement.Application.DTOs.Shared;

public record AddressDto(
	string Street,
	string Number,
	string? Complement,
	string City,
	string State,
	string ZipCode
);