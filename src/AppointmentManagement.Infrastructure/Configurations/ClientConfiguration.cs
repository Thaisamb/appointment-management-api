using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AppointmentManagement.Domain.Entities;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // 1. Chave Primária
        builder.HasKey(c => c.Id);

        // 2. Configurações dos campos Obrigatórios (Required)
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Phone).IsRequired().HasMaxLength(20);
        builder.Property(c => c.CPF).IsRequired().HasMaxLength(11);
        builder.Property(c => c.PricePerSession).HasColumnType("decimal(18,2)");

        // 3. Relacionamento com as novas tabelas (1 para 1)
        builder.HasOne(c => c.FinancialResponsible)
            .WithMany() // Ajuste se houver propriedade inversa na outra entidade
            .HasForeignKey(c => c.FinancialResponsibleId)
            .OnDelete(DeleteBehavior.Restrict); // Evita deletar o financeiro por engano

        builder.HasOne(c => c.InvoiceIssuer)
            .WithMany()
            .HasForeignKey(c => c.InvoiceIssuerId)
            .OnDelete(DeleteBehavior.Restrict);

        // 4. Contatos de Emergência (Configuração de Deleção em Cascata)
        // Substituído o 'OwnsMany' para dar suporte ao '.Clear()' e '.Add()' sem erros
        // Substitua o bloco antigo de EmergencyContacts por este no ClientConfiguration.cs
        builder.OwnsMany(c => c.EmergencyContacts, contact =>
        {
            // Vincula a tabela ao cliente usando uma FK implícita
            contact.WithOwner().HasForeignKey("ClientId");

            // Cria uma chave primária oculta (Shadow Key) para o banco de dados gerenciar
            contact.Property<int>("Id");
            contact.HasKey("Id");

            // Mapeia as propriedades do seu record
            contact.Property(c => c.Name).HasMaxLength(150);
            contact.Property(c => c.Relationship).HasMaxLength(50);
            contact.Property(c => c.Phone).HasMaxLength(20);
        });


        // Habilita o rastreamento correto da lista interna
        builder.Navigation(c => c.EmergencyContacts)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}
