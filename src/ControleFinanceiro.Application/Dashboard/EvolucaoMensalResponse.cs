namespace ControleFinanceiro.Application.Dashboard;

public sealed record EvolucaoMensalResponse
{
    public required int Ano { get; init; }
    public required int Mes { get; init; }

    // Realizados
    public required decimal ReceitasRealizadas { get; init; }
    public required decimal DespesasRealizadas { get; init; }
    public required decimal SaldoRealizado { get; init; }
    public required decimal SaldoAcumuladoRealizado { get; init; }

    // Previsão (realizados + pendentes)
    public required decimal ReceitasPrevistas { get; init; }
    public required decimal DespesasPrevistas { get; init; }
    public required decimal SaldoPrevisto { get; init; }
    public required decimal SaldoAcumuladoPrevisto { get; init; }
}
