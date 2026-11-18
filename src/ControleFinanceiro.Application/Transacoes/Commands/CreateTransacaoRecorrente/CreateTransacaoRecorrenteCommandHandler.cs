using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;

namespace ControleFinanceiro.Application.Transacoes.Commands.CreateTransacaoRecorrente;

public sealed class CreateTransacaoRecorrenteCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateTransacaoRecorrenteCommand, Result<Guid>>
{
    private const int MaxOcorrenciasIndeterminadas = 24;

    public async Task<Result<Guid>> Handle(
        CreateTransacaoRecorrenteCommand request, CancellationToken cancellationToken)
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

        var recorrenciaResult = Recorrencia.Create(
            request.UserId, request.TipoRecorrencia, request.IntervaloMeses, request.TotalParcelas, null, request.DataFim);

        if (recorrenciaResult.IsFailure)
            return Result.Failure<Guid>(recorrenciaResult.Error);

        var recorrencia = recorrenciaResult.Value;
        var datas = GerarDatas(request);

        var transacoes = new List<Transacao>();
        for (var i = 0; i < datas.Count; i++)
        {
            var descricao = request.TipoRecorrencia == TipoRecorrencia.Parcelada
                ? $"{request.Descricao} ({i + 1}/{datas.Count})"
                : request.Descricao;

            var transacaoResult = Transacao.Create(
                request.UserId,
                request.Tipo,
                request.Valor,
                datas[i],
                descricao,
                request.ContaId,
                request.CartaoCreditoId,
                request.ContaDestinoId,
                request.CategoriaId,
                request.SubcategoriaId,
                StatusTransacao.Pendente,
                recorrencia.Id);

            if (transacaoResult.IsFailure)
                return Result.Failure<Guid>(transacaoResult.Error);

            var transacao = transacaoResult.Value;
            foreach (var tagId in request.TagIds)
                transacao.AdicionarTag(tagId);

            transacoes.Add(transacao);
        }

        dbContext.Recorrencias.Add(recorrencia);
        dbContext.Transacoes.AddRange(transacoes);
        await dbContext.SaveChangesAsync(cancellationToken);

        return recorrencia.Id;
    }

    private static List<DateOnly> GerarDatas(CreateTransacaoRecorrenteCommand request)
    {
        var intervalo = request.IntervaloMeses ?? 1;
        var datas = new List<DateOnly>();

        if (request.TipoRecorrencia == TipoRecorrencia.Parcelada)
        {
            var total = request.TotalParcelas!.Value;
            for (var i = 0; i < total; i++)
                datas.Add(request.DataInicio.AddMonths(i * intervalo));

            return datas;
        }

        // Sem data-fim, limitamos o horizonte de geração para não criar lançamentos indefinidamente.
        var data = request.DataInicio;
        while (datas.Count < MaxOcorrenciasIndeterminadas && (request.DataFim is null || data <= request.DataFim))
        {
            datas.Add(data);
            data = data.AddMonths(intervalo);
        }

        return datas;
    }
}
