namespace AppointmentManagement.Application.DTOs.ValueObjects;

public class CepResponseDto
{
	public string Cep { get; set; }
	public string Logradouro { get; set; }
	public string Complemento { get; set; }
	public string Bairro { get; set; }
	public string Localidade { get; set; }
	public string Uf { get; set; }
}