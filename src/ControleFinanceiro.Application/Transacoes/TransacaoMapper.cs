using ControleFinanceiro.Domain.Transacoes;

namespace ControleFinanceiro.Application.Transacoes;

internal static class TransacaoMapper
{
    public static TransacaoResponse ToResponse(Transacao transacao) => new()
    {
        Id = transacao.Id,
        Tipo = transacao.Tipo,
        Valor = transacao.Valor,
        Data = transacao.Data,
        Descricao = transacao.Descricao,
        ContaId = transacao.ContaId,
        CartaoCreditoId = transacao.CartaoCreditoId,
        ContaDestinoId = transacao.ContaDestinoId,
        CategoriaId = transacao.CategoriaId,
        SubcategoriaId = transacao.SubcategoriaId,
        Status = transacao.Status,
        DataPagamento = transacao.DataPagamento,
        RecorrenciaId = transacao.RecorrenciaId,
        TagIds = transacao.TagIds,
    };
}
