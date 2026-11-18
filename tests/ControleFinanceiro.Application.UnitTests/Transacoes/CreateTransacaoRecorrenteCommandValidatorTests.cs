using ControleFinanceiro.Application.Transacoes.Commands.CreateTransacaoRecorrente;
using ControleFinanceiro.Domain.Transacoes;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ControleFinanceiro.Application.UnitTests.Transacoes;

public class CreateTransacaoRecorrenteCommandValidatorTests
{
    private readonly CreateTransacaoRecorrenteCommandValidator _validator = new();

    private static CreateTransacaoRecorrenteCommand ValidCommand() => new()
    {
        UserId = Guid.NewGuid(),
        Tipo = TipoTransacao.Despesa,
        Valor = 100m,
        DataInicio = new DateOnly(2026, 1, 10),
        Descricao = "Assinatura streaming",
        ContaId = Guid.NewGuid(),
        TipoRecorrencia = TipoRecorrencia.Mensal,
    };

    [Fact]
    public void Should_ReturnError_When_ParceladaSemTotalParcelas()
    {
        var command = ValidCommand() with { TipoRecorrencia = TipoRecorrencia.Parcelada, TotalParcelas = null };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TotalParcelas);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(121)]
    public void Should_ReturnError_When_TotalParcelasForaDoIntervalo(int totalParcelas)
    {
        var command = ValidCommand() with { TipoRecorrencia = TipoRecorrencia.Parcelada, TotalParcelas = totalParcelas };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.TotalParcelas);
    }

    [Fact]
    public void Should_ReturnError_When_DataFimAnteriorADataInicio()
    {
        var command = ValidCommand() with { DataFim = new DateOnly(2025, 12, 1) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DataFim);
    }

    [Fact]
    public void Should_NotHaveErrors_When_ComandoValido()
    {
        var command = ValidCommand() with { TipoRecorrencia = TipoRecorrencia.Parcelada, TotalParcelas = 12 };

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }
}
