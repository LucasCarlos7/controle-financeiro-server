using ControleFinanceiro.Domain.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class SubcategoriaConfiguration : IEntityTypeConfiguration<Subcategoria>
{
    public void Configure(EntityTypeBuilder<Subcategoria> builder)
    {
        builder.ToTable("subcategorias");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.CategoriaId)
            .HasColumnName("categoria_id")
            .IsRequired();

        builder.Property(s => s.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();
    }
}
