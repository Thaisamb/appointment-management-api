using AppointmentManagement.Application.DTOs.Session;

namespace AppointmentManagement.Application.Interfaces.Services;

public interface ISessionService
{
    Task<SessionFormDataDto> GetFormDataAsync();
    Task<IReadOnlyList<int>> CreateAsync(CreateSessionDto dto);
    Task<SessionResponseDto> GetByIdAsync(int id);
    Task<IReadOnlyList<SessionResponseDto>> GetByUserAsync(SessionFilterDto filter);
    Task UpdateAsync(int id, UpdateSessionDto dto);
    Task UpdateStatusAsync(int id, UpdateSessionStatusDto dto);
    Task UpdateGroupAsync(UpdateSessionGroupDto dto);
    Task DeleteAsync(int id);
    Task DeleteGroupAsync(Guid groupId);
}