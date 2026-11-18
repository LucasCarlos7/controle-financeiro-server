using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Usuarios;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Email == emailNormalizado, cancellationToken);

        if (usuario is null || !passwordHasher.Verify(request.Senha, usuario.SenhaHash))
            return Result.Failure<AuthResponse>(UsuarioErrors.CredenciaisInvalidas);

        var token = jwtTokenGenerator.GenerateToken(usuario);

        return Result.Success(new AuthResponse
        {
            UserId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Token = token,
        });
    }
}
