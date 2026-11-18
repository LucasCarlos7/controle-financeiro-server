using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Objetivos;

public sealed class Aporte : Entity
{
    public Guid ObjetivoId { get; private set; }
    public decimal Valor { get; private set; }
    public DateOnly Data { get; private set; }

    private Aporte(Guid id, Guid objetivoId, decimal valor, DateOnly data) : base(id)
    {
        ObjetivoId = objetivoId;
        Valor = valor;
        Data = data;
    }

    public static Result<Aporte> Create(Guid objetivoId, decimal valor, DateOnly data)
    {
        if (valor <= 0)
            return Result.Failure<Aporte>(ObjetivoErrors.ValorAporteInvalido);

        return Result.Success(new Aporte(Guid.NewGuid(), objetivoId, valor, data));
    }
}
