using AppointmentManagement.Application.DTOs.FocusNFe;

namespace AppointmentManagement.Application.Interfaces.Services;

public interface IFocusNFeService
{
	Task<FocusNFeResponseDto> EmitirNFSeAsync(FocusNFeRequestDto dto);
}