using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Contas.Commands.CreateCartaoCredito;
using ControleFinanceiro.Domain.Contas;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Contas;

public class CreateCartaoCreditoCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<CartaoCredito> _cartoesDbSet = Substitute.For<DbSet<CartaoCredito>, IQueryable<CartaoCredito>>();

    public CreateCartaoCreditoCommandHandlerTests() => _dbContext.CartoesCredito.Returns(_cartoesDbSet);

    [Fact]
    public async Task Handle_Should_AdicionarCartaoESalvar_When_SemContaPagamento()
    {
        var handler = new CreateCartaoCreditoCommandHandler(_dbContext);
        var command = new CreateCartaoCreditoCommand
        {
            UserId = Guid.NewGuid(),
            Nome = "Nubank",
            Limite = 5000m,
            DiaFechamento = 10,
            DiaVencimento = 17,
            ContaPagamentoId = null,
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _cartoesDbSet.Received(1).Add(Arg.Is<CartaoCredito>(c => c.Nome == "Nubank"));
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_LimiteInvalido()
    {
        var handler = new CreateCartaoCreditoCommandHandler(_dbContext);
        var command = new CreateCartaoCreditoCommand
        {
            UserId = Guid.NewGuid(),
            Nome = "Nubank",
            Limite = 0m,
            DiaFechamento = 10,
            DiaVencimento = 17,
            ContaPagamentoId = null,
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CartaoCreditoErrors.LimiteInvalido);
        _cartoesDbSet.DidNotReceive().Add(Arg.Any<CartaoCredito>());
    }
}
