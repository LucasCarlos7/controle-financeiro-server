using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Objetivos;

public sealed class Objetivo : Entity
{
    public Guid UserId { get; private set; }
    public string Nome { get; private set; }
    public decimal ValorAlvo { get; private set; }
    public DateOnly DataAlvo { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Objetivo(Guid id, Guid userId, string nome, decimal valorAlvo, DateOnly dataAlvo, DateTime criadoEm)
        : base(id)
    {
        UserId = userId;
        Nome = nome;
        ValorAlvo = valorAlvo;
        DataAlvo = dataAlvo;
        CriadoEm = criadoEm;
    }

    public static Result<Objetivo> Create(Guid userId, string nome, decimal valorAlvo, DateOnly dataAlvo)
    {
        var validacao = Validar(nome, valorAlvo);
        if (validacao.IsFailure)
            return Result.Failure<Objetivo>(validacao.Error);

        return Result.Success(new Objetivo(Guid.NewGuid(), userId, nome.Trim(), valorAlvo, dataAlvo, DateTime.UtcNow));
    }

    public Result Atualizar(string nome, decimal valorAlvo, DateOnly dataAlvo)
    {
        var validacao = Validar(nome, valorAlvo);
        if (validacao.IsFailure)
            return validacao;

        Nome = nome.Trim();
        ValorAlvo = valorAlvo;
        DataAlvo = dataAlvo;
        return Result.Success();
    }

    private static Result Validar(string nome, decimal valorAlvo)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(ObjetivoErrors.NomeObrigatorio);

        if (valorAlvo <= 0)
            return Result.Failure(ObjetivoErrors.ValorAlvoInvalido);

        return Result.Success();
    }
}
