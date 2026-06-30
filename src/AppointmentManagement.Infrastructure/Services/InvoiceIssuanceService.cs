using AppointmentManagement.Application.DTOs.Invoice;
using AppointmentManagement.Application.Interfaces;
using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Application.Interfaces.Services;
using AppointmentManagement.Domain.Entities;
using AppointmentManagement.Domain.Enums;
using Microsoft.AspNetCore.Hosting;

namespace AppointmentManagement.Infrastructure.Services;

public class InvoiceIssuanceService(
    IInvoiceIssuanceRepository repository,
    ICurrentUserService currentUser,
    IUnitOfWork uow,
    IEncryptionService encryption,
    IWebHostEnvironment env) : IInvoiceIssuanceService
{
    public async Task<InvoiceIssuanceDto?> GetAsync()
    {
        var config = await repository.GetByUserAsync(currentUser.UserId);
        if (config is null) return null;

        return new InvoiceIssuanceDto
        {
            IssuerType           = config.IssuerType,
            GovBrLogin           = config.GovBrLogin,
            HasGovBrPassword     = !string.IsNullOrEmpty(config.GovBrPasswordEncrypted),
            CNPJ                 = config.CNPJ,
            IsMei                = config.IsMei,
            HasCertificate       = !string.IsNullOrEmpty(config.CertificatePath),
            CertificateExpiry    = config.CertificateExpiry,
            MunicipalRegistration = config.MunicipalRegistration,
            ServiceCode          = config.ServiceCode
        };
    }

    public async Task SaveAsync(SaveInvoiceIssuanceDto dto)
    {
        var config = await repository.GetByUserAsync(currentUser.UserId)
            ?? new InvoiceIssuance { UserId = currentUser.UserId };

        config.IssuerType = dto.IssuerType;
        config.MunicipalRegistration = dto.MunicipalRegistration;
        config.ServiceCode = dto.ServiceCode;

        if (dto.IssuerType == TypeInvoiceIssuer.CPF)
        {
            config.GovBrLogin = dto.GovBrLogin;
            if (!string.IsNullOrEmpty(dto.GovBrPassword))
                config.GovBrPasswordEncrypted = encryption.Encrypt(dto.GovBrPassword);
        }
        else
        {
            config.CNPJ  = dto.CNPJ;
            config.IsMei = dto.IsMei;
        }

        config.UpdatedAt = DateTime.UtcNow;

        if (config.Id == 0)
            await repository.AddAsync(config);

        await uow.CommitAsync();
    }

    public async Task UploadCertificateAsync(UploadCertificateDto dto)
    {
        var config = await repository.GetByUserAsync(currentUser.UserId)
            ?? new InvoiceIssuance { UserId = currentUser.UserId };

        // Salva fora do wwwroot
        var certsDir = Path.Combine(env.ContentRootPath, "SecureCerts", currentUser.UserId.ToString());
        Directory.CreateDirectory(certsDir);
        var filePath = Path.Combine(certsDir, "certificate.pfx");

        await using var stream = File.Create(filePath);
        await dto.Certificate.CopyToAsync(stream);

        config.CertificatePath             = filePath;
        config.CertificatePasswordEncrypted = encryption.Encrypt(dto.Password);
        config.CertificateExpiry           = dto.Expiry;
        config.UpdatedAt                   = DateTime.UtcNow;

        if (config.Id == 0)
            await repository.AddAsync(config);

        await uow.CommitAsync();
    }
}