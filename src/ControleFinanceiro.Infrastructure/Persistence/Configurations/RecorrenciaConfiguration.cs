using ControleFinanceiro.Domain.Transacoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class RecorrenciaConfiguration : IEntityTypeConfiguration<Recorrencia>
{
    public void Configure(EntityTypeBuilder<Recorrencia> builder)
    {
        builder.ToTable("recorrencias");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(r => r.Tipo)
            .HasColumnName("tipo")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.IntervaloMeses)
            .HasColumnName("intervalo_meses");

        builder.Property(r => r.TotalParcelas)
            .HasColumnName("total_parcelas");

        builder.Property(r => r.ParcelaAtual)
            .HasColumnName("parcela_atual");

        builder.Property(r => r.DataFim)
            .HasColumnName("data_fim");

        builder.Ignore(r => r.DomainEvents);
    }
}
