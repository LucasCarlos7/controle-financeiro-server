using ControleFinanceiro.Domain.Transacoes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.ToTable("transacoes");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(t => t.Tipo)
            .HasColumnName("tipo")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Valor)
            .HasColumnName("valor")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(t => t.Data)
            .HasColumnName("data")
            .IsRequired();

        builder.Property(t => t.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.ContaId).HasColumnName("conta_id");
        builder.Property(t => t.CartaoCreditoId).HasColumnName("cartao_credito_id");
        builder.Property(t => t.ContaDestinoId).HasColumnName("conta_destino_id");
        builder.Property(t => t.CategoriaId).HasColumnName("categoria_id");
        builder.Property(t => t.SubcategoriaId).HasColumnName("subcategoria_id");
        builder.Property(t => t.RecorrenciaId).HasColumnName("recorrencia_id");

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Lista de tags mapeada como array nativo do PostgreSQL (uuid[]), em vez de tabela
        // de junção, já que o agregado Transacao só precisa dos ids para suas invariantes.
        // O campo privado (List<Guid>) é mapeado diretamente; a propriedade pública somente
        // leitura (IReadOnlyList<Guid>) não é um tipo suportado para primitive collections.
        builder.Ignore(t => t.TagIds);
        builder.PrimitiveCollection<List<Guid>>("_tagIds")
            .HasColumnName("tag_ids")
            .Metadata.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Domain.Contas.Conta>()
            .WithMany()
            .HasForeignKey(t => t.ContaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Contas.Conta>()
            .WithMany()
            .HasForeignKey(t => t.ContaDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Contas.CartaoCredito>()
            .WithMany()
            .HasForeignKey(t => t.CartaoCreditoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Categorias.Categoria>()
            .WithMany()
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Domain.Categorias.Subcategoria>()
            .WithMany()
            .HasForeignKey(t => t.SubcategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Recorrencia>()
            .WithMany()
            .HasForeignKey(t => t.RecorrenciaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => new { t.UserId, t.Data });
        builder.HasIndex(t => t.ContaId);
        builder.HasIndex(t => t.CartaoCreditoId);

        builder.Ignore(t => t.DomainEvents);
    }
}
