using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Usuarios;

public static class UsuarioErrors
{
    public static readonly Error NomeObrigatorio =
        new("Usuario.NomeObrigatorio", "O nome é obrigatório.");

    public static readonly Error EmailInvalido =
        new("Usuario.EmailInvalido", "O e-mail informado é inválido.");

    public static readonly Error SenhaObrigatoria =
        new("Usuario.SenhaObrigatoria", "A senha é obrigatória.");

    public static readonly Error EmailJaCadastrado =
        new("Usuario.EmailJaCadastrado", "Já existe uma conta com este e-mail.");

    public static readonly Error CredenciaisInvalidas =
        new("Usuario.CredenciaisInvalidas", "E-mail ou senha inválidos.");
}
