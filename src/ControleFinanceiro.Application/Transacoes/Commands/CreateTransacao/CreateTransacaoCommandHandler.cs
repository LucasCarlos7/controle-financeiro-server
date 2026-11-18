using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.CreateTransacao;

public sealed class CreateTransacaoCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateTransacaoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTransacaoCommand request, CancellationToken cancellationToken)
    {
        var referenciasValidas = await TransacaoReferenciaValidator.ValidarAsync(
            dbContext,
            request.UserId,
            new TransacaoReferencias
            {
                ContaId = request.ContaId,
                ContaDestinoId = request.ContaDestinoId,
                CartaoCreditoId = request.CartaoCreditoId,
                CategoriaId = request.CategoriaId,
                SubcategoriaId = request.SubcategoriaId,
                TagIds = request.TagIds,
            },
            cancellationToken);

        if (referenciasValidas.IsFailure)
            return Result.Failure<Guid>(referenciasValidas.Error);

        var result = Transacao.Create(
            request.UserId,
            request.Tipo,
            request.Valor,
            request.Data,
            request.Descricao,
            request.ContaId,
            request.CartaoCreditoId,
            request.ContaDestinoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.Status,
            null);

        if (result.IsFailure)
            return Result.Failure<Guid>(result.Error);

        var transacao = result.Value;
        foreach (var tagId in request.TagIds)
            transacao.AdicionarTag(tagId);

        dbContext.Transacoes.Add(transacao);
        await dbContext.SaveChangesAsync(cancellationToken);

        return transacao.Id;
    }
}
