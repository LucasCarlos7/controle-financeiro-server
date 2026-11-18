using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Orcamentos;

public static class OrcamentoErrors
{
    public static readonly Error MesInvalido =
        new("Orcamento.MesInvalido", "O mês deve estar entre 1 e 12.");

    public static readonly Error AnoInvalido =
        new("Orcamento.AnoInvalido", "Ano inválido.");

    public static readonly Error LimiteInvalido =
        new("Orcamento.LimiteInvalido", "O limite do orçamento deve ser maior que zero.");

    public static readonly Error NaoEncontrado =
        new("Orcamento.NaoEncontrado", "Orçamento não encontrado.");

    public static readonly Error JaExiste =
        new("Orcamento.JaExiste", "Já existe um orçamento para esta categoria neste mês.");
}
