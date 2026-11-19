using ControleFinanceiro.Domain.Transacoes;
using FluentAssertions;

namespace ControleFinanceiro.Domain.UnitTests.Transacoes;

public class StatusTransacaoTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ContaId = Guid.NewGuid();

    private static Transacao CriarDespesa(StatusTransacao status = StatusTransacao.Pendente) =>
        Transacao.Create(UserId, TipoTransacao.Despesa, 100m, DateOnly.FromDateTime(DateTime.Today),
            "Mercado", ContaId, null, null, null, null, status, null).Value;

    private static Transacao CriarReceita(StatusTransacao status = StatusTransacao.Pendente) =>
        Transacao.Create(UserId, TipoTransacao.Receita, 100m, DateOnly.FromDateTime(DateTime.Today),
            "Salário", ContaId, null, null, null, null, status, null).Value;

    // --- IsRealizado ---

    [Theory]
    [InlineData(StatusTransacao.Pago, true)]
    [InlineData(StatusTransacao.Recebido, true)]
    [InlineData(StatusTransacao.Pendente, false)]
    [InlineData(StatusTransacao.Cancelado, false)]
    public void IsRealizado_Should_ReflectCorrectly(StatusTransacao status, bool expected)
        => status.IsRealizado().Should().Be(expected);

    // --- IsPendenteParaPrevisao ---

    [Theory]
    [InlineData(StatusTransacao.Pendente, true)]
    [InlineData(StatusTransacao.Pago, false)]
    [InlineData(StatusTransacao.Recebido, false)]
    [InlineData(StatusTransacao.Cancelado, false)]
    public void IsPendenteParaPrevisao_Should_ReflectCorrectly(StatusTransacao status, bool expected)
        => status.IsPendenteParaPrevisao().Should().Be(expected);

    // --- MarcarComoEfetivada ---

    [Fact]
    public void MarcarComoEfetivada_Should_SetPago_When_Despesa()
    {
        var despesa = CriarDespesa();
        despesa.MarcarComoEfetivada();
        despesa.Status.Should().Be(StatusTransacao.Pago);
    }

    [Fact]
    public void MarcarComoEfetivada_Should_SetRecebido_When_Receita()
    {
        var receita = CriarReceita();
        receita.MarcarComoEfetivada();
        receita.Status.Should().Be(StatusTransacao.Recebido);
    }

    [Fact]
    public void MarcarComoEfetivada_Should_ReturnFailure_When_JaCancelado()
    {
        var transacao = CriarDespesa(StatusTransacao.Cancelado);
        var result = transacao.MarcarComoEfetivada();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.StatusInvalido);
    }

    // --- Cancelar ---

    [Fact]
    public void Cancelar_Should_SetCancelado_When_Pendente()
    {
        var transacao = CriarDespesa();
        var result = transacao.Cancelar();
        result.IsSuccess.Should().BeTrue();
        transacao.Status.Should().Be(StatusTransacao.Cancelado);
    }

    [Fact]
    public void Cancelar_Should_ReturnFailure_When_JaEfetivado()
    {
        var transacao = CriarDespesa(StatusTransacao.Pago);
        var result = transacao.Cancelar();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TransacaoErrors.NaoPodeCancelarEfetivado);
    }

    // --- Fórmulas SR / SP ---

    [Fact]
    public void SaldoRealizado_Should_SerZero_When_SomentePendentes()
    {
        // SR = RR - DR; pendentes não entram
        var transacoes = new[]
        {
            CriarReceita(StatusTransacao.Pendente),
            CriarDespesa(StatusTransacao.Pendente),
        };

        var sr = transacoes
            .Where(t => t.Status.IsRealizado())
            .Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        sr.Should().Be(0m);
    }

    [Fact]
    public void SaldoPrevisto_Should_ConsiderarPendentes()
    {
        // SP = (RR + RP) - (DR + DP)
        // Receita pendente R$4123, Despesa pendente R$65 → SP = R$4058
        var receita = Transacao.Create(UserId, TipoTransacao.Receita, 4123m,
            DateOnly.FromDateTime(DateTime.Today), "Salário",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;

        var despesa = Transacao.Create(UserId, TipoTransacao.Despesa, 65m,
            DateOnly.FromDateTime(DateTime.Today), "Internet",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;

        var transacoes = new[] { receita, despesa };

        var receitasPrevistas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Receita && !t.Status.IsExcluido())
            .Sum(t => t.Valor);

        var despesasPrevistas = transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa && !t.Status.IsExcluido())
            .Sum(t => t.Valor);

        var sp = receitasPrevistas - despesasPrevistas;

        sp.Should().Be(4058m);
    }

    [Fact]
    public void SaldoPrevisto_Should_MantidoAposEfetivar_Receita()
    {
        // Marcar receita como recebida: SR aumenta, SP permanece igual
        var receita = Transacao.Create(UserId, TipoTransacao.Receita, 4123m,
            DateOnly.FromDateTime(DateTime.Today), "Salário",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;

        var despesa = Transacao.Create(UserId, TipoTransacao.Despesa, 65m,
            DateOnly.FromDateTime(DateTime.Today), "Internet",
            ContaId, null, null, null, null, StatusTransacao.Pendente, null).Value;

        // Antes
        var spAntes = receita.Valor - despesa.Valor; // 4058

        receita.MarcarComoEfetivada();

        // Depois: SR = 4123, SP = 4058 (inalterado)
        var receitasRealizadas = receita.Status.IsRealizado() ? receita.Valor : 0m;
        var receitasPrevistas = receita.Valor + (receita.Status.IsRealizado() ? 0m : receita.Valor); // simplificado

        var spDepois = (receitasRealizadas + 0m) - (0m + despesa.Valor);

        receitasRealizadas.Should().Be(4123m);
        spDepois.Should().Be(4058m); // SP = SR + PL
    }

    [Fact]
    public void Cancelado_Should_NaoEntrarNosCalculos()
    {
        var transacao = CriarReceita();
        transacao.Cancelar();

        transacao.Status.IsRealizado().Should().BeFalse();
        transacao.Status.IsPendenteParaPrevisao().Should().BeFalse();
        transacao.Status.IsExcluido().Should().BeTrue();
    }
}
