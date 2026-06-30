using AppointmentManagement.Application.Interfaces.Services;
using AppointmentManagement.Application.DTOs.ValueObjects;
using System.Text.Json;

namespace AppointmentManagement.Infrastructure.Services;

public class CepService : ICepService
{
    private readonly HttpClient _httpClient;

    public CepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CepResponseDto?> BuscarCepAsync(string cep)
    {
        cep = cep.Replace("-", "").Trim();

        var response = await _httpClient.GetAsync(
            $"https://viacep.com.br/ws/{cep}/json/"
        );

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<CepResponseDto>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result;
    }
}