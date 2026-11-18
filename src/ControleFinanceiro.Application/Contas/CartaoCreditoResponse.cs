namespace ControleFinanceiro.Application.Contas;

public sealed record CartaoCreditoResponse
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public required decimal Limite { get; init; }
    public required decimal LimiteDisponivel { get; init; }
    public required int DiaFechamento { get; init; }
    public required int DiaVencimento { get; init; }
    public required Guid? ContaPagamentoId { get; init; }
    public string? Icone { get; init; }
    public required bool Ativo { get; init; }
}
