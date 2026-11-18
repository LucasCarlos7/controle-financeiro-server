using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Categorias;

public sealed class Subcategoria : Entity
{
    public Guid CategoriaId { get; private set; }
    public string Nome { get; private set; }

    private Subcategoria(Guid id, Guid categoriaId, string nome) : base(id)
    {
        CategoriaId = categoriaId;
        Nome = nome;
    }

    public static Result<Subcategoria> Create(Guid categoriaId, string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Subcategoria>(CategoriaErrors.NomeSubcategoriaObrigatorio);

        return Result.Success(new Subcategoria(Guid.NewGuid(), categoriaId, nome.Trim()));
    }

    public Result AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(CategoriaErrors.NomeSubcategoriaObrigatorio);

        Nome = nome.Trim();
        return Result.Success();
    }
}
