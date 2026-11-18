using ControleFinanceiro.Api.Contracts.Contas;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Contas;
using ControleFinanceiro.Application.Contas.Commands.CreateConta;
using ControleFinanceiro.Application.Contas.Commands.DeactivateConta;
using ControleFinanceiro.Application.Contas.Commands.ReactivateConta;
using ControleFinanceiro.Application.Contas.Commands.UpdateConta;
using ControleFinanceiro.Application.Contas.Queries.GetContaById;
using ControleFinanceiro.Application.Contas.Queries.GetContas;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

public sealed class ContasController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ContaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContas(
        [FromQuery] bool apenasAtivas, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetContasQuery { UserId = currentUser.UserId, ApenasAtivas = apenasAtivas }, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ContaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContaById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetContaByIdQuery { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateConta(CreateContaRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateContaCommand
        {
            UserId = currentUser.UserId,
            Nome = request.Nome,
            Tipo = request.Tipo,
            SaldoInicial = request.SaldoInicial,
            Icone = request.Icone,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetContaById), new { id = result.Value }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateConta(
        Guid id, UpdateContaRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateContaCommand { UserId = currentUser.UserId, Id = id, Nome = request.Nome, Icone = request.Icone }, cancellationToken);

        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateConta(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateContaCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/reativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactivateConta(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ReactivateContaCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}
