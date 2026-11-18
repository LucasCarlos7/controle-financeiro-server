using ControleFinanceiro.Api.Contracts.Orcamentos;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Orcamentos;
using ControleFinanceiro.Application.Orcamentos.Commands.CreateOrcamento;
using ControleFinanceiro.Application.Orcamentos.Commands.DeleteOrcamento;
using ControleFinanceiro.Application.Orcamentos.Commands.UpdateOrcamento;
using ControleFinanceiro.Application.Orcamentos.Queries.GetOrcamentos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

public sealed class OrcamentosController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrcamentoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrcamentos(
        [FromQuery] int ano, [FromQuery] int mes, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetOrcamentosQuery { UserId = currentUser.UserId, Ano = ano, Mes = mes }, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrcamento(
        CreateOrcamentoRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrcamentoCommand
        {
            UserId = currentUser.UserId,
            CategoriaId = request.CategoriaId,
            Ano = request.Ano,
            Mes = request.Mes,
            LimiteValor = request.LimiteValor,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOrcamentos), new { ano = request.Ano, mes = request.Mes }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrcamento(
        Guid id, UpdateOrcamentoRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateOrcamentoCommand { UserId = currentUser.UserId, Id = id, LimiteValor = request.LimiteValor }, cancellationToken);

        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrcamento(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteOrcamentoCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}
