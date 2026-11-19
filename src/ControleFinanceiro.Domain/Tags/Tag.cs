using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Tags;

public sealed class Tag : Entity
{
    public Guid UserId { get; private set; }
    public string Nome { get; private set; }
    public string? Icone { get; private set; }
    public bool IsPadrao { get; private set; }

    private Tag(Guid id, Guid userId, string nome, string? icone, bool isPadrao) : base(id)
    {
        UserId = userId;
        Nome = nome;
        Icone = icone;
        IsPadrao = isPadrao;
    }

    public static Result<Tag> Create(Guid userId, string nome, string? icone = null, bool isPadrao = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Tag>(TagErrors.NomeObrigatorio);

        return Result.Success(new Tag(Guid.NewGuid(), userId, nome.Trim(), Icones.Normalizar(icone), isPadrao));
    }

    public Result AtualizarDetalhes(string nome, string? icone)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(TagErrors.NomeObrigatorio);

        Nome = nome.Trim();
        Icone = Icones.Normalizar(icone);
        return Result.Success();
    }
}
