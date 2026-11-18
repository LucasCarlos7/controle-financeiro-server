using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Contas.Commands.CreateConta;
using ControleFinanceiro.Domain.Contas;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Contas;

public class CreateContaCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<Conta> _contasDbSet = Substitute.For<DbSet<Conta>, IQueryable<Conta>>();

    public CreateContaCommandHandlerTests() => _dbContext.Contas.Returns(_contasDbSet);

    [Fact]
    public async Task Handle_Should_AdicionarContaESalvar_When_DadosValidos()
    {
        var handler = new CreateContaCommandHandler(_dbContext);
        var command = new CreateContaCommand
        {
            UserId = Guid.NewGuid(),
            Nome = "Conta Corrente",
            Tipo = TipoConta.Corrente,
            SaldoInicial = 100m,
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _contasDbSet.Received(1).Add(Arg.Is<Conta>(c => c.Nome == "Conta Corrente"));
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NomeInvalido()
    {
        var handler = new CreateContaCommandHandler(_dbContext);
        var command = new CreateContaCommand
        {
            UserId = Guid.NewGuid(),
            Nome = string.Empty,
            Tipo = TipoConta.Corrente,
            SaldoInicial = 0m,
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ContaErrors.NomeObrigatorio);
        _contasDbSet.DidNotReceive().Add(Arg.Any<Conta>());
    }
}
