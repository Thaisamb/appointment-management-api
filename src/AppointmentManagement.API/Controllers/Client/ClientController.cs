using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentManagement.Application.DTOs.Client;
using AppointmentManagement.Application.Interfaces.Services;

namespace AppointmentManagement.API.Controllers.Client;

[Authorize]
[ApiController]
[Route("api/clients")]
public class ClientController : ControllerBase
{
    private readonly IClientService _service;

    public ClientController(IClientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _service.GetByIdAsync(id);
        return client == null ? NotFound() : Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClientDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateClientDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}