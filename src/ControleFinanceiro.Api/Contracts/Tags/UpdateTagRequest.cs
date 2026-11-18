namespace ControleFinanceiro.Api.Contracts.Tags;

public sealed record UpdateTagRequest
{
    public required string Nome { get; init; }
    public string? Icone { get; init; }
}
