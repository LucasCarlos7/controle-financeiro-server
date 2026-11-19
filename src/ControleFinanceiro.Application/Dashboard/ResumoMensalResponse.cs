namespace ControleFinanceiro.Application.Dashboard;

public sealed record ResumoMensalResponse
{
    public required int Ano { get; init; }
    public required int Mes { get; init; }

    // Realizados
    public required decimal ReceitasRecebidas { get; init; }
    public required decimal DespesasPagas { get; init; }
    public required decimal SaldoRealizado { get; init; }

    // Pendentes (Pendente)
    public required decimal ReceitasPendentes { get; init; }
    public required decimal DespesasPendentes { get; init; }

    // Previsão = realizados + pendentes
    public required decimal ReceitasPrevistas { get; init; }
    public required decimal DespesasPrevistas { get; init; }
    public required decimal SaldoPrevisto { get; init; }

    // Totais gerais (realizados + pendentes)
    public required int QtdReceitasPendentes { get; init; }
    public required int QtdDespesasPendentes { get; init; }
}
