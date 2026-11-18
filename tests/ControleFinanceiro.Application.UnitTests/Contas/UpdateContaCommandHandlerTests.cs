using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Contas.Commands.UpdateConta;
using ControleFinanceiro.Domain.Contas;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Contas;

public class UpdateContaCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<Conta> _contasDbSet = Substitute.For<DbSet<Conta>, IQueryable<Conta>>();

    public UpdateContaCommandHandlerTests() => _dbContext.Contas.Returns(_contasDbSet);

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ContaNaoEncontrada()
    {
        _contasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Conta?>(null));

        var handler = new UpdateContaCommandHandler(_dbContext);
        var command = new UpdateContaCommand { UserId = Guid.NewGuid(), Id = Guid.NewGuid(), Nome = "Novo nome" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContaErrors.NaoEncontrada);
    }

    [Fact]
    public async Task Handle_Should_AtualizarNomeEIconeESalvar_When_ContaEncontrada()
    {
        var userId = Guid.NewGuid();
        var conta = Conta.Create(userId, "Nome antigo", TipoConta.Corrente, 0m).Value;
        _contasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Conta?>(conta));

        var handler = new UpdateContaCommandHandler(_dbContext);
        var command = new UpdateContaCommand { UserId = userId, Id = conta.Id, Nome = "Nome novo", Icone = "wallet" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        conta.Nome.Should().Be("Nome novo");
        conta.Icone.Should().Be("wallet");
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNaoEncontrada_When_ContaPertenceAOutroUsuario()
    {
        var conta = Conta.Create(Guid.NewGuid(), "Nome original", TipoConta.Corrente, 0m).Value;
        _contasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Conta?>(conta));

        var handler = new UpdateContaCommandHandler(_dbContext);
        var command = new UpdateContaCommand { UserId = Guid.NewGuid(), Id = conta.Id, Nome = "Invadido" };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContaErrors.NaoEncontrada);
        conta.Nome.Should().Be("Nome original");
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
