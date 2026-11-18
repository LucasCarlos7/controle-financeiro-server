using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Transacoes.Commands.DeleteTransacao;
using ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPago;
using ControleFinanceiro.Application.Transacoes.Commands.UpdateTransacao;
using ControleFinanceiro.Domain.Transacoes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Transacoes;

public class TransacaoOwnershipTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<Transacao> _transacoesDbSet = Substitute.For<DbSet<Transacao>, IQueryable<Transacao>>();
    private readonly Guid _donoId = Guid.NewGuid();
    private readonly Transacao _transacao;

    public TransacaoOwnershipTests()
    {
        _dbContext.Transacoes.Returns(_transacoesDbSet);
        _transacao = Transacao.Create(
            _donoId, TipoTransacao.Despesa, 100m, new DateOnly(2026, 10, 5), "Mercado",
            Guid.NewGuid(), null, null, null, null, StatusTransacao.Pendente, null).Value;
        _transacoesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Transacao?>(_transacao));
    }

    private UpdateTransacaoCommand UpdateCommand(Guid userId) => new()
    {
        UserId = userId,
        Id = _transacao.Id,
        Valor = 250m,
        Data = new DateOnly(2026, 10, 6),
        Descricao = "Alterada",
    };

    [Fact]
    public async Task UpdateTransacao_Should_AtualizarESalvar_When_UsuarioEhDono()
    {
        var handler = new UpdateTransacaoCommandHandler(_dbContext);

        var result = await handler.Handle(UpdateCommand(_donoId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _transacao.Valor.Should().Be(250m);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTransacao_Should_ReturnNaoEncontrada_When_TransacaoPertenceAOutroUsuario()
    {
        var handler = new UpdateTransacaoCommandHandler(_dbContext);

        var result = await handler.Handle(UpdateCommand(Guid.NewGuid()), CancellationToken.None);

        result.Error.Should().Be(TransacaoErrors.NaoEncontrada);
        _transacao.Valor.Should().Be(100m);
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTransacao_Should_ReturnNaoEncontrada_When_TransacaoPertenceAOutroUsuario()
    {
        var handler = new DeleteTransacaoCommandHandler(_dbContext);

        var result = await handler.Handle(
            new DeleteTransacaoCommand { UserId = Guid.NewGuid(), Id = _transacao.Id }, CancellationToken.None);

        result.Error.Should().Be(TransacaoErrors.NaoEncontrada);
        _transacoesDbSet.DidNotReceive().Remove(Arg.Any<Transacao>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTransacao_Should_RemoverESalvar_When_UsuarioEhDono()
    {
        var handler = new DeleteTransacaoCommandHandler(_dbContext);

        var result = await handler.Handle(
            new DeleteTransacaoCommand { UserId = _donoId, Id = _transacao.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _transacoesDbSet.Received(1).Remove(_transacao);
    }

    [Fact]
    public async Task MarkTransacaoAsPago_Should_ReturnNaoEncontrada_When_TransacaoPertenceAOutroUsuario()
    {
        var handler = new MarkTransacaoAsPagoCommandHandler(_dbContext);

        var result = await handler.Handle(
            new MarkTransacaoAsPagoCommand { UserId = Guid.NewGuid(), Id = _transacao.Id }, CancellationToken.None);

        result.Error.Should().Be(TransacaoErrors.NaoEncontrada);
        _transacao.Status.Should().Be(StatusTransacao.Pendente);
    }
}
