using ControleFinanceiro.Domain.Transacoes;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Transacoes;

public class RecorrenciaTests
{
    [Fact]
    public void Create_Should_ReturnFailure_When_ParceladaSemTotalParcelas()
    {
        var result = Recorrencia.Create(Guid.NewGuid(), TipoRecorrencia.Parcelada, null, null, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecorrenciaErrors.TotalParcelasObrigatorio);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_Should_ReturnFailure_When_ParceladaComTotalParcelasInvalido(int totalParcelas)
    {
        var result = Recorrencia.Create(Guid.NewGuid(), TipoRecorrencia.Parcelada, null, totalParcelas, null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecorrenciaErrors.TotalParcelasObrigatorio);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ParcelaAtualForaDoIntervalo()
    {
        var result = Recorrencia.Create(Guid.NewGuid(), TipoRecorrencia.Parcelada, null, 3, 4, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecorrenciaErrors.ParcelaAtualInvalida);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_Parcelada()
    {
        var result = Recorrencia.Create(Guid.NewGuid(), TipoRecorrencia.Parcelada, null, 12, 1, null);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalParcelas.Should().Be(12);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_MensalSemTotalParcelas()
    {
        var result = Recorrencia.Create(Guid.NewGuid(), TipoRecorrencia.Mensal, null, null, null, null);

        result.IsSuccess.Should().BeTrue();
    }
}
