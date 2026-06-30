using AppointmentManagement.Application.DTOs.Session;
using AppointmentManagement.Application.DTOs.Invoice;
using AppointmentManagement.Application.Exceptions;
using AppointmentManagement.Application.Interfaces;
using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Application.Interfaces.Services;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;

namespace AppointmentManagement.Infrastructure.Services;

public class SessionService(
    ISessionRepository sessionRepository,
    IClientRepository clientRepository,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : ISessionService
{
    // ── Form Data ────────────────────────────────────────────
    public async Task<SessionFormDataDto> GetFormDataAsync()
    {
        var userId = currentUser.UserId;
        var clients = await clientRepository.GetAllByUserAsync(userId);

        return new SessionFormDataDto
        {
            Clients = clients
                .Select(c => new ClientOptionDto(c.Id, c.Name, c.PricePerSession))
                .ToList(),

            Statuses =
            [
                new((int)SessionStatus.Scheduled, "Agendado"),
                new((int)SessionStatus.Confirmed, "Confirmado")
            ],

            Durations =
            [
                new(50,  "50 min"),
                new(60,  "60 min"),
                new(90,  "90 min"),
                new(120, "2 horas")
            ],

            Repetitions =
            [
                new("none",     "Sem repetição"),
                new("weekly",   "Semanal"),
                new("biweekly", "Quinzenal"),
                new("monthly",  "Mensal")
            ]
        };
    }

    // ── Create ───────────────────────────────────────────────
    public async Task<IReadOnlyList<int>> CreateAsync(CreateSessionDto dto)
    {
        var userId = currentUser.UserId;

        var client = await clientRepository.GetByIdAsync(dto.ClientId)
            ?? throw new NotFoundException($"Cliente {dto.ClientId} não encontrado.");

        if (client.UserId != userId)
            throw new ForbiddenException("Cliente não pertence a este usuário.");

        var dates = GenerateDates(dto.DateTime, dto.Repetition);
        var groupId = dates.Count > 1 ? Guid.NewGuid() : (Guid?)null;

        var sessions = dates.Select(date => new Session
        {
            UserId = userId,
            ClientId = dto.ClientId,
            DateTime = date,
            Duration = dto.Duration,
            Status = dto.Status,
            PricePerSession = dto.PricePerSession ?? client.PricePerSession,
            RepetitionGroupId = groupId,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await sessionRepository.AddRangeAsync(sessions);
        await uow.CommitAsync();

        return sessions.Select(s => s.Id).ToList();
    }

    // ── GetById ──────────────────────────────────────────────
    public async Task<SessionResponseDto> GetByIdAsync(int id)
    {
        var session = await sessionRepository.GetByIdWithClientAsync(id)
            ?? throw new NotFoundException($"Sessão {id} não encontrada.");

        EnsureOwnership(session);
        return MapToResponse(session);
    }

    // ── GetByUser ────────────────────────────────────────────
    public async Task<IReadOnlyList<SessionResponseDto>> GetByUserAsync(SessionFilterDto filter)
    {
        var sessions = await sessionRepository.GetByUserAsync(currentUser.UserId, filter);
        return sessions.Select(MapToResponse).ToList();
    }

    // ── Update ───────────────────────────────────────────────
    public async Task UpdateAsync(int id, UpdateSessionDto dto)
    {
        var session = await sessionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Sessão {id} não encontrada.");

        EnsureOwnership(session);

        session.DateTime = dto.DateTime;
        session.Duration = dto.Duration;
        session.PricePerSession = dto.PricePerSession;
        session.UpdatedAt = DateTime.UtcNow;

        await uow.CommitAsync();
    }
    // ── UpdateStatus ─────────────────────────────────────────
    public async Task UpdateStatusAsync(int id, UpdateSessionStatusDto dto)
    {
        var session = await sessionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Sessão {id} não encontrada.");

        EnsureOwnership(session);
        ValidateStatusTransition(session.Status, dto.Status, session);

        session.Status = dto.Status;
        session.UpdatedAt = DateTime.UtcNow;

        await uow.CommitAsync();

    }
    // ── UpdateGroup ─────────────────────────────────────────
    public async Task UpdateGroupAsync(UpdateSessionGroupDto dto)
    {
        var sessions = await sessionRepository.GetByGroupIdAsync(dto.GroupId);

        if (!sessions.Any())
            throw new NotFoundException($"Grupo {dto.GroupId} não encontrado.");

        if (sessions.Any(s => s.UserId != currentUser.UserId))
            throw new ForbiddenException("Acesso negado ao grupo.");

        var futureSessions = sessions
            .Where(s => s.DateTime >= dto.FromDateTime && s.Status != SessionStatus.Cancelled)
            .OrderBy(s => s.DateTime)
            .ToList();

        // Calcula o offset de tempo entre a sessão original e o novo horário
        var offset = dto.NewDateTime - dto.FromDateTime;

        foreach (var s in futureSessions)
        {
            s.DateTime = s.DateTime + offset;
            s.Duration = dto.Duration;
            s.PricePerSession = dto.PricePerSession;
            s.UpdatedAt = DateTime.UtcNow;
        }

        await uow.CommitAsync();
    }

    // ── Delete ───────────────────────────────────────────────
    public async Task DeleteAsync(int id)
    {
        var session = await sessionRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Sessão {id} não encontrada.");

        EnsureOwnership(session);
        sessionRepository.Remove(session);
        await uow.CommitAsync();
    }

    // ── DeleteGroup ──────────────────────────────────────────
    public async Task DeleteGroupAsync(Guid groupId)
    {
        var sessions = await sessionRepository.GetByGroupIdAsync(groupId);

        if (!sessions.Any())
            throw new NotFoundException($"Grupo {groupId} não encontrado.");

        if (sessions.Any(s => s.UserId != currentUser.UserId))
            throw new ForbiddenException("Acesso negado ao grupo de repetição.");

        sessionRepository.RemoveRange(sessions);
        await uow.CommitAsync();
    }

    // ── Geração de datas ─────────────────────────────────────
    private static List<DateTime> GenerateDates(DateTime start, RepetitionDto? rep)
    {
        if (rep is null || rep.Type == "none")
            return [start];

        if (rep.EndDate is null)
            throw new ValidationException("EndDate é obrigatório para repetições.");

        var dates = new List<DateTime>();
        var end = rep.EndDate.Value.Date.AddDays(1).AddTicks(-1);
        var sh = start.Hour;
        var sm = start.Minute;

        switch (rep.Type)
        {
            case "weekly":
                var cur = start.Date;
                while (cur <= end)
                {
                    if (rep.WeekDays!.Contains(cur.DayOfWeek))
                    {
                        var d = cur.AddHours(sh).AddMinutes(sm);
                        if (d >= start) dates.Add(d);
                    }
                    cur = cur.AddDays(1);
                }
                break;

            case "biweekly":
                var targetDay = rep.WeekDays!.First();
                var first = start.Date;
                var diff = ((int)targetDay - (int)first.DayOfWeek + 7) % 7;
                first = first.AddDays(diff);
                var bi = first.AddHours(sh).AddMinutes(sm);
                if (bi < start) bi = bi.AddDays(14);
                while (bi <= end)
                {
                    dates.Add(bi);
                    bi = bi.AddDays(14);
                }
                break;

            case "monthly":
                var month = new DateTime(start.Year, start.Month, 1);
                while (month <= end)
                {
                    var nd = GetNthWeekday(month.Year, month.Month,
                                           rep.WeekOfMonth!.Value,
                                           rep.DayOfWeek!.Value);
                    if (nd.HasValue)
                    {
                        var d = nd.Value.AddHours(sh).AddMinutes(sm);
                        if (d >= start && d <= end) dates.Add(d);
                    }
                    month = month.AddMonths(1);
                }
                break;

            default:
                throw new ValidationException($"Tipo de repetição inválido: {rep.Type}");
        }

        return dates;
    }

    private static DateTime? GetNthWeekday(int year, int month, int week, DayOfWeek dow)
    {
        if (week == 5)
        {
            var last = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            while (last.DayOfWeek != dow) last = last.AddDays(-1);
            return last;
        }
        var d = new DateTime(year, month, 1);
        while (d.DayOfWeek != dow) d = d.AddDays(1);
        d = d.AddDays((week - 1) * 7);
        return d.Month == month ? d : null;
    }

    // ── Status transitions ───────────────────────────────────
    private static void ValidateStatusTransition(SessionStatus current, SessionStatus next, Session session)
    {
        if (next == SessionStatus.Cancelled && session.InvoiceId.HasValue)
            throw new ValidationException("Sessão não pode ser cancelada pois já possui nota fiscal emitida.");
    }

    // ── Helpers ──────────────────────────────────────────────
    private void EnsureOwnership(Session session)
    {
        if (session.UserId != currentUser.UserId)
            throw new ForbiddenException("Acesso negada a esta sessão.");
    }

    private static SessionResponseDto MapToResponse(Session s) => new()
    {
        Id = s.Id,
        ClientId = s.ClientId,
        ClientName = s.Client?.Name ?? string.Empty,
        DateTime = s.DateTime,
        Duration = s.Duration,
        Status = s.Status,
        PricePerSession = s.PricePerSession,
        Invoice = s.Invoice is null ? null : new InvoiceSummaryDto
        {
            Id = s.Invoice.Id,
            Status = s.Invoice.Status,
            InvoiceNumber = s.Invoice.InvoiceNumber,
            PdfUrl = s.Invoice.PdfUrl,
            IssuedAt = s.Invoice.IssuedAt
        },
        RepetitionGroupId = s.RepetitionGroupId,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}