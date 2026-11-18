using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Contas;

public static class CartaoCreditoErrors
{
    public static readonly Error NomeObrigatorio =
        new("CartaoCredito.NomeObrigatorio", "O nome do cartão é obrigatório.");

    public static readonly Error LimiteInvalido =
        new("CartaoCredito.LimiteInvalido", "O limite do cartão deve ser maior que zero.");

    public static readonly Error DiaFechamentoInvalido =
        new("CartaoCredito.DiaFechamentoInvalido", "O dia de fechamento deve estar entre 1 e 31.");

    public static readonly Error DiaVencimentoInvalido =
        new("CartaoCredito.DiaVencimentoInvalido", "O dia de vencimento deve estar entre 1 e 31.");

    public static readonly Error NaoEncontrado =
        new("CartaoCredito.NaoEncontrado", "Cartão de crédito não encontrado.");

    public static readonly Error Inativo =
        new("CartaoCredito.Inativo", "O cartão de crédito está inativo e não aceita novos lançamentos.");

    public static readonly Error ContaPagamentoNaoEncontrada =
        new("CartaoCredito.ContaPagamentoNaoEncontrada", "Conta de pagamento informada não foi encontrada.");
}
