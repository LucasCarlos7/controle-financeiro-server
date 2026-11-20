namespace ControleFinanceiro.Api.Contracts.Transacoes;

public sealed record MarkTransacaoAsPagoRequest
{
    public DateOnly? DataPagamento { get; init; }
}
