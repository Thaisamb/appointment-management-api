namespace AppointmentManagement.Application.DTOs.FocusNFe;

public class FocusNFeRequestDto
{
	public string CpfCnpjPrestador { get; set; } = string.Empty;
	public string? InscricaoMunicipal { get; set; }
	public string NomeTomador { get; set; } = string.Empty;
	public string CpfCnpjTomador { get; set; } = string.Empty;
	public string EmailTomador { get; set; } = string.Empty;
	public string Discriminacao { get; set; } = string.Empty;
	public decimal ValorServico { get; set; }
	public string CodigoServico { get; set; } = string.Empty;
	public DateTime DataEmissao { get; set; }
}