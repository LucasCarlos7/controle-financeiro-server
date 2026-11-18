using ControleFinanceiro.Domain.Contas;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Contas;

public class ContaTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_DadosValidos()
    {
        var result = Conta.Create(Guid.NewGuid(), "Conta Corrente", TipoConta.Corrente, 100m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Nome.Should().Be("Conta Corrente");
        result.Value.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_ReturnFailure_When_NomeVazio(string nome)
    {
        var result = Conta.Create(Guid.NewGuid(), nome, TipoConta.Corrente, 0m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContaErrors.NomeObrigatorio);
    }

    [Fact]
    public void AtualizarDetalhes_Should_ReturnFailure_When_NomeVazio()
    {
        var conta = Conta.Create(Guid.NewGuid(), "Conta Corrente", TipoConta.Corrente, 0m).Value;

        var result = conta.AtualizarDetalhes(" ", null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContaErrors.NomeObrigatorio);
    }

    [Fact]
    public void Desativar_Should_MarcarContaComoInativa()
    {
        var conta = Conta.Create(Guid.NewGuid(), "Conta Corrente", TipoConta.Corrente, 0m).Value;

        conta.Desativar();

        conta.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Reativar_Should_MarcarContaComoAtiva()
    {
        var conta = Conta.Create(Guid.NewGuid(), "Conta Corrente", TipoConta.Corrente, 0m).Value;
        conta.Desativar();

        conta.Reativar();

        conta.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_NormalizarIcone_When_IconeInformado()
    {
        var comIcone = Conta.Create(Guid.NewGuid(), "Nubank", TipoConta.Corrente, 0m, " bank ").Value;
        var semIcone = Conta.Create(Guid.NewGuid(), "Carteira", TipoConta.Carteira, 0m, "  ").Value;

        comIcone.Icone.Should().Be("bank");
        semIcone.Icone.Should().BeNull();
    }

    [Fact]
    public void AtualizarDetalhes_Should_TrocarNomeEIcone()
    {
        var conta = Conta.Create(Guid.NewGuid(), "Conta", TipoConta.Corrente, 0m, "bank").Value;

        var result = conta.AtualizarDetalhes(" Nubank ", "wallet");

        result.IsSuccess.Should().BeTrue();
        conta.Nome.Should().Be("Nubank");
        conta.Icone.Should().Be("wallet");
    }
}
