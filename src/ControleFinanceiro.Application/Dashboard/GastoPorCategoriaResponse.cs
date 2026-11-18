namespace ControleFinanceiro.Application.Dashboard;

public sealed record GastoPorCategoriaResponse
{
    public Guid? CategoriaId { get; init; }
    public required string CategoriaNome { get; init; }
    public required decimal Valor { get; init; }
}
