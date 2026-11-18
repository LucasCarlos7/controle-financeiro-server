namespace ControleFinanceiro.Application.Dashboard;

public sealed record ResumoMensalResponse
{
    public required int Ano { get; init; }
    public required int Mes { get; init; }
    public required decimal Receitas { get; init; }
    public required decimal Despesas { get; init; }
    public required decimal Saldo { get; init; }
}
