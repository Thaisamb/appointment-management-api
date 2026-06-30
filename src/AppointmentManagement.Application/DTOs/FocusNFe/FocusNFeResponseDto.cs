namespace AppointmentManagement.Application.DTOs.FocusNFe;

public class FocusNFeResponseDto
{
    public string? InvoiceNumber { get; set; }
    public string? PdfUrl { get; set; }
    public string? XmlUrl { get; set; }
    public bool Sucesso { get; set; }
    public string? ErroMensagem { get; set; }
}