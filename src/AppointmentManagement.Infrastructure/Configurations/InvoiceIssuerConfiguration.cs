using AppointmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppointmentManagement.Infrastructure.Data.Configurations;

public class InvoiceIssuerConfiguration : IEntityTypeConfiguration<InvoiceIssuer>
{
    public void Configure(EntityTypeBuilder<InvoiceIssuer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Address, addr =>
        {
            addr.Property(a => a.Street).HasColumnName("Street");
            addr.Property(a => a.Number).HasColumnName("Number");
            addr.Property(a => a.Complement).HasColumnName("Complement");
            addr.Property(a => a.District).HasColumnName("District");
            addr.Property(a => a.City).HasColumnName("City");
            addr.Property(a => a.State).HasColumnName("State");
            addr.Property(a => a.ZipCode).HasColumnName("ZipCode");
        });
    }
}