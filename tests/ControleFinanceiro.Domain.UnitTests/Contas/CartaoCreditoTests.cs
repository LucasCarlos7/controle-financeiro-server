using ControleFinanceiro.Domain.Contas;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Contas;

public class CartaoCreditoTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_DadosValidos()
    {
        var result = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 5000m, 10, 17, null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_LimiteNaoPositivo()
    {
        var result = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 0m, 10, 17, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartaoCreditoErrors.LimiteInvalido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void Create_Should_ReturnFailure_When_DiaFechamentoForaDoIntervalo(int diaFechamento)
    {
        var result = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 1000m, diaFechamento, 17, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartaoCreditoErrors.DiaFechamentoInvalido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void Create_Should_ReturnFailure_When_DiaVencimentoForaDoIntervalo(int diaVencimento)
    {
        var result = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 1000m, 10, diaVencimento, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartaoCreditoErrors.DiaVencimentoInvalido);
    }

    [Fact]
    public void Atualizar_Should_AlterarDados_When_Validos()
    {
        var cartao = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 5000m, 10, 17, null).Value;
        var contaPagamentoId = Guid.NewGuid();

        var result = cartao.Atualizar("  Nubank Ultravioleta ", 8000m, 3, 10, contaPagamentoId);

        result.IsSuccess.Should().BeTrue();
        cartao.Nome.Should().Be("Nubank Ultravioleta");
        cartao.Limite.Should().Be(8000m);
        cartao.DiaFechamento.Should().Be(3);
        cartao.DiaVencimento.Should().Be(10);
        cartao.ContaPagamentoId.Should().Be(contaPagamentoId);
    }

    [Fact]
    public void Atualizar_Should_ManterDados_When_LimiteInvalido()
    {
        var cartao = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 5000m, 10, 17, null).Value;

        var result = cartao.Atualizar("Outro nome", 0m, 3, 10, null);

        result.Error.Should().Be(CartaoCreditoErrors.LimiteInvalido);
        cartao.Nome.Should().Be("Nubank");
        cartao.Limite.Should().Be(5000m);
    }

    [Fact]
    public void Atualizar_Should_TrocarIcone_When_DadosValidos()
    {
        var cartao = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 5000m, 10, 17, null, "card").Value;

        var result = cartao.Atualizar("Nubank", 5000m, 10, 17, null, "gift");

        result.IsSuccess.Should().BeTrue();
        cartao.Icone.Should().Be("gift");
    }

    [Fact]
    public void Atualizar_Should_ManterIconeOriginal_When_DadosInvalidos()
    {
        var cartao = CartaoCredito.Create(Guid.NewGuid(), "Nubank", 5000m, 10, 17, null, "card").Value;

        var result = cartao.Atualizar("Nubank", 0m, 10, 17, null, "gift");

        result.IsFailure.Should().BeTrue();
        cartao.Icone.Should().Be("card");
    }
}
