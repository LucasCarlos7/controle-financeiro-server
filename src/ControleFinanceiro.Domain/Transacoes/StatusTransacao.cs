namespace ControleFinanceiro.Domain.Transacoes;

public enum StatusTransacao
{
    Pendente = 1,
    Pago = 2,
    Recebido = 3,
    Cancelado = 4,
}

public static class StatusTransacaoExtensions
{
    /// <summary>Entra no saldo realizado (SR).</summary>
    public static bool IsRealizado(this StatusTransacao status) =>
        status is StatusTransacao.Pago or StatusTransacao.Recebido;

    /// <summary>Entra no saldo previsto (SP) mas não no realizado.</summary>
    public static bool IsPendenteParaPrevisao(this StatusTransacao status) =>
        status == StatusTransacao.Pendente;

    /// <summary>Excluído de todos os cálculos.</summary>
    public static bool IsExcluido(this StatusTransacao status) =>
        status == StatusTransacao.Cancelado;
}
