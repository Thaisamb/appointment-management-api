using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppointmentManagement.Domain.Entities;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasColumnName("Street");
            address.Property(a => a.District).HasColumnName("District");
            address.Property(a => a.City).HasColumnName("City");
            address.Property(a => a.State).HasColumnName("State");
            address.Property(a => a.ZipCode).HasColumnName("ZipCode");
        });
        builder.OwnsMany(c => c.EmergencyContacts, contact =>
        {
            contact.WithOwner().HasForeignKey("ClientId");

            contact.Property<int>("Id");
            contact.HasKey("Id");

            contact.Property(c => c.Name);
            contact.Property(c => c.Phone);
        });
    }

}