using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentManagement.Application.Interfaces.Services;

[Authorize]
[ApiController]
[Route("api/cep")]
public class CepController : ControllerBase
{
    private readonly ICepService _cepService;

    public CepController(ICepService cepService)
    {
        _cepService = cepService;
    }

    [HttpGet]
    public async Task<IActionResult> Buscar(string cep)
    {
        var result = await _cepService.BuscarCepAsync(cep);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}