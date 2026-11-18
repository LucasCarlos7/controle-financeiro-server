using ControleFinanceiro.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleFailure(Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("HandleFailure não deve ser chamado para um resultado de sucesso.");

        var statusCode = result.Error.Code switch
        {
            "Validation.Failed" => StatusCodes.Status400BadRequest,
            "Usuario.CredenciaisInvalidas" => StatusCodes.Status401Unauthorized,
            "Usuario.EmailJaCadastrado" => StatusCodes.Status409Conflict,
            var code when code.EndsWith("NaoEncontrada", StringComparison.Ordinal)
                || code.EndsWith("NaoEncontrado", StringComparison.Ordinal) => StatusCodes.Status404NotFound,
            var code when code.EndsWith("JaExiste", StringComparison.Ordinal) => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status422UnprocessableEntity,
        };

        return Problem(
            statusCode: statusCode,
            title: result.Error.Code,
            detail: result.Error.Message);
    }
}
