using ControleFinanceiro.Domain.Categorias;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Categorias;

public class CategoriaTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_DadosValidos()
    {
        var result = Categoria.Create(Guid.NewGuid(), "Alimentação", TipoCategoria.Despesa, "🍔", "#FF0000");

        result.IsSuccess.Should().BeTrue();
        result.Value.Subcategorias.Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NomeVazio()
    {
        var result = Categoria.Create(Guid.NewGuid(), " ", TipoCategoria.Despesa, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoriaErrors.NomeObrigatorio);
    }

    [Fact]
    public void AdicionarSubcategoria_Should_AdicionarNaLista_When_NomeValido()
    {
        var categoria = Categoria.Create(Guid.NewGuid(), "Alimentação", TipoCategoria.Despesa, null, null).Value;

        var result = categoria.AdicionarSubcategoria("Restaurante");

        result.IsSuccess.Should().BeTrue();
        categoria.Subcategorias.Should().ContainSingle(s => s.Nome == "Restaurante");
    }

    [Fact]
    public void AdicionarSubcategoria_Should_ReturnFailure_When_NomeVazio()
    {
        var categoria = Categoria.Create(Guid.NewGuid(), "Alimentação", TipoCategoria.Despesa, null, null).Value;

        var result = categoria.AdicionarSubcategoria(" ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoriaErrors.NomeSubcategoriaObrigatorio);
    }

    [Fact]
    public void RemoverSubcategoria_Should_ReturnFailure_When_SubcategoriaNaoExiste()
    {
        var categoria = Categoria.Create(Guid.NewGuid(), "Alimentação", TipoCategoria.Despesa, null, null).Value;

        var result = categoria.RemoverSubcategoria(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoriaErrors.SubcategoriaNaoEncontrada);
    }

    [Fact]
    public void AtualizarSubcategoria_Should_AtualizarNome_When_SubcategoriaExiste()
    {
        var categoria = Categoria.Create(Guid.NewGuid(), "Alimentação", TipoCategoria.Despesa, null, null).Value;
        var subcategoriaId = categoria.AdicionarSubcategoria("Restaurante").Value.Id;

        var result = categoria.AtualizarSubcategoria(subcategoriaId, "Delivery");

        result.IsSuccess.Should().BeTrue();
        categoria.Subcategorias.Should().ContainSingle(s => s.Nome == "Delivery");
    }
}
