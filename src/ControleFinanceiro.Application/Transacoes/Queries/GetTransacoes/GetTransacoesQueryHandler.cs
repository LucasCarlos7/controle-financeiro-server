using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Transacoes.Queries.GetTransacoes;

public sealed class GetTransacoesQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetTransacoesQuery, Result<IReadOnlyList<TransacaoResponse>>>
{
    public async Task<Result<IReadOnlyList<TransacaoResponse>>> Handle(
        GetTransacoesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Transacoes.AsNoTracking().Where(t => t.UserId == request.UserId);

        if (request.DataInicio is { } dataInicio)
            query = query.Where(t => t.Data >= dataInicio);

        if (request.DataFim is { } dataFim)
            query = query.Where(t => t.Data <= dataFim);

        if (request.ContaId is { } contaId)
            query = query.Where(t => t.ContaId == contaId || t.ContaDestinoId == contaId);

        if (request.CartaoCreditoId is { } cartaoCreditoId)
            query = query.Where(t => t.CartaoCreditoId == cartaoCreditoId);

        if (request.CategoriaId is { } categoriaId)
            query = query.Where(t => t.CategoriaId == categoriaId);

        if (request.Tipo is { } tipo)
            query = query.Where(t => t.Tipo == tipo);

        if (request.Status is { } status)
            query = query.Where(t => t.Status == status);

        if (request.ApenasRecorrentes == true)
            query = query.Where(t => t.RecorrenciaId != null);

        if (!string.IsNullOrWhiteSpace(request.Busca))
            query = query.Where(t => t.Descricao.Contains(request.Busca));

        if (request.TagId is { } tagId)
            query = query.Where(t => EF.Property<List<Guid>>(t, "_tagIds").Contains(tagId));

        var transacoes = await query
            .OrderByDescending(t => t.Data)
            .ToListAsync(cancellationToken);

        IReadOnlyList<TransacaoResponse> response = transacoes.Select(TransacaoMapper.ToResponse).ToList();

        return Result.Success(response);
    }
}
