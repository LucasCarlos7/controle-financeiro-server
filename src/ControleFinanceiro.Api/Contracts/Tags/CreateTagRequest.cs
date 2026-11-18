namespace ControleFinanceiro.Api.Contracts.Tags;

public sealed record CreateTagRequest
{
    public required string Nome { get; init; }
    public string? Icone { get; init; }
}
