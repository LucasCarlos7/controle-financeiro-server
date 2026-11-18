using ControleFinanceiro.Domain.Usuarios;

namespace ControleFinanceiro.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateToken(Usuario usuario);
}
