using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Objetivos;

public static class ObjetivoErrors
{
    public static readonly Error NomeObrigatorio =
        new("Objetivo.NomeObrigatorio", "O nome do objetivo é obrigatório.");

    public static readonly Error ValorAlvoInvalido =
        new("Objetivo.ValorAlvoInvalido", "O valor alvo deve ser maior que zero.");

    public static readonly Error ValorAporteInvalido =
        new("Objetivo.ValorAporteInvalido", "O valor do aporte deve ser maior que zero.");

    public static readonly Error NaoEncontrado =
        new("Objetivo.NaoEncontrado", "Objetivo não encontrado.");

    public static readonly Error AporteNaoEncontrado =
        new("Objetivo.AporteNaoEncontrado", "Aporte não encontrado.");
}
