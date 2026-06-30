using AppointmentManagement.Application.DTOs.FocusNFe;
using AppointmentManagement.Application.DTOs.Invoice;
using AppointmentManagement.Application.DTOs.Session;
using AppointmentManagement.Application.Exceptions;
using AppointmentManagement.Application.Interfaces;
using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Application.Interfaces.Services;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Infrastructure.Services;

public class InvoiceService(
    IInvoiceRepository invoiceRepository,
    IInvoiceIssuanceRepository issuanceRepository,
    IClientRepository clientRepository,
    ISessionRepository sessionRepository,
    ICurrentUserService currentUser,
    IFocusNFeService focusNFe,
    IUnitOfWork uow) : IInvoiceService
{
    public async Task<int> GetQuotaUsedAsync(int month, int year) =>
        await invoiceRepository.CountEmittedByUserAsync(currentUser.UserId, month, year);

    public async Task<InvoiceSummaryDto> EmitirAsync(CreateInvoiceDto dto)
    {
        var userId = currentUser.UserId;

        var config = await issuanceRepository.GetByUserAsync(userId)
            ?? throw new ValidationException("Configure os dados de emissão de NF antes de emitir.");

        // 1. ATUALIZADO: Busca o cliente com os detalhes (Includes de InvoiceIssuer)
        var client = await clientRepository.GetByIdWithDetailsAsync(dto.ClientId)
            ?? throw new NotFoundException($"Cliente {dto.ClientId} não encontrado.");

        if (client.UserId != userId)
            throw new ForbiddenException("Cliente não pertence a este usuário.");

        var sessions = await sessionRepository.GetByUserAsync(userId, new SessionFilterDto
        {
            ClientId = dto.ClientId,
            DateFrom = new DateOnly(dto.Year, dto.Month, 1),
            DateTo = new DateOnly(dto.Year, dto.Month, DateTime.DaysInMonth(dto.Year, dto.Month))
        });

        var paidSessions = sessions
            .Where(s => s.Status == SessionStatus.Paid && s.InvoiceId == null)
            .ToList();

        if (!paidSessions.Any())
            throw new ValidationException("Não há sessões pagas sem nota fiscal neste período.");

        var cpfCnpj = config.IssuerType == TypeInvoiceIssuer.CNPJ
            ? config.CNPJ!
            : config.GovBrLogin!;

        var discriminacao = $"Sessões de psicoterapia - " +
            $"{new DateTime(dto.Year, dto.Month, 1):MMMM/yyyy} - {client.Name}";

        // 2. ATUALIZADO: Lê os dados fiscais de client.InvoiceIssuer e não da raiz do cliente
        var focusRequest = new FocusNFeRequestDto
        {
            CpfCnpjPrestador = cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", ""),
            InscricaoMunicipal = config.MunicipalRegistration,

            // Dados direcionados do Tomador da Nota correto
            NomeTomador = client.InvoiceIssuer?.Name ?? client.Name,
            CpfCnpjTomador = (client.InvoiceIssuer?.CPF ?? client.CPF).Replace(".", "").Replace("-", ""),
            EmailTomador = client.InvoiceIssuer?.Email ?? string.Empty,

            Discriminacao = discriminacao,
            ValorServico = dto.TotalAmount,
            CodigoServico = config.ServiceCode ?? "14.01",
            DataEmissao = DateTime.Today

            // Dica: Se o seu FocusNFeRequestDto exigir endereço futuramente,
            // você usará client.InvoiceIssuer.Address.Street, etc.
        };

        var focusResponse = await focusNFe.EmitirNFSeAsync(focusRequest);

        var invoice = new Invoice
        {
            UserId = userId,
            ClientId = dto.ClientId,
            Month = dto.Month,
            Year = dto.Year,
            TotalAmount = dto.TotalAmount,
            Status = focusResponse.Sucesso ? InvoiceStatus.Issued : InvoiceStatus.Error,
            InvoiceNumber = focusResponse.InvoiceNumber,
            PdfUrl = focusResponse.PdfUrl,
            XmlUrl = focusResponse.XmlUrl,
            ErrorMessage = focusResponse.ErroMensagem,
            IssuedAt = focusResponse.Sucesso ? DateTime.UtcNow : null
        };

        await invoiceRepository.AddAsync(invoice);
        await uow.CommitAsync();

        foreach (var s in paidSessions)
            s.InvoiceId = invoice.Id;

        await uow.CommitAsync();

        if (!focusResponse.Sucesso)
            throw new ValidationException($"Erro ao emitir NF: {focusResponse.ErroMensagem}");

        return new InvoiceSummaryDto
        {
            Id = invoice.Id,
            Status = invoice.Status,
            InvoiceNumber = invoice.InvoiceNumber,
            PdfUrl = invoice.PdfUrl,
            IssuedAt = invoice.IssuedAt
        };
    }

    public async Task<BatchInvoiceResultDto> EmitirBatchAsync(BatchInvoiceDto dto)
    {
        var userId = currentUser.UserId;
        var quotaUsed = await invoiceRepository.CountEmittedByUserAsync(userId, dto.Month, dto.Year);
        var quotaTotal = 40;
        var quotaRemaining = Math.Max(0, quotaTotal - quotaUsed);

        var result = new BatchInvoiceResultDto
        {
            TotalSelected = dto.ClientIds.Count,
            QuotaUsed = quotaUsed,
            QuotaRemaining = quotaRemaining
        };

        var toEmit = dto.ClientIds.Take(quotaRemaining).ToList();
        var toSkip = dto.ClientIds.Skip(quotaRemaining).ToList();

        foreach (var clientId in toEmit)
        {
            try
            {
                var sessions = await sessionRepository.GetByUserAsync(userId, new SessionFilterDto
                {
                    ClientId = clientId,
                    DateFrom = new DateOnly(dto.Year, dto.Month, 1),
                    DateTo = new DateOnly(dto.Year, dto.Month,
                                   DateTime.DaysInMonth(dto.Year, dto.Month))
                });

                var paidSessions = sessions
                    .Where(s => s.Status == SessionStatus.Paid && s.InvoiceId == null)
                    .ToList();

                if (!paidSessions.Any()) continue;

                var totalAmount = paidSessions.Sum(s => s.PricePerSession);

                await EmitirAsync(new CreateInvoiceDto
                {
                    ClientId = clientId,
                    Month = dto.Month,
                    Year = dto.Year,
                    TotalAmount = totalAmount
                });

                result.TotalEmitted++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro cliente {clientId}: {ex.Message} | {ex.InnerException?.Message}");
            }
        }

        foreach (var clientId in toSkip)
        {
            // 3. ATUALIZADO: Mantido GetByIdAsync aqui apenas porque o skip usa apenas o Name do cliente
            var client = await clientRepository.GetByIdAsync(clientId);
            if (client is null) continue;

            var sessions = await sessionRepository.GetByUserAsync(userId, new SessionFilterDto
            {
                ClientId = clientId,
                DateFrom = new DateOnly(dto.Year, dto.Month, 1),
                DateTo = new DateOnly(dto.Year, dto.Month,
                               DateTime.DaysInMonth(dto.Year, dto.Month))
            });

            var total = sessions
                .Where(s => s.Status == SessionStatus.Paid && s.InvoiceId == null)
                .Sum(s => s.PricePerSession);

            result.Skipped.Add(new SkippedInvoiceDto
            {
                ClientId = clientId,
                ClientName = client.Name,
                TotalAmount = total
            });
        }

        result.TotalSkipped = result.Skipped.Count;
        result.QuotaRemaining = Math.Max(0, quotaRemaining - result.TotalEmitted);

        return result;
    }
}
