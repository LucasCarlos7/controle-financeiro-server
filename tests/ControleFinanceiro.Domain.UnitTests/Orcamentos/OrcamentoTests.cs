using ControleFinanceiro.Domain.Orcamentos;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Orcamentos;

public class OrcamentoTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_DadosValidos()
    {
        var result = Orcamento.Create(Guid.NewGuid(), Guid.NewGuid(), 2026, 8, 1000m);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Create_Should_ReturnFailure_When_MesInvalido(int mes)
    {
        var result = Orcamento.Create(Guid.NewGuid(), Guid.NewGuid(), 2026, mes, 1000m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrcamentoErrors.MesInvalido);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_LimiteNaoPositivo()
    {
        var result = Orcamento.Create(Guid.NewGuid(), Guid.NewGuid(), 2026, 8, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrcamentoErrors.LimiteInvalido);
    }

    [Fact]
    public void AtualizarLimite_Should_ReturnFailure_When_NovoLimiteInvalido()
    {
        var orcamento = Orcamento.Create(Guid.NewGuid(), Guid.NewGuid(), 2026, 8, 1000m).Value;

        var result = orcamento.AtualizarLimite(-1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrcamentoErrors.LimiteInvalido);
    }
}
