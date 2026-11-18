using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Orcamentos;

public sealed class Orcamento : Entity
{
    public Guid UserId { get; private set; }
    public Guid CategoriaId { get; private set; }
    public int Ano { get; private set; }
    public int Mes { get; private set; }
    public decimal LimiteValor { get; private set; }

    private Orcamento(Guid id, Guid userId, Guid categoriaId, int ano, int mes, decimal limiteValor) : base(id)
    {
        UserId = userId;
        CategoriaId = categoriaId;
        Ano = ano;
        Mes = mes;
        LimiteValor = limiteValor;
    }

    public static Result<Orcamento> Create(Guid userId, Guid categoriaId, int ano, int mes, decimal limiteValor)
    {
        if (mes is < 1 or > 12)
            return Result.Failure<Orcamento>(OrcamentoErrors.MesInvalido);

        if (ano < 2000)
            return Result.Failure<Orcamento>(OrcamentoErrors.AnoInvalido);

        if (limiteValor <= 0)
            return Result.Failure<Orcamento>(OrcamentoErrors.LimiteInvalido);

        return Result.Success(new Orcamento(Guid.NewGuid(), userId, categoriaId, ano, mes, limiteValor));
    }

    public Result AtualizarLimite(decimal limiteValor)
    {
        if (limiteValor <= 0)
            return Result.Failure(OrcamentoErrors.LimiteInvalido);

        LimiteValor = limiteValor;
        return Result.Success();
    }
}
