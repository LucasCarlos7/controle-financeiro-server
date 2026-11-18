using ControleFinanceiro.Application.Abstractions;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Usuarios;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Application.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();

        var emailEmUso = await dbContext.Usuarios
            .AnyAsync(u => u.Email == emailNormalizado, cancellationToken);

        if (emailEmUso)
            return Result.Failure<AuthResponse>(UsuarioErrors.EmailJaCadastrado);

        var senhaHash = passwordHasher.Hash(request.Senha);

        var result = Usuario.Create(request.Nome, request.Email, senhaHash);
        if (result.IsFailure)
            return Result.Failure<AuthResponse>(result.Error);

        dbContext.Usuarios.Add(result.Value);
        dbContext.Categorias.AddRange(DadosIniciaisUsuario.CriarCategorias(result.Value.Id));
        dbContext.Tags.AddRange(DadosIniciaisUsuario.CriarTags(result.Value.Id));
        await dbContext.SaveChangesAsync(cancellationToken);

        var token = jwtTokenGenerator.GenerateToken(result.Value);

        return Result.Success(new AuthResponse
        {
            UserId = result.Value.Id,
            Nome = result.Value.Nome,
            Email = result.Value.Email,
            Token = token,
        });
    }
}
