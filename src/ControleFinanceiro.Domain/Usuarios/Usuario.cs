using System.Text.RegularExpressions;
using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Usuarios;

public sealed partial class Usuario : Entity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Usuario(Guid id, string nome, string email, string senhaHash, DateTime criadoEm) : base(id)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        CriadoEm = criadoEm;
    }

    public static Result<Usuario> Create(string nome, string email, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Usuario>(UsuarioErrors.NomeObrigatorio);

        if (string.IsNullOrWhiteSpace(email) || !EmailRegex().IsMatch(email))
            return Result.Failure<Usuario>(UsuarioErrors.EmailInvalido);

        if (string.IsNullOrWhiteSpace(senhaHash))
            return Result.Failure<Usuario>(UsuarioErrors.SenhaObrigatoria);

        return Result.Success(new Usuario(
            Guid.NewGuid(), nome.Trim(), email.Trim().ToLowerInvariant(), senhaHash, DateTime.UtcNow));
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
