using ControleFinanceiro.Application.Auth.Commands.Register;
using FluentValidation.TestHelper;

namespace ControleFinanceiro.Application.UnitTests.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_EmailInvalido()
    {
        var command = new RegisterCommand { Nome = "Lucas", Email = "invalido", Senha = "12345678" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_When_SenhaMuitoCurta()
    {
        var command = new RegisterCommand { Nome = "Lucas", Email = "lucas@example.com", Senha = "123" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void Should_NotHaveErrors_When_ComandoValido()
    {
        var command = new RegisterCommand { Nome = "Lucas", Email = "lucas@example.com", Senha = "12345678" };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
