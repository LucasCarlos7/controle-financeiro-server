using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Transacoes;

public static class RecorrenciaErrors
{
    public static readonly Error TotalParcelasObrigatorio =
        new("Recorrencia.TotalParcelasObrigatorio", "Recorrência parcelada exige o total de parcelas.");

    public static readonly Error ParcelaAtualInvalida =
        new("Recorrencia.ParcelaAtualInvalida", "A parcela atual deve estar entre 1 e o total de parcelas.");

    public static readonly Error NaoEncontrada =
        new("Recorrencia.NaoEncontrada", "Recorrência não encontrada.");
}
