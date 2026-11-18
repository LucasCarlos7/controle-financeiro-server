using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Dashboard;
using ControleFinanceiro.Application.Dashboard.Queries.GetEvolucaoMensal;
using ControleFinanceiro.Application.Dashboard.Queries.GetGastosPorCategoria;
using ControleFinanceiro.Application.Dashboard.Queries.GetResumoMensal;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

public sealed class DashboardController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet("resumo-mensal")]
    [ProducesResponseType<ResumoMensalResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResumoMensal(
        [FromQuery] int ano, [FromQuery] int mes, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetResumoMensalQuery { UserId = currentUser.UserId, Ano = ano, Mes = mes }, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("gastos-por-categoria")]
    [ProducesResponseType<IReadOnlyList<GastoPorCategoriaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGastosPorCategoria(
        [FromQuery] int ano, [FromQuery] int mes, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetGastosPorCategoriaQuery { UserId = currentUser.UserId, Ano = ano, Mes = mes }, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("evolucao-mensal")]
    [ProducesResponseType<IReadOnlyList<EvolucaoMensalResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvolucaoMensal(
        [FromQuery] DateOnly? dataInicio, [FromQuery] DateOnly? dataFim, CancellationToken cancellationToken)
    {
        var fim = dataFim ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var inicio = dataInicio ?? new DateOnly(fim.Year, fim.Month, 1).AddMonths(-5);

        var result = await sender.Send(
            new GetEvolucaoMensalQuery { UserId = currentUser.UserId, DataInicio = inicio, DataFim = fim },
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }
}
