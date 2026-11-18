using ControleFinanceiro.Api.Contracts.Categorias;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Categorias;
using ControleFinanceiro.Application.Categorias.Commands.CreateCategoria;
using ControleFinanceiro.Application.Categorias.Commands.CreateSubcategoria;
using ControleFinanceiro.Application.Categorias.Commands.DeleteCategoria;
using ControleFinanceiro.Application.Categorias.Commands.DeleteSubcategoria;
using ControleFinanceiro.Application.Categorias.Commands.UpdateCategoria;
using ControleFinanceiro.Application.Categorias.Commands.UpdateSubcategoria;
using ControleFinanceiro.Application.Categorias.Queries.GetCategoriaById;
using ControleFinanceiro.Application.Categorias.Queries.GetCategorias;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

public sealed class CategoriasController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CategoriaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategorias(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriasQuery { UserId = currentUser.UserId }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<CategoriaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoriaById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriaByIdQuery { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategoria(
        CreateCategoriaRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoriaCommand
        {
            UserId = currentUser.UserId,
            Nome = request.Nome,
            Tipo = request.Tipo,
            Icone = request.Icone,
            Cor = request.Cor,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCategoriaById), new { id = result.Value }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategoria(
        Guid id, UpdateCategoriaRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCategoriaCommand
        {
            UserId = currentUser.UserId,
            Id = id,
            Nome = request.Nome,
            Icone = request.Icone,
            Cor = request.Cor,
        };

        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategoria(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCategoriaCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpPost("{id:guid}/subcategorias")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSubcategoria(
        Guid id, SubcategoriaRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateSubcategoriaCommand { UserId = currentUser.UserId, CategoriaId = id, Nome = request.Nome }, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCategoriaById), new { id }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPut("{id:guid}/subcategorias/{subcategoriaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubcategoria(
        Guid id, Guid subcategoriaId, SubcategoriaRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSubcategoriaCommand
        {
            UserId = currentUser.UserId,
            CategoriaId = id,
            SubcategoriaId = subcategoriaId,
            Nome = request.Nome,
        };

        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:guid}/subcategorias/{subcategoriaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubcategoria(
        Guid id, Guid subcategoriaId, CancellationToken cancellationToken)
    {
        var command = new DeleteSubcategoriaCommand { UserId = currentUser.UserId, CategoriaId = id, SubcategoriaId = subcategoriaId };
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}
