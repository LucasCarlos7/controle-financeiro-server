using ControleFinanceiro.Domain.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(c => c.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Tipo)
            .HasColumnName("tipo")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Icone)
            .HasColumnName("icone")
            .HasMaxLength(50);

        builder.Property(c => c.Cor)
            .HasColumnName("cor")
            .HasMaxLength(20);

        builder.Property(c => c.IsPadrao)
            .HasColumnName("is_padrao")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasMany(c => c.Subcategorias)
            .WithOne()
            .HasForeignKey(s => s.CategoriaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Categoria.Subcategorias))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(c => c.UserId);

        builder.Ignore(c => c.DomainEvents);
    }
}
