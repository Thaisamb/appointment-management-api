using Microsoft.AspNetCore.Http;

namespace AppointmentManagement.Application.DTOs.Invoice;

public class UploadCertificateDto
{
    public IFormFile Certificate { get; set; } = null!;
    public string Password { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}