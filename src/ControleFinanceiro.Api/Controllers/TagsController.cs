using ControleFinanceiro.Api.Contracts.Tags;
using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Application.Tags;
using ControleFinanceiro.Application.Tags.Commands.CreateTag;
using ControleFinanceiro.Application.Tags.Commands.DeleteTag;
using ControleFinanceiro.Application.Tags.Commands.UpdateTag;
using ControleFinanceiro.Application.Tags.Queries.GetTags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

public sealed class TagsController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TagResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTags(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetTagsQuery { UserId = currentUser.UserId }, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTag(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand { UserId = currentUser.UserId, Nome = request.Nome, Icone = request.Icone };
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTags), new { }, new { id = result.Value })
            : HandleFailure(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTag(Guid id, UpdateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand { UserId = currentUser.UserId, Id = id, Nome = request.Nome, Icone = request.Icone };
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTag(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteTagCommand { UserId = currentUser.UserId, Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}
