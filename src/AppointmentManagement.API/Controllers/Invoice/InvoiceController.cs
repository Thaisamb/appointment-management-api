using AppointmentManagement.Application.DTOs.Invoice;
using AppointmentManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagement.API.Controllers.Invoice;

[ApiController]
[Route("api/invoices")]
[Authorize]
public class InvoiceController(IInvoiceService invoiceService) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Emitir(CreateInvoiceDto dto)
	{
		var result = await invoiceService.EmitirAsync(dto);
		return Ok(result);
	}

	[HttpPost("batch")]
	public async Task<IActionResult> EmitirBatch(BatchInvoiceDto dto)
	{
		var result = await invoiceService.EmitirBatchAsync(dto);
		return Ok(result);
	}

	[HttpGet("quota")]
	public async Task<IActionResult> GetQuota([FromQuery] int month, [FromQuery] int year)
	{
		var used = await invoiceService.GetQuotaUsedAsync(month, year);
		return Ok(new { used, quota = 40, remaining = Math.Max(0, 40 - used) });
	}
}