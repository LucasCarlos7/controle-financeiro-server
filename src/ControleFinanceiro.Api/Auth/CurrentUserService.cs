using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ControleFinanceiro.Application.Abstractions;

namespace ControleFinanceiro.Api.Auth;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var subject = httpContextAccessor.HttpContext?.User
                .FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (subject is null || !Guid.TryParse(subject, out var userId))
                throw new InvalidOperationException("Usuário autenticado não encontrado no contexto da requisição.");

            return userId;
        }
    }
}
