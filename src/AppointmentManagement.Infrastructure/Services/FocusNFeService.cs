using AppointmentManagement.Application.Configurations;
using AppointmentManagement.Application.DTOs.FocusNFe;
using AppointmentManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AppointmentManagement.Infrastructure.Services;

public class FocusNFeService(
	HttpClient httpClient,
	IOptions<FocusNFeSettings> settings) : IFocusNFeService
{
	private readonly FocusNFeSettings _settings = settings.Value;

	public async Task<FocusNFeResponseDto> EmitirNFSeAsync(FocusNFeRequestDto dto)
	{
		var payload = new
		{
			data_emissao = dto.DataEmissao.ToString("yyyy-MM-dd"),
			prestador = new
			{
				cpf_cnpj = dto.CpfCnpjPrestador,
				inscricao_municipal = dto.InscricaoMunicipal
			},
			tomador = new
			{
				cpf_cnpj = dto.CpfCnpjTomador,
				razao_social = dto.NomeTomador,
				email = dto.EmailTomador
			},
			servico = new
			{
				discriminacao = dto.Discriminacao,
				valor_servico = dto.ValorServico,
				item_lista_servico = dto.CodigoServico
			}
		};

		var json = JsonSerializer.Serialize(payload);
		var content = new StringContent(json, Encoding.UTF8, "application/json");

		// Focus NFe usa Basic Auth com token como usuário e senha vazia
		var authToken = Convert.ToBase64String(
			Encoding.UTF8.GetBytes(_settings.Token + ":"));
		httpClient.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

		var response = await httpClient.PostAsync(
			$"{_settings.BaseUrl}/v2/nfse", content);

		var responseBody = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode)
			return new FocusNFeResponseDto
			{
				Sucesso = false,
				ErroMensagem = responseBody
			};

		using var doc = JsonDocument.Parse(responseBody);
		var root = doc.RootElement;

		return new FocusNFeResponseDto
		{
			Sucesso = true,
			InvoiceNumber = root.TryGetProperty("numero", out var n) ? n.GetString() : null,
			PdfUrl = root.TryGetProperty("url_danfse", out var p) ? p.GetString() : null,
			XmlUrl = root.TryGetProperty("url_xml", out var x) ? x.GetString() : null
		};
	}
}