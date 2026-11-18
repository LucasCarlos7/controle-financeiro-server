using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Objetivos.Commands.UpdateObjetivo;
using ControleFinanceiro.Domain.Objetivos;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ControleFinanceiro.Application.UnitTests.Objetivos;

public class UpdateObjetivoCommandHandlerTests
{
    private readonly IApplicationDbContext _dbContext = Substitute.For<IApplicationDbContext>();
    private readonly DbSet<Objetivo> _objetivosDbSet = Substitute.For<DbSet<Objetivo>, IQueryable<Objetivo>>();
    private readonly Guid _donoId = Guid.NewGuid();
    private readonly Objetivo _objetivo;

    public UpdateObjetivoCommandHandlerTests()
    {
        _dbContext.Objetivos.Returns(_objetivosDbSet);
        _objetivo = Objetivo.Create(_donoId, "Viagem", 5000m, new DateOnly(2027, 1, 31)).Value;
        _objetivosDbSet.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Objetivo?>(_objetivo));
    }

    private UpdateObjetivoCommand Command(Guid userId) => new()
    {
        UserId = userId,
        Id = _objetivo.Id,
        Nome = "Viagem para Floripa",
        ValorAlvo = 6000m,
        DataAlvo = new DateOnly(2027, 7, 15),
    };

    [Fact]
    public async Task Handle_Should_AtualizarESalvar_When_UsuarioEhDono()
    {
        var result = await new UpdateObjetivoCommandHandler(_dbContext).Handle(Command(_donoId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _objetivo.ValorAlvo.Should().Be(6000m);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnNaoEncontrado_When_ObjetivoPertenceAOutroUsuario()
    {
        var result = await new UpdateObjetivoCommandHandler(_dbContext).Handle(Command(Guid.NewGuid()), CancellationToken.None);

        result.Error.Should().Be(ObjetivoErrors.NaoEncontrado);
        _objetivo.Nome.Should().Be("Viagem");
        await _dbContext.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
