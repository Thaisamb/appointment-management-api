using AppointmentManagement.Infrastructure.Data;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Application.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using AppointmentManagement.Application.Configurations;

namespace AppointmentManagement.Application.Services.Auth;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwt;

    public AuthService(AppDbContext context, IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _jwt = jwtOptions.Value;
    }

    public async Task<string> Register(RegisterDto dto)
    {
        await EnsureEmailIsUnique(dto.Email);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            CPF = dto.CPF,
            CNPJ = dto.CNPJ,
            CompanyName = dto.CompanyName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<string> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !IsValidPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Credenciais inválidas");

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.Key)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task EnsureEmailIsUnique(string email)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == email);

        if (exists)
            throw new InvalidOperationException("Email já cadastrado");
    }

    private bool IsValidPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}