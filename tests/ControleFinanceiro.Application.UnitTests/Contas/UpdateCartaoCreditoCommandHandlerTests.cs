using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Contas.Commands.UpdateCartaoCredito;
using ControleFinanceiro.Domain.Contas;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Contas;

public class UpdateCartaoCreditoCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<CartaoCredito> _cartoesDbSet = Substitute.For<DbSet<CartaoCredito>, IQueryable<CartaoCredito>>();
    private readonly Guid _donoId = Guid.NewGuid();
    private readonly CartaoCredito _cartao;

    public UpdateCartaoCreditoCommandHandlerTests()
    {
        _dbContext.CartoesCredito.Returns(_cartoesDbSet);
        _cartao = CartaoCredito.Create(_donoId, "Nubank", 5000m, 10, 17, null).Value;
        _cartoesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<CartaoCredito?>(_cartao));
    }

    private UpdateCartaoCreditoCommand Command(Guid userId, decimal limite = 8000m) => new()
    {
        UserId = userId,
        Id = _cartao.Id,
        Nome = "Nubank Ultravioleta",
        Limite = limite,
        DiaFechamento = 3,
        DiaVencimento = 10,
    };

    [Fact]
    public async Task Handle_Should_AtualizarESalvar_When_UsuarioEhDono()
    {
        var result = await new UpdateCartaoCreditoCommandHandler(_dbContext).Handle(Command(_donoId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _cartao.Limite.Should().Be(8000m);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNaoEncontrado_When_CartaoPertenceAOutroUsuario()
    {
        var result = await new UpdateCartaoCreditoCommandHandler(_dbContext).Handle(Command(Guid.NewGuid()), CancellationToken.None);

        result.Error.Should().Be(CartaoCreditoErrors.NaoEncontrado);
        _cartao.Limite.Should().Be(5000m);
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_LimiteInvalido()
    {
        var result = await new UpdateCartaoCreditoCommandHandler(_dbContext).Handle(Command(_donoId, limite: 0m), CancellationToken.None);

        result.Error.Should().Be(CartaoCreditoErrors.LimiteInvalido);
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
