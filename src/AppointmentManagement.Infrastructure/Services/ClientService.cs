using AppointmentManagement.Application.DTOs.Client;
using AppointmentManagement.Application.DTOs.ValueObjects;
using AppointmentManagement.Application.Exceptions;
using AppointmentManagement.Application.Interfaces;
using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Application.Interfaces.Services;
using AppointmentManagement.Application.Constants;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.ValueObjects;

namespace AppointmentManagement.Infrastructure.Services;

public class ClientService(
    IClientRepository clientRepository,
    ICurrentUserService currentUser,
    IUnitOfWork uow) : IClientService
{
    public async Task<IEnumerable<ClientResponseDto>> GetAllAsync()
    {
        var clients = await clientRepository.GetAllByUserAsync(currentUser.UserId);
        return clients.Select(ClientResponseDto.FromEntity);
    }

    public async Task<ClientResponseDto?> GetByIdAsync(int id)
    {
        var client = await clientRepository.GetByIdWithDetailsAsync(id);

        if (client is null) return null;
        EnsureOwnership(client);

        return ClientResponseDto.FromEntity(client);
    }

    public async Task<ClientResponseDto> CreateAsync(CreateClientDto dto)
    {
        // 1. VALIDAÇÃO DE NEGÓCIO (Clean Code)
        var CPFExists = await clientRepository.ExistsByCpfAsync(dto.CPF, currentUser.UserId);

        if (CPFExists)
            throw new ValidationException(ValidationMessages.CpfJaExiste);

        // 2. Instancia o objeto principal e os filhos em cascata
        var client = new Client
        {
            Name = dto.Name,
            Phone = dto.Phone,
            CPF = dto.CPF, 
            PricePerSession = dto.PricePerSession,
            UserId = currentUser.UserId,
            CreateAt = DateTime.Now, 
            BirthDate = dto.BirthDate,

            FinancialResponsible = new FinancialResponsible
            {
                Name = dto.FinancialResponsibleName,
                CPF = dto.FinancialResponsibleCPF.Replace(".", "").Replace("-", ""),
                Phone = dto.FinancialResponsiblePhone
            },

            InvoiceIssuer = new InvoiceIssuer
            {
                Name = dto.ClientAsIssuer ? dto.Name : dto.FinancialResponsibleName,
                CPF = dto.ClientAsIssuer ? dto.CPF : dto.FinancialResponsibleCPF.Replace(".", "").Replace("-", ""),
                Email = dto.Email,
                Address = new Address(
                    dto.Address.Street,
                    dto.Address.District,
                    dto.Address.City,
                    dto.Address.State,
                    dto.Address.ZipCode,
                    dto.Address.Number,
                    dto.Address.Complement
                )
            },

            EmergencyContacts = dto.EmergencyContacts?
                .Select(em => new EmergencyContact(em.Name, em.Phone, em.Relationship))
                .ToList() ?? new List<EmergencyContact>()
        };

        await clientRepository.AddAsync(client);
        await uow.CommitAsync();

        return ClientResponseDto.FromEntity(client);
    }

    public async Task<ClientResponseDto?> UpdateAsync(int id, UpdateClientDto dto)
    {
        var client = await clientRepository.GetByIdWithDetailsAsync(id);

        if (client is null) return null;
        EnsureOwnership(client);

        // 1. VALIDAÇÃO DE NEGÓCIO NO UPDATE (Clean Code)

        // Só valida se o usuário estiver tentando ALTERAR o CPF atual do cliente
        if (client.CPF != dto.CPF)
        {
            var CPFExists = await clientRepository.ExistsByCpfAsync(dto.CPF, currentUser.UserId);
            if (CPFExists)
                throw new ValidationException("CPF já está sendo utilizado por outro cliente.");
        }

        // 2. Atualiza dados do Cliente
        client.Name = dto.Name;
        client.Phone = dto.Phone;
        client.CPF = dto.CPF;
        client.PricePerSession = dto.PricePerSession;
        client.BirthDate = dto.BirthDate;
        client.UpdateAt = DateTime.Now; // Registra a data de alteração local

        // 3. Atualiza dados do Responsável Financeiro
        client.FinancialResponsible.Name = dto.FinancialResponsibleName;
        client.FinancialResponsible.CPF = dto.FinancialResponsibleCPF.Replace(".", "").Replace("-", "");
        client.FinancialResponsible.Phone = dto.FinancialResponsiblePhone;

        // 4. Atualiza dados do Emissor/Tomador da Nota
        client.InvoiceIssuer.Email = dto.Email;
        client.InvoiceIssuer.Name = dto.ClientAsIssuer ? dto.Name : dto.FinancialResponsibleName;
        client.InvoiceIssuer.CPF = dto.ClientAsIssuer ? dto.CPF : dto.FinancialResponsibleCPF.Replace(".", "").Replace("-", "");

        client.InvoiceIssuer.Address = dto.Address is null ? null : new Address(
            dto.Address.Street,
            dto.Address.District,
            dto.Address.City,
            dto.Address.State,
            dto.Address.ZipCode,
            dto.Address.Number,
            dto.Address.Complement
        );

        // 5. Atualiza os contatos de emergência usando o record e o método .Clear()
        client.EmergencyContacts.Clear();
        if (dto.EmergencyContacts is not null)
        {
            foreach (var ec in dto.EmergencyContacts)
            {
                client.EmergencyContacts.Add(new EmergencyContact(ec.Name, ec.Phone, ec.Relationship));
            }
        }

        await uow.CommitAsync();

        return ClientResponseDto.FromEntity(client);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var client = await clientRepository.GetByIdAsync(id);

        if (client is null) return false;
        EnsureOwnership(client);

        clientRepository.Remove(client);
        await uow.CommitAsync();

        return true;
    }

    // ── Helpers ──────────────────────────────────────────────
    private void EnsureOwnership(Client client)
    {
        if (client.UserId != currentUser.UserId)
            throw new ForbiddenException("Acesso negado a este cliente.");
    }

    //private static ClientResponseDto ClientResponseDto.FromEntity(Client c) => new()
    //{
    //    Id = c.Id,
    //    Name = c.Name,
    //    // Busca o Email do Emissor da Nota, já que no Cliente não tem mais
    //    Email = c.InvoiceIssuer?.Email ?? string.Empty,
    //    Phone = c.Phone,
    //    CPF = c.CPF,
    //    PricePerSession = c.PricePerSession,
    //    BirthDate = c.BirthDate,
    //    Address = c.InvoiceIssuer?.Address is null ? null : new AddressDto
    //    {
    //        Street = c.InvoiceIssuer.Address.Street,
    //        District = c.InvoiceIssuer.Address.District,
    //        City = c.InvoiceIssuer.Address.City,
    //        State = c.InvoiceIssuer.Address.State,
    //        ZipCode = c.InvoiceIssuer.Address.ZipCode,
    //        Number = c.InvoiceIssuer.Address.Number
    //    },
    //    EmergencyContacts = c.EmergencyContacts?
    //    .Select(ec => new EmergencyContactDto
    //    {
    //        Name = ec.Name,
    //        Phone = ec.Phone,
    //        Relationship = ec.Relationship
    //    }).ToList() ?? []
    //};
}