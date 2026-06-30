using AppointmentManagement.Application.DTOs.Invoice;
using AppointmentManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagement.API.Controllers.Invoice;

[ApiController]
[Route("api/invoice-issuance")]
[Authorize]
public class InvoiceIssuanceController(IInvoiceIssuanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var config = await service.GetAsync();
        return config is null ? NoContent() : Ok(config);
    }

    [HttpPost]
    public async Task<IActionResult> Save(SaveInvoiceIssuanceDto dto)
    {
        await service.SaveAsync(dto);
        return NoContent();
    }

    [HttpPost("certificate")]
    public async Task<IActionResult> UploadCertificate([FromForm] UploadCertificateDto dto)
    {
        await service.UploadCertificateAsync(dto);
        return NoContent();
    }
}