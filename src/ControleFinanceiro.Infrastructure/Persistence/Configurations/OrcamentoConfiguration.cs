using ControleFinanceiro.Domain.Orcamentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class OrcamentoConfiguration : IEntityTypeConfiguration<Orcamento>
{
    public void Configure(EntityTypeBuilder<Orcamento> builder)
    {
        builder.ToTable("orcamentos");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasColumnName("id");

        builder.Property(o => o.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(o => o.CategoriaId).HasColumnName("categoria_id").IsRequired();
        builder.Property(o => o.Ano).HasColumnName("ano").IsRequired();
        builder.Property(o => o.Mes).HasColumnName("mes").IsRequired();

        builder.Property(o => o.LimiteValor)
            .HasColumnName("limite_valor")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.HasOne<Domain.Categorias.Categoria>()
            .WithMany()
            .HasForeignKey(o => o.CategoriaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => new { o.UserId, o.CategoriaId, o.Ano, o.Mes }).IsUnique();

        builder.Ignore(o => o.DomainEvents);
    }
}
