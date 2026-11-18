namespace ControleFinanceiro.Api.Contracts.Contas;

public sealed record CreateCartaoCreditoRequest
{
    public required string Nome { get; init; }
    public required decimal Limite { get; init; }
    public required int DiaFechamento { get; init; }
    public required int DiaVencimento { get; init; }
    public Guid? ContaPagamentoId { get; init; }
    public string? Icone { get; init; }
}
