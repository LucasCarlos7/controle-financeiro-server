namespace ControleFinanceiro.Application.Tags;

public sealed record TagResponse
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public string? Icone { get; init; }
}
