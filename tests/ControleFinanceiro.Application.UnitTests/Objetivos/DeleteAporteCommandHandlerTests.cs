using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Objetivos.Commands.DeleteAporte;
using ControleFinanceiro.Domain.Objetivos;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Objetivos;

public class DeleteAporteCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<Aporte> _aportesDbSet = Substitute.For<DbSet<Aporte>, IQueryable<Aporte>>();

    public DeleteAporteCommandHandlerTests() => _dbContext.Aportes.Returns(_aportesDbSet);

    [Fact]
    public async Task Handle_Should_ReturnAporteNaoEncontrado_When_AporteNaoExiste()
    {
        _aportesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Aporte?>(null));

        var handler = new DeleteAporteCommandHandler(_dbContext);
        var command = new DeleteAporteCommand { UserId = Guid.NewGuid(), ObjetivoId = Guid.NewGuid(), Id = Guid.NewGuid() };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Error.Should().Be(ObjetivoErrors.AporteNaoEncontrado);
    }

    [Fact]
    public async Task Handle_Should_ReturnAporteNaoEncontrado_When_AporteEhDeOutroObjetivo()
    {
        var aporte = Aporte.Create(Guid.NewGuid(), 100m, new DateOnly(2026, 10, 1)).Value;
        _aportesDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Aporte?>(aporte));

        var handler = new DeleteAporteCommandHandler(_dbContext);
        var command = new DeleteAporteCommand { UserId = Guid.NewGuid(), ObjetivoId = Guid.NewGuid(), Id = aporte.Id };

        var result = await handler.Handle(command, CancellationToken.None);

        result.Error.Should().Be(ObjetivoErrors.AporteNaoEncontrado);
        _aportesDbSet.DidNotReceive().Remove(Arg.Any<Aporte>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
