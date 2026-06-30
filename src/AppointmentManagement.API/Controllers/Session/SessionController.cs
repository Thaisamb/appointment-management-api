using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AppointmentManagement.Application.DTOs.Session;
using AppointmentManagement.Application.Interfaces.Services;

namespace AppointmentManagement.API.Controllers.Session;

[ApiController]
[Route("api/sessions")]
[Authorize]
public class SessionController(ISessionService sessionService) : ControllerBase
{
    [HttpGet("form-data")]
    public async Task<ActionResult<SessionFormDataDto>> GetFormData()
    {
        var data = await sessionService.GetFormDataAsync();
        return Ok(data);
    }

    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<int>>> Create([FromBody] CreateSessionDto dto)
    {
        var ids = await sessionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetFormData), new { }, ids);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SessionResponseDto>>> GetAll(
        [FromQuery] SessionFilterDto filter)
    {
        var sessions = await sessionService.GetByUserAsync(filter);
        return Ok(sessions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SessionResponseDto>> GetById(int id)
    {
        var session = await sessionService.GetByIdAsync(id);
        return Ok(session);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSessionDto dto)
    {
        await sessionService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateSessionStatusDto dto)
    {
        await sessionService.UpdateStatusAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("group")]
    public async Task<IActionResult> UpdateGroup([FromBody] UpdateSessionGroupDto dto)
    {
        await sessionService.UpdateGroupAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await sessionService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("group/{groupId}")]
    public async Task<IActionResult> DeleteGroup(Guid groupId)
    {
        await sessionService.DeleteGroupAsync(groupId);
        return NoContent();
    }
}