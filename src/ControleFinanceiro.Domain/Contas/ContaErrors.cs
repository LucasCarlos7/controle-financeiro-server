using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Contas;

public static class ContaErrors
{
    public static readonly Error NomeObrigatorio =
        new("Conta.NomeObrigatorio", "O nome da conta é obrigatório.");

    public static readonly Error Inativa =
        new("Conta.Inativa", "A conta está inativa e não aceita novos lançamentos.");

    public static readonly Error NaoEncontrada =
        new("Conta.NaoEncontrada", "Conta não encontrada.");
}
