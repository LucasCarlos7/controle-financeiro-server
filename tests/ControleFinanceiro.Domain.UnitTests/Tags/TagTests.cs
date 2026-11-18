using ControleFinanceiro.Domain.Tags;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Tags;

public class TagTests
{
    [Fact]
    public void Create_Should_GuardarIcone_When_IconeInformado()
    {
        var result = Tag.Create(Guid.NewGuid(), " viagem ", "plane");

        result.IsSuccess.Should().BeTrue();
        result.Value.Nome.Should().Be("viagem");
        result.Value.Icone.Should().Be("plane");
    }

    [Fact]
    public void AtualizarDetalhes_Should_TrocarNomeEIcone_When_NomeValido()
    {
        var tag = Tag.Create(Guid.NewGuid(), "viagem", "plane").Value;

        var result = tag.AtualizarDetalhes("férias", "");

        result.IsSuccess.Should().BeTrue();
        tag.Nome.Should().Be("férias");
        tag.Icone.Should().BeNull();
    }

    [Fact]
    public void AtualizarDetalhes_Should_ReturnFailure_When_NomeVazio()
    {
        var tag = Tag.Create(Guid.NewGuid(), "viagem", "plane").Value;

        var result = tag.AtualizarDetalhes(" ", "tag");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TagErrors.NomeObrigatorio);
        tag.Icone.Should().Be("plane");
    }
}
