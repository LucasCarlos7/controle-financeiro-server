using ControleFinanceiro.Domain.Usuarios;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Usuarios;

public class UsuarioTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_DadosValidos()
    {
        var result = Usuario.Create("Lucas", "lucas@example.com", "hash");

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("lucas@example.com");
    }

    [Fact]
    public void Create_Should_NormalizarEmail_Should_ToLowerCase()
    {
        var result = Usuario.Create("Lucas", "Lucas@Example.COM", "hash");

        result.Value.Email.Should().Be("lucas@example.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("nao-e-email")]
    [InlineData("sem-arroba.com")]
    public void Create_Should_ReturnFailure_When_EmailInvalido(string email)
    {
        var result = Usuario.Create("Lucas", email, "hash");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UsuarioErrors.EmailInvalido);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NomeVazio()
    {
        var result = Usuario.Create(" ", "lucas@example.com", "hash");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UsuarioErrors.NomeObrigatorio);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_SenhaHashVazia()
    {
        var result = Usuario.Create("Lucas", "lucas@example.com", " ");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UsuarioErrors.SenhaObrigatoria);
    }
}
