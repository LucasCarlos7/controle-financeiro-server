using ControleFinanceiro.Application.Auth.Commands.Register;
using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using FluentAssertions;

namespace ControleFinanceiro.Application.UnitTests.Auth;

public class DadosIniciaisUsuarioTests
{
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void CriarCategorias_Should_CriarCategoriasDoUsuarioComReceitasEDespesas()
    {
        var categorias = DadosIniciaisUsuario.CriarCategorias(_userId);

        categorias.Should().NotBeEmpty().And.OnlyContain(c => c.UserId == _userId);
        categorias.Should().Contain(c => c.Tipo == TipoCategoria.Despesa);
        categorias.Should().Contain(c => c.Tipo == TipoCategoria.Receita);
    }

    [Fact]
    public void CriarCategorias_Should_NaoRepetirNome_When_MesmoTipo()
    {
        var categorias = DadosIniciaisUsuario.CriarCategorias(_userId);

        categorias.GroupBy(c => (c.Tipo, Nome: c.Nome.ToLowerInvariant()))
            .Should().OnlyContain(grupo => grupo.Count() == 1);
    }

    [Fact]
    public void CriarCategorias_Should_VincularSubcategoriasACategoria()
    {
        var categorias = DadosIniciaisUsuario.CriarCategorias(_userId);

        var moradia = categorias.Single(c => c.Nome == "Moradia");
        moradia.Subcategorias.Should().Contain(s => s.Nome == "Aluguel")
            .And.OnlyContain(s => s.CategoriaId == moradia.Id);
        categorias.Should().OnlyContain(c => c.Subcategorias.Select(s => s.Nome).Distinct().Count() == c.Subcategorias.Count);
    }

    [Fact]
    public void CriarCategorias_Should_RespeitarLimitesDaPersistencia()
    {
        var categorias = DadosIniciaisUsuario.CriarCategorias(_userId);

        categorias.Should().OnlyContain(c => c.Nome.Length <= 100 && (c.Icone == null || c.Icone.Length <= Icones.TamanhoMaximo));
    }

    [Fact]
    public void CriarCategorias_Should_GerarNovosIds_When_ChamadoParaOutroUsuario()
    {
        var primeiro = DadosIniciaisUsuario.CriarCategorias(_userId);
        var segundo = DadosIniciaisUsuario.CriarCategorias(Guid.NewGuid());

        primeiro.Select(c => c.Id).Should().NotIntersectWith(segundo.Select(c => c.Id));
    }

    [Fact]
    public void CriarTags_Should_CriarTagsDoUsuarioSemNomeRepetido()
    {
        var tags = DadosIniciaisUsuario.CriarTags(_userId);

        tags.Should().NotBeEmpty().And.OnlyContain(t => t.UserId == _userId && t.Nome.Length <= 50);
        tags.Select(t => t.Nome.ToLowerInvariant()).Should().OnlyHaveUniqueItems();
    }
}
