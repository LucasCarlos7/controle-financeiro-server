using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Categorias;

public sealed class Categoria : Entity
{
    public Guid UserId { get; private set; }
    public string Nome { get; private set; }
    public TipoCategoria Tipo { get; private set; }
    public string? Icone { get; private set; }
    public string? Cor { get; private set; }
    public bool IsPadrao { get; private set; }

    private readonly List<Subcategoria> _subcategorias = [];
    public IReadOnlyList<Subcategoria> Subcategorias => _subcategorias.AsReadOnly();

    private Categoria(Guid id, Guid userId, string nome, TipoCategoria tipo, string? icone, string? cor, bool isPadrao)
        : base(id)
    {
        UserId = userId;
        Nome = nome;
        Tipo = tipo;
        Icone = icone;
        Cor = cor;
        IsPadrao = isPadrao;
    }

    public static Result<Categoria> Create(Guid userId, string nome, TipoCategoria tipo, string? icone, string? cor, bool isPadrao = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Categoria>(CategoriaErrors.NomeObrigatorio);

        return Result.Success(new Categoria(Guid.NewGuid(), userId, nome.Trim(), tipo, icone, cor, isPadrao));
    }

    public Result AtualizarDetalhes(string nome, string? icone, string? cor)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(CategoriaErrors.NomeObrigatorio);

        Nome = nome.Trim();
        Icone = icone;
        Cor = cor;

        return Result.Success();
    }

    public Result<Subcategoria> AdicionarSubcategoria(string nome)
    {
        var result = Subcategoria.Create(Id, nome);
        if (result.IsFailure)
            return result;

        _subcategorias.Add(result.Value);
        return result;
    }

    public Result AtualizarSubcategoria(Guid subcategoriaId, string nome)
    {
        var subcategoria = _subcategorias.Find(s => s.Id == subcategoriaId);
        if (subcategoria is null)
            return Result.Failure(CategoriaErrors.SubcategoriaNaoEncontrada);

        return subcategoria.AtualizarNome(nome);
    }

    public Result RemoverSubcategoria(Guid subcategoriaId)
    {
        var subcategoria = _subcategorias.Find(s => s.Id == subcategoriaId);
        if (subcategoria is null)
            return Result.Failure(CategoriaErrors.SubcategoriaNaoEncontrada);

        _subcategorias.Remove(subcategoria);
        return Result.Success();
    }
}
