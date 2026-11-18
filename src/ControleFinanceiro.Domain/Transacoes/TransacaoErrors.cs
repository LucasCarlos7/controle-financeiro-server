using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Transacoes;

public static class TransacaoErrors
{
    public static readonly Error ValorInvalido =
        new("Transacao.ValorInvalido", "O valor da transação deve ser maior que zero.");

    public static readonly Error DescricaoObrigatoria =
        new("Transacao.DescricaoObrigatoria", "A descrição da transação é obrigatória.");

    public static readonly Error OrigemInvalida =
        new("Transacao.OrigemInvalida", "A transação deve referenciar exatamente uma conta ou um cartão de crédito.");

    public static readonly Error TransferenciaNaoPermiteCartao =
        new("Transacao.TransferenciaNaoPermiteCartao", "Transferências não podem ser originadas de um cartão de crédito.");

    public static readonly Error ContaDestinoObrigatoria =
        new("Transacao.ContaDestinoObrigatoria", "Transferências exigem uma conta de destino.");

    public static readonly Error ContaDestinoIgualOrigem =
        new("Transacao.ContaDestinoIgualOrigem", "A conta de destino deve ser diferente da conta de origem.");

    public static readonly Error ContaDestinoNaoPermitida =
        new("Transacao.ContaDestinoNaoPermitida", "Somente transferências podem ter conta de destino.");

    public static readonly Error NaoEncontrada =
        new("Transacao.NaoEncontrada", "Transação não encontrada.");
}
