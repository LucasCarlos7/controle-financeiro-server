using ControleFinanceiro.Domain.Objetivos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class ObjetivoConfiguration : IEntityTypeConfiguration<Objetivo>
{
    public void Configure(EntityTypeBuilder<Objetivo> builder)
    {
        builder.ToTable("objetivos");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id");

        builder.Property(o => o.UserId).HasColumnName("user_id").IsRequired();

        builder.Property(o => o.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.ValorAlvo)
            .HasColumnName("valor_alvo")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(o => o.DataAlvo).HasColumnName("data_alvo").IsRequired();
        builder.Property(o => o.CriadoEm).HasColumnName("criado_em").IsRequired();

        builder.HasIndex(o => o.UserId);

        builder.Ignore(o => o.DomainEvents);
    }
}
