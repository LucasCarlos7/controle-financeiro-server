using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Contas;
using ControleFinanceiro.Domain.Orcamentos;
using ControleFinanceiro.Domain.Objetivos;
using ControleFinanceiro.Domain.Tags;
using ControleFinanceiro.Domain.Transacoes;
using ControleFinanceiro.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<Conta> Contas { get; }
    DbSet<CartaoCredito> CartoesCredito { get; }
    DbSet<Categoria> Categorias { get; }
    DbSet<Subcategoria> Subcategorias { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Recorrencia> Recorrencias { get; }
    DbSet<Transacao> Transacoes { get; }
    DbSet<Orcamento> Orcamentos { get; }
    DbSet<Objetivo> Objetivos { get; }
    DbSet<Aporte> Aportes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
