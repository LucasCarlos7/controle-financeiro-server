using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Transacoes.Commands.UpdateTransacao;

public sealed class UpdateTransacaoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateTransacaoCommand, Result>
{
    public async Task<Result> Handle(UpdateTransacaoCommand request, CancellationToken cancellationToken)
    {
        var transacao = await dbContext.Transacoes.FindAsync([request.Id], cancellationToken);
        if (transacao is null || transacao.UserId != request.UserId)
            return Result.Failure(TransacaoErrors.NaoEncontrada);

        var referenciasValidas = await TransacaoReferenciaValidator.ValidarAsync(
            dbContext,
            request.UserId,
            new TransacaoReferencias
            {
                CategoriaId = request.CategoriaId,
                SubcategoriaId = request.SubcategoriaId,
                TagIds = request.TagIds,
            },
            cancellationToken);

        if (referenciasValidas.IsFailure)
            return referenciasValidas;

        // Sem recorrência ou escopo restrito a esta: atualiza somente ela.
        if (transacao.RecorrenciaId is null || request.EscopoEdicao == EscopoEdicaoRecorrencia.SomenteEsta)
            return await AtualizarUma(transacao, request, cancellationToken);

        return await AtualizarEmLote(transacao, request, cancellationToken);
    }

    private async Task<Result> AtualizarUma(
        Transacao transacao, UpdateTransacaoCommand request, CancellationToken cancellationToken)
    {
        var result = transacao.AtualizarDetalhes(
            request.Valor, request.Data, request.Descricao, request.CategoriaId, request.SubcategoriaId);

        if (result.IsFailure)
            return result;

        SincronizarTags(transacao, request.TagIds);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> AtualizarEmLote(
        Transacao transacao, UpdateTransacaoCommand request, CancellationToken cancellationToken)
    {
        var query = dbContext.Transacoes
            .Where(t => t.RecorrenciaId == transacao.RecorrenciaId && t.UserId == request.UserId);

        if (request.EscopoEdicao == EscopoEdicaoRecorrencia.EstaEPosteriores)
            query = query.Where(t => t.Data >= transacao.Data);

        var transacoes = await query.ToListAsync(cancellationToken);

        // A diferença de dias entre a data original e a nova é propagada a todas as parcelas,
        // mantendo o dia-do-mês relativo de cada uma (ex.: conta no dia 5 de cada mês).
        var deltaDias = request.Data.DayNumber - transacao.Data.DayNumber;

        foreach (var t in transacoes)
        {
            var novaData = t.Data.AddDays(deltaDias);
            var result = t.AtualizarDetalhes(request.Valor, novaData, request.Descricao, request.CategoriaId, request.SubcategoriaId);
            if (result.IsFailure)
                return result;

            SincronizarTags(t, request.TagIds);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static void SincronizarTags(Transacao transacao, IReadOnlyList<Guid> novasTags)
    {
        foreach (var tagId in transacao.TagIds.ToList())
        {
            if (!novasTags.Contains(tagId))
                transacao.RemoverTag(tagId);
        }

        foreach (var tagId in novasTags)
            transacao.AdicionarTag(tagId);
    }
}
