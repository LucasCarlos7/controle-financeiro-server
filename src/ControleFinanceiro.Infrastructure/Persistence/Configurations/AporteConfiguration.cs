using ControleFinanceiro.Domain.Objetivos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class AporteConfiguration : IEntityTypeConfiguration<Aporte>
{
    public void Configure(EntityTypeBuilder<Aporte> builder)
    {
        builder.ToTable("aportes");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.Property(a => a.ObjetivoId).HasColumnName("objetivo_id").IsRequired();

        builder.Property(a => a.Valor)
            .HasColumnName("valor")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(a => a.Data).HasColumnName("data").IsRequired();

        builder.HasOne<Objetivo>()
            .WithMany()
            .HasForeignKey(a => a.ObjetivoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.ObjetivoId);

        builder.Ignore(a => a.DomainEvents);
    }
}
