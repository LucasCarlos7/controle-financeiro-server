using System.Reflection;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Contas;
using ControleFinanceiro.Domain.Objetivos;
using ControleFinanceiro.Domain.Orcamentos;
using ControleFinanceiro.Domain.Tags;
using ControleFinanceiro.Domain.Transacoes;
using ControleFinanceiro.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<CartaoCredito> CartoesCredito => Set<CartaoCredito>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Subcategoria> Subcategorias => Set<Subcategoria>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Recorrencia> Recorrencias => Set<Recorrencia>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
    public DbSet<Objetivo> Objetivos => Set<Objetivo>();
    public DbSet<Aporte> Aportes => Set<Aporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
