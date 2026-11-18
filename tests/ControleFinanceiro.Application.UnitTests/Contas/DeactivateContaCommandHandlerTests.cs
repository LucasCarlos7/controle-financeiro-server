using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Contas.Commands.DeactivateConta;
using ControleFinanceiro.Domain.Contas;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Contas;

public class DeactivateContaCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<Conta> _contasDbSet = Substitute.For<DbSet<Conta>, IQueryable<Conta>>();

    public DeactivateContaCommandHandlerTests() => _dbContext.Contas.Returns(_contasDbSet);

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ContaNaoEncontrada()
    {
        _contasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Conta?>(null));

        var handler = new DeactivateContaCommandHandler(_dbContext);

        var result = await handler.Handle(
            new DeactivateContaCommand { UserId = Guid.NewGuid(), Id = Guid.NewGuid() }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContaErrors.NaoEncontrada);
    }

    [Fact]
    public async Task Handle_Should_DesativarContaESalvar_When_ContaEncontrada()
    {
        var userId = Guid.NewGuid();
        var conta = Conta.Create(userId, "Conta Corrente", TipoConta.Corrente, 0m).Value;
        _contasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Conta?>(conta));

        var handler = new DeactivateContaCommandHandler(_dbContext);

        var result = await handler.Handle(
            new DeactivateContaCommand { UserId = userId, Id = conta.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        conta.Ativo.Should().BeFalse();
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNaoEncontrada_When_ContaPertenceAOutroUsuario()
    {
        var conta = Conta.Create(Guid.NewGuid(), "Conta de outra pessoa", TipoConta.Corrente, 0m).Value;
        _contasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Conta?>(conta));

        var handler = new DeactivateContaCommandHandler(_dbContext);

        var result = await handler.Handle(
            new DeactivateContaCommand { UserId = Guid.NewGuid(), Id = conta.Id }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContaErrors.NaoEncontrada);
        conta.Ativo.Should().BeTrue();
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
