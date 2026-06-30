using AppointmentManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AppointmentManagement.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
	public int UserId =>
		int.Parse(httpContextAccessor.HttpContext!.User
			.FindFirstValue(ClaimTypes.NameIdentifier)!);

	public string UserName =>
		httpContextAccessor.HttpContext!.User
			.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
}