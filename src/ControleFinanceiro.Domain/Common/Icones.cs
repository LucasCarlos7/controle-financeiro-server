namespace ControleFinanceiro.Domain.Common;

/// <summary>Ícone opcional escolhido pelo usuário (nome de um ícone do front-end, ex.: "wallet").</summary>
public static class Icones
{
    public const int TamanhoMaximo = 50;

    /// <summary>Remove espaços nas pontas; vazio vira null.</summary>
    public static string? Normalizar(string? icone) => string.IsNullOrWhiteSpace(icone) ? null : icone.Trim();
}
