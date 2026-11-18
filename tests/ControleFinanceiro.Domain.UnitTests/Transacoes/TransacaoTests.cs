using ControleFinanceiro.Domain.Transacoes;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Transacoes;

public class TransacaoTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ContaId = Guid.NewGuid();
    private static readonly Guid CartaoCreditoId = Guid.NewGuid();
    private static readonly Guid ContaDestinoId = Guid.NewGuid();

    [Fact]
    public void Create_Should_ReturnSuccess_When_DespesaComConta()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ValorNaoPositivo()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Despesa, 0m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.ValorInvalido);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NemContaNemCartaoInformados()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            null, null, null, null, null, StatusTransacao.Pendente, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.OrigemInvalida);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ContaECartaoInformadosJuntos()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, CartaoCreditoId, null, null, null, StatusTransacao.Pendente, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.OrigemInvalida);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_TransferenciaOriginadaDeCartao()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Transferencia, 100m, DateOnly.FromDateTime(DateTime.Today), "Pagamento fatura",
            null, CartaoCreditoId, ContaDestinoId, null, null, StatusTransacao.Pendente, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.TransferenciaNaoPermiteCartao);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_TransferenciaSemContaDestino()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Transferencia, 100m, DateOnly.FromDateTime(DateTime.Today), "Transferência",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.ContaDestinoObrigatoria);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ContaDestinoIgualContaOrigem()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Transferencia, 100m, DateOnly.FromDateTime(DateTime.Today), "Transferência",
            ContaId, null, ContaId, null, null, StatusTransacao.Pendente, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.ContaDestinoIgualOrigem);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ContaDestinoInformadaForaDeTransferencia()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, null, ContaDestinoId, null, null, StatusTransacao.Pendente, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.ContaDestinoNaoPermitida);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_TransferenciaValida()
    {
        var result = Transacao.Create(
            UserId, TipoTransacao.Transferencia, 100m, DateOnly.FromDateTime(DateTime.Today), "Transferência",
            ContaId, null, ContaDestinoId, null, null, StatusTransacao.Pendente, null);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void MarcarComoPaga_Should_AlterarStatusParaPago()
    {
        var transacao = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;

        transacao.MarcarComoPaga();

        transacao.Status.Should().Be(StatusTransacao.Pago);
    }

    [Fact]
    public void AtualizarDetalhes_Should_ReturnFailure_When_ValorNaoPositivo()
    {
        var transacao = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;

        var result = transacao.AtualizarDetalhes(0m, DateOnly.FromDateTime(DateTime.Today), "Mercado", null, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.ValorInvalido);
    }

    [Fact]
    public void AtualizarDetalhes_Should_AtualizarCampos_When_DadosValidos()
    {
        var transacao = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;
        var novaData = DateOnly.FromDateTime(DateTime.Today).AddDays(-1);
        var categoriaId = Guid.NewGuid();

        var result = transacao.AtualizarDetalhes(200m, novaData, "Farmácia", categoriaId, null);

        result.IsSuccess.Should().BeTrue();
        transacao.Valor.Should().Be(200m);
        transacao.Data.Should().Be(novaData);
        transacao.Descricao.Should().Be("Farmácia");
        transacao.CategoriaId.Should().Be(categoriaId);
    }

    [Fact]
    public void AdicionarTag_Should_NaoDuplicar_When_TagJaAdicionada()
    {
        var transacao = Transacao.Create(
            UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today), "Mercado",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;
        var tagId = Guid.NewGuid();

        transacao.AdicionarTag(tagId);
        transacao.AdicionarTag(tagId);

        transacao.TagIds.Should().ContainSingle().Which.Should().Be(tagId);
    }
}
