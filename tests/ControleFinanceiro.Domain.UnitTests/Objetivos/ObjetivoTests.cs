using ControleFinanceiro.Domain.Objetivos;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Objetivos;

public class ObjetivoTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_DadosValidos()
    {
        var result = Objetivo.Create(Guid.NewGuid(), "Viagem", 5000m, new DateOnly(2026, 12, 31));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NomeVazio()
    {
        var result = Objetivo.Create(Guid.NewGuid(), " ", 5000m, new DateOnly(2026, 12, 31));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ObjetivoErrors.NomeObrigatorio);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ValorAlvoNaoPositivo()
    {
        var result = Objetivo.Create(Guid.NewGuid(), "Viagem", 0m, new DateOnly(2026, 12, 31));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ObjetivoErrors.ValorAlvoInvalido);
    }
}

public class AporteTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_ValorPositivo()
    {
        var result = Aporte.Create(Guid.NewGuid(), 100m, DateOnly.FromDateTime(DateTime.Today));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ValorNaoPositivo()
    {
        var result = Aporte.Create(Guid.NewGuid(), 0m, DateOnly.FromDateTime(DateTime.Today));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ObjetivoErrors.ValorAporteInvalido);
    }

    [Fact]
    public void Atualizar_Should_AlterarDados_When_Validos()
    {
        var objetivo = Objetivo.Create(Guid.NewGuid(), "Viagem", 5000m, new DateOnly(2026, 12, 31)).Value;

        var result = objetivo.Atualizar(" Viagem para Floripa ", 6000m, new DateOnly(2027, 7, 15));

        result.IsSuccess.Should().BeTrue();
        objetivo.Nome.Should().Be("Viagem para Floripa");
        objetivo.ValorAlvo.Should().Be(6000m);
        objetivo.DataAlvo.Should().Be(new DateOnly(2027, 7, 15));
    }

    [Fact]
    public void Atualizar_Should_ManterDados_When_NomeVazio()
    {
        var objetivo = Objetivo.Create(Guid.NewGuid(), "Viagem", 5000m, new DateOnly(2026, 12, 31)).Value;

        var result = objetivo.Atualizar(" ", 6000m, new DateOnly(2027, 7, 15));

        result.Error.Should().Be(ObjetivoErrors.NomeObrigatorio);
        objetivo.Nome.Should().Be("Viagem");
        objetivo.ValorAlvo.Should().Be(5000m);
    }
}
