using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Contas;
using ControleFinanceiro.Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Transacoes;

internal sealed record TransacaoReferencias
{
    public Guid? ContaId { get; init; }
    public Guid? ContaDestinoId { get; init; }
    public Guid? CartaoCreditoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public IReadOnlyCollection<Guid> TagIds { get; init; } = [];
}

/// <summary>
/// Garante que toda referência de uma transação existe e pertence ao usuário. Registros de outro usuário
/// são tratados como inexistentes, para não revelar que o id existe.
/// </summary>
internal static class TransacaoReferenciaValidator
{
    public static async Task<Result> ValidarAsync(
        IApplicationDbContext dbContext,
        Guid userId,
        TransacaoReferencias referencias,
        CancellationToken cancellationToken)
    {
        var origem = await ValidarContasECartaoAsync(dbContext, userId, referencias, cancellationToken);
        if (origem.IsFailure)
            return origem;

        var categoria = await ValidarCategoriaAsync(dbContext, userId, referencias, cancellationToken);
        if (categoria.IsFailure)
            return categoria;

        return await ValidarTagsAsync(dbContext, userId, referencias.TagIds, cancellationToken);
    }

    // Só é chamado na criação: lançamentos já existentes em conta ou cartão desativados continuam editáveis.
    private static async Task<Result> ValidarContasECartaoAsync(
        IApplicationDbContext dbContext, Guid userId, TransacaoReferencias referencias, CancellationToken cancellationToken)
    {
        foreach (var contaId in new[] { referencias.ContaId, referencias.ContaDestinoId })
        {
            if (contaId is not { } id)
                continue;

            var ativa = await dbContext.Contas
                .Where(c => c.Id == id && c.UserId == userId)
                .Select(c => (bool?)c.Ativo)
                .FirstOrDefaultAsync(cancellationToken);

            if (ativa is null)
                return Result.Failure(ContaErrors.NaoEncontrada);

            if (ativa == false)
                return Result.Failure(ContaErrors.Inativa);
        }

        if (referencias.CartaoCreditoId is { } cartaoId)
        {
            var ativo = await dbContext.CartoesCredito
                .Where(c => c.Id == cartaoId && c.UserId == userId)
                .Select(c => (bool?)c.Ativo)
                .FirstOrDefaultAsync(cancellationToken);

            if (ativo is null)
                return Result.Failure(CartaoCreditoErrors.NaoEncontrado);

            if (ativo == false)
                return Result.Failure(CartaoCreditoErrors.Inativo);
        }

        return Result.Success();
    }

    private static async Task<Result> ValidarCategoriaAsync(
        IApplicationDbContext dbContext, Guid userId, TransacaoReferencias referencias, CancellationToken cancellationToken)
    {
        if (referencias.CategoriaId is { } catId
            && !await dbContext.Categorias.AnyAsync(c => c.Id == catId && c.UserId == userId, cancellationToken))
            return Result.Failure(CategoriaErrors.NaoEncontrada);

        // A categoria já foi validada como do usuário, então basta a subcategoria pertencer a ela.
        if (referencias.SubcategoriaId is { } subId
            && !await dbContext.Subcategorias.AnyAsync(
                s => s.Id == subId && s.CategoriaId == referencias.CategoriaId, cancellationToken))
            return Result.Failure(CategoriaErrors.SubcategoriaNaoPertenceACategoria);

        return Result.Success();
    }

    private static async Task<Result> ValidarTagsAsync(
        IApplicationDbContext dbContext, Guid userId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken)
    {
        if (tagIds.Count == 0)
            return Result.Success();

        var distintas = tagIds.Distinct().ToList();
        var tagsDoUsuario = await dbContext.Tags
            .CountAsync(t => distintas.Contains(t.Id) && t.UserId == userId, cancellationToken);

        return tagsDoUsuario == distintas.Count ? Result.Success() : Result.Failure(TagErrors.NaoEncontrada);
    }
}
