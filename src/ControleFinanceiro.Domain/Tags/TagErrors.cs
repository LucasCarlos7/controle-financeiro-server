using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Tags;

public static class TagErrors
{
    public static readonly Error NomeObrigatorio =
        new("Tag.NomeObrigatorio", "O nome da tag é obrigatório.");

    public static readonly Error NaoEncontrada =
        new("Tag.NaoEncontrada", "Tag não encontrada.");

    public static readonly Error JaExiste =
        new("Tag.JaExiste", "Você já tem uma tag com este nome.");
}
