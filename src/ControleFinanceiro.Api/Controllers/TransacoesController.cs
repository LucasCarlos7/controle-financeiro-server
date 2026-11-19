using ControleFinanceiro.Api.Contracts.Transacoes;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Transacoes;
using ControleFinanceiro.Application.Transacoes.Commands.CreateTransacao;
using ControleFinanceiro.Application.Transacoes.Commands.CreateTransacaoRecorrente;
using ControleFinanceiro.Application.Transacoes.Commands.DeleteRecorrencia;
using ControleFinanceiro.Application.Transacoes.Commands.DeleteTransacao;
using ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPago;
using ControleFinanceiro.Application.Transacoes.Commands.MarkTransacaoAsPendente;
using ControleFinanceiro.Application.Transacoes.Commands.UpdateTransacao;
using ControleFinanceiro.Application.Transacoes.Queries.GetTransacaoById;
using ControleFinanceiro.Application.Transacoes.Queries.GetTransacoes;
using ControleFinanceiro.Domain.Transacoes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

public sealed class TransacoesController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TransacaoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransacoes(
        [FromQuery] DateOnly? dataInicio,
        [FromQuery] DateOnly? dataFim,
        [FromQuery] Guid? contaId,
        [FromQuery] Guid? cartaoCreditoId,
        [FromQuery] Guid? categoriaId,
        [FromQuery] Guid? tagId,
        [FromQuery] TipoTransacao? tipo,
        [FromQuery] StatusTransacao? status,
        [FromQuery] bool? apenasRecorrentes,
        [FromQuery] string? busca,
        CancellationToken cancellationToken)
    {
        var query = new GetTransacoesQuery
        {
            UserId = currentUser.UserId,
            DataInicio = dataInicio,
            DataFim = dataFim,
            ContaId = contaId,
            CartaoCreditoId = cartaoCreditoId,
            CategoriaId = categoriaId,
            TagId = tagId,
            Tipo = tipo,
            Status = status,
            ApenasRecorrentes = apenasRecorrentes,
            Busca = busca,
        };

        var result = await sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<TransacaoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransacaoById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTransacaoByIdQuery { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTransacao(
        CreateTransacaoRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTransacaoCommand
        {
            UserId = currentUser.UserId,
            Tipo = request.Tipo,
            Valor = request.Valor,
            Data = request.Data,
            Descricao = request.Descricao,
            ContaId = request.ContaId,
            CartaoCreditoId = request.CartaoCreditoId,
            ContaDestinoId = request.ContaDestinoId,
            CategoriaId = request.CategoriaId,
            SubcategoriaId = request.SubcategoriaId,
            Status = request.Status,
            TagIds = request.TagIds,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTransacaoById), new { id = result.Value }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPost("recorrente")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTransacaoRecorrente(
        CreateTransacaoRecorrenteRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTransacaoRecorrenteCommand
        {
            UserId = currentUser.UserId,
            Tipo = request.Tipo,
            Valor = request.Valor,
            DataInicio = request.DataInicio,
            Descricao = request.Descricao,
            ContaId = request.ContaId,
            CartaoCreditoId = request.CartaoCreditoId,
            ContaDestinoId = request.ContaDestinoId,
            CategoriaId = request.CategoriaId,
            SubcategoriaId = request.SubcategoriaId,
            TagIds = request.TagIds,
            TipoRecorrencia = request.TipoRecorrencia,
            IntervaloMeses = request.IntervaloMeses,
            TotalParcelas = request.TotalParcelas,
            DataFim = request.DataFim,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTransacoes), null, new { recorrenciaId = result.Value })
            : HandleFailure(result);
    }

    [HttpDelete("recorrencias/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecorrencia(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteRecorrenciaCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTransacao(
        Guid id, UpdateTransacaoRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTransacaoCommand
        {
            UserId = currentUser.UserId,
            Id = id,
            Valor = request.Valor,
            Data = request.Data,
            Descricao = request.Descricao,
            CategoriaId = request.CategoriaId,
            SubcategoriaId = request.SubcategoriaId,
            TagIds = request.TagIds,
            EscopoEdicao = request.EscopoEdicao,
        };

        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransacao(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTransacaoCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/marcar-pago")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsPago(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new MarkTransacaoAsPagoCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    // Alias explícito para receitas — internamente chama o mesmo comando
    [HttpPost("{id:guid}/marcar-recebido")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRecebido(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new MarkTransacaoAsPagoCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/marcar-pendente")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsPendente(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new MarkTransacaoAsPendenteCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}
