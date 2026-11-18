using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Transacoes.Commands.DeleteRecorrencia;
using ControleFinanceiro.Domain.Transacoes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Transacoes;

public class DeleteRecorrenciaCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<Recorrencia> _recorrenciasDbSet = Substitute.For<DbSet<Recorrencia>, IQueryable<Recorrencia>>();

    public DeleteRecorrenciaCommandHandlerTests() => _dbContext.Recorrencias.Returns(_recorrenciasDbSet);

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RecorrenciaNaoEncontrada()
    {
        _recorrenciasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Recorrencia?>(null));

        var handler = new DeleteRecorrenciaCommandHandler(_dbContext);

        var result = await handler.Handle(
            new DeleteRecorrenciaCommand { UserId = Guid.NewGuid(), Id = Guid.NewGuid() }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecorrenciaErrors.NaoEncontrada);
    }

    [Fact]
    public async Task Handle_Should_ReturnNaoEncontrada_When_RecorrenciaPertenceAOutroUsuario()
    {
        var recorrencia = Recorrencia.Create(Guid.NewGuid(), TipoRecorrencia.Mensal, 1, null, null, null).Value;
        _recorrenciasDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Recorrencia?>(recorrencia));

        var handler = new DeleteRecorrenciaCommandHandler(_dbContext);

        var result = await handler.Handle(
            new DeleteRecorrenciaCommand { UserId = Guid.NewGuid(), Id = recorrencia.Id }, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecorrenciaErrors.NaoEncontrada);
        _recorrenciasDbSet.DidNotReceive().Remove(Arg.Any<Recorrencia>());
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
