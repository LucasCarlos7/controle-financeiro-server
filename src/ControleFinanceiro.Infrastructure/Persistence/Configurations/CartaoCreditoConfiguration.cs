using ControleFinanceiro.Domain.Contas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

internal sealed class CartaoCreditoConfiguration : IEntityTypeConfiguration<CartaoCredito>
{
    public void Configure(EntityTypeBuilder<CartaoCredito> builder)
    {
        builder.ToTable("cartoes_credito");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(c => c.Nome)
            .HasColumnName("nome")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Icone)
            .HasColumnName("icone")
            .HasMaxLength(50);

        builder.Property(c => c.Limite)
            .HasColumnName("limite")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(c => c.DiaFechamento)
            .HasColumnName("dia_fechamento")
            .IsRequired();

        builder.Property(c => c.DiaVencimento)
            .HasColumnName("dia_vencimento")
            .IsRequired();

        builder.Property(c => c.ContaPagamentoId)
            .HasColumnName("conta_pagamento_id");

        builder.Property(c => c.Ativo)
            .HasColumnName("ativo")
            .IsRequired();

        builder.HasOne<Conta>()
            .WithMany()
            .HasForeignKey(c => c.ContaPagamentoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.UserId);

        builder.Ignore(c => c.DomainEvents);
    }
}
