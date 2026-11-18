using ControleFinanceiro.Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(t => t.Nome)
            .HasColumnName("nome")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Icone)
            .HasColumnName("icone")
            .HasMaxLength(50);

        builder.HasIndex(t => new { t.UserId, t.Nome }).IsUnique();
    }
}
