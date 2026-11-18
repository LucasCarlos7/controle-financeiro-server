using ControleFinanceiro.Api.Contracts.Objetivos;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Objetivos;
using ControleFinanceiro.Application.Objetivos.Commands.CreateAporte;
using ControleFinanceiro.Application.Objetivos.Commands.CreateObjetivo;
using ControleFinanceiro.Application.Objetivos.Commands.DeleteAporte;
using ControleFinanceiro.Application.Objetivos.Commands.DeleteObjetivo;
using ControleFinanceiro.Application.Objetivos.Commands.UpdateObjetivo;
using ControleFinanceiro.Application.Objetivos.Queries.GetObjetivoById;
using ControleFinanceiro.Application.Objetivos.Queries.GetObjetivos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

public sealed class ObjetivosController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ObjetivoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetObjetivos(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetObjetivosQuery { UserId = currentUser.UserId }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ObjetivoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetObjetivoById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetObjetivoByIdQuery { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateObjetivo(
        CreateObjetivoRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateObjetivoCommand
        {
            UserId = currentUser.UserId,
            Nome = request.Nome,
            ValorAlvo = request.ValorAlvo,
            DataAlvo = request.DataAlvo,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetObjetivoById), new { id = result.Value }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateObjetivo(
        Guid id, UpdateObjetivoRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateObjetivoCommand
        {
            UserId = currentUser.UserId,
            Id = id,
            Nome = request.Nome,
            ValorAlvo = request.ValorAlvo,
            DataAlvo = request.DataAlvo,
        };

        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteObjetivo(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteObjetivoCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/aportes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAporte(
        Guid id, CreateAporteRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateAporteCommand { UserId = currentUser.UserId, ObjetivoId = id, Valor = request.Valor, Data = request.Data };
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetObjetivoById), new { id }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpDelete("{id:guid}/aportes/{aporteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAporte(Guid id, Guid aporteId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteAporteCommand { UserId = currentUser.UserId, ObjetivoId = id, Id = aporteId }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}
