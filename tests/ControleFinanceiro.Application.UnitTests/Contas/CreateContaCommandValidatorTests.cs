using ControleFinanceiro.Application.Contas.Commands.CreateConta;
using ControleFinanceiro.Domain.Contas;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ControleFinanceiro.Application.UnitTests.Contas;

public class CreateContaCommandValidatorTests
{
    private readonly CreateContaCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_NomeVazio()
    {
        var command = new CreateContaCommand
        {
            UserId = Guid.NewGuid(),
            Nome = string.Empty,
            Tipo = TipoConta.Corrente,
            SaldoInicial = 0m,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Should_HaveError_When_UserIdVazio()
    {
        var command = new CreateContaCommand
        {
            UserId = Guid.Empty,
            Nome = "Conta Corrente",
            Tipo = TipoConta.Corrente,
            SaldoInicial = 0m,
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_NotHaveErrors_When_ComandoValido()
    {
        var command = new CreateContaCommand
        {
            UserId = Guid.NewGuid(),
            Nome = "Conta Corrente",
            Tipo = TipoConta.Corrente,
            SaldoInicial = 100m,
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_IconeMuitoLongo()
    {
        var command = new CreateContaCommand
        {
            UserId = Guid.NewGuid(),
            Nome = "Conta Corrente",
            Tipo = TipoConta.Corrente,
            Icone = new string('a', 51),
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Icone);
    }
}
