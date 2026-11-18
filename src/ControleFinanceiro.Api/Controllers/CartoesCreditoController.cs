using ControleFinanceiro.Api.Contracts.Contas;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Contas;
using ControleFinanceiro.Application.Contas.Commands.CreateCartaoCredito;
using ControleFinanceiro.Application.Contas.Commands.DeactivateCartaoCredito;
using ControleFinanceiro.Application.Contas.Commands.ReactivateCartaoCredito;
using ControleFinanceiro.Application.Contas.Commands.UpdateCartaoCredito;
using ControleFinanceiro.Application.Contas.Queries.GetCartaoCreditoById;
using ControleFinanceiro.Application.Contas.Queries.GetCartoesCredito;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

[Route("api/v1/cartoes-credito")]
public sealed class CartoesCreditoController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CartaoCreditoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCartoesCredito(
        [FromQuery] bool apenasAtivos, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCartoesCreditoQuery { UserId = currentUser.UserId, ApenasAtivos = apenasAtivos }, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<CartaoCreditoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCartaoCreditoById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCartaoCreditoByIdQuery { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCartaoCredito(
        CreateCartaoCreditoRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCartaoCreditoCommand
        {
            UserId = currentUser.UserId,
            Nome = request.Nome,
            Limite = request.Limite,
            DiaFechamento = request.DiaFechamento,
            DiaVencimento = request.DiaVencimento,
            ContaPagamentoId = request.ContaPagamentoId,
            Icone = request.Icone,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCartaoCreditoById), new { id = result.Value }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCartaoCredito(
        Guid id, UpdateCartaoCreditoRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCartaoCreditoCommand
        {
            UserId = currentUser.UserId,
            Id = id,
            Nome = request.Nome,
            Limite = request.Limite,
            DiaFechamento = request.DiaFechamento,
            DiaVencimento = request.DiaVencimento,
            ContaPagamentoId = request.ContaPagamentoId,
            Icone = request.Icone,
        };

        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateCartaoCredito(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateCartaoCreditoCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/reativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactivateCartaoCredito(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ReactivateCartaoCreditoCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}
