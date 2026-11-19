using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Transacoes;

public sealed class Recorrencia : Entity
{
    public Guid UserId { get; private set; }
    public TipoRecorrencia Tipo { get; private set; }
    public int? IntervaloMeses { get; private set; }
    public int? TotalParcelas { get; private set; }
    public int? ParcelaAtual { get; private set; }

    private Recorrencia(
        Guid id,
        Guid userId,
        TipoRecorrencia tipo,
        int? intervaloMeses,
        int? totalParcelas,
        int? parcelaAtual)
        : base(id)
    {
        UserId = userId;
        Tipo = tipo;
        IntervaloMeses = intervaloMeses;
        TotalParcelas = totalParcelas;
        ParcelaAtual = parcelaAtual;
    }

    public static Result<Recorrencia> Create(
        Guid userId,
        TipoRecorrencia tipo,
        int? intervaloMeses,
        int? totalParcelas,
        int? parcelaAtual)
    {
        if (tipo == TipoRecorrencia.Parcelada && (totalParcelas is null or <= 0))
            return Result.Failure<Recorrencia>(RecorrenciaErrors.TotalParcelasObrigatorio);

        if (tipo == TipoRecorrencia.Parcelada && parcelaAtual is not null && (parcelaAtual <= 0 || parcelaAtual > totalParcelas))
            return Result.Failure<Recorrencia>(RecorrenciaErrors.ParcelaAtualInvalida);

        return Result.Success(new Recorrencia(
            Guid.NewGuid(), userId, tipo, intervaloMeses, totalParcelas, parcelaAtual));
    }
}
