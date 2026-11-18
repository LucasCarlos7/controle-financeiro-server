using ControleFinanceiro.Domain.Categorias;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Tags;

namespace ControleFinanceiro.Application.Auth.Commands.Register;

/// <summary>
/// Categorias e tags com que todo usuário começa. Depois de criadas são registros comuns do usuário:
/// podem ser editadas e removidas. Os ícones são chaves do catálogo do front-end (iconCatalog.tsx).
/// </summary>
/// <remarks>
/// A migration SeedCategoriasETagsPadrao tem uma cópia congelada desta lista para os usuários que já existiam;
/// mudar a lista aqui afeta só quem se cadastrar depois.
/// </remarks>
public static class DadosIniciaisUsuario
{
    private sealed record CategoriaPadrao(string Nome, TipoCategoria Tipo, string? Icone, string Cor, string[] Subcategorias);

    private sealed record TagPadrao(string Nome, string? Icone);

    private static readonly CategoriaPadrao[] CategoriasPadrao =
    [
        new("Moradia", TipoCategoria.Despesa, "building", "#f97316", ["Aluguel", "Condomínio", "Energia", "Água", "Internet", "Manutenção"]),
        new("Alimentação", TipoCategoria.Despesa, "food", "#ef4444", ["Mercado", "Restaurante", "Delivery", "Padaria"]),
        new("Transporte", TipoCategoria.Despesa, "car", "#3b82f6", ["Combustível", "Transporte público", "Aplicativo", "Estacionamento", "Manutenção do veículo"]),
        new("Saúde", TipoCategoria.Despesa, "heart", "#ec4899", ["Plano de saúde", "Farmácia", "Consultas", "Exames"]),
        new("Educação", TipoCategoria.Despesa, "graduation", "#8b5cf6", ["Mensalidade", "Cursos", "Livros"]),
        new("Lazer", TipoCategoria.Despesa, "film", "#eab308", ["Streaming", "Passeios", "Viagens", "Hobbies"]),
        new("Compras", TipoCategoria.Despesa, "cart", "#14b8a6", ["Roupas", "Eletrônicos", "Casa e decoração"]),
        new("Cuidados pessoais", TipoCategoria.Despesa, "scissors", "#d946ef", ["Salão e barbearia", "Cosméticos", "Academia"]),
        new("Pets", TipoCategoria.Despesa, "paw", "#a16207", ["Ração", "Veterinário", "Banho e tosa"]),
        new("Impostos e tarifas", TipoCategoria.Despesa, "receipt", "#64748b", ["Tarifas bancárias", "Juros", "Impostos"]),
        new("Presentes e doações", TipoCategoria.Despesa, "gift", "#f43f5e", []),
        new("Outras despesas", TipoCategoria.Despesa, null, "#94a3b8", []),
        new("Salário", TipoCategoria.Receita, "briefcase", "#22c55e", ["Mensal", "13º salário", "Férias", "Bônus e PLR"]),
        new("Renda extra", TipoCategoria.Receita, "coins", "#10b981", ["Freelance", "Vendas"]),
        new("Investimentos", TipoCategoria.Receita, "chart-up", "#06b6d4", ["Rendimentos", "Dividendos"]),
        new("Reembolsos", TipoCategoria.Receita, "cash", "#84cc16", []),
        new("Outras receitas", TipoCategoria.Receita, null, "#94a3b8", []),
    ];

    private static readonly TagPadrao[] TagsPadrao =
    [
        new("Essencial", "star"),
        new("Supérfluo", null),
        new("Trabalho", "briefcase"),
        new("Viagem", "plane"),
        new("Parcelado", "receipt"),
        new("Reembolsável", "cash"),
    ];

    public static IReadOnlyList<Categoria> CriarCategorias(Guid userId) =>
        CategoriasPadrao.Select(padrao =>
        {
            var categoria = Garantir(Categoria.Create(userId, padrao.Nome, padrao.Tipo, padrao.Icone, padrao.Cor));
            foreach (var subcategoria in padrao.Subcategorias)
                Garantir(categoria.AdicionarSubcategoria(subcategoria));

            return categoria;
        }).ToList();

    public static IReadOnlyList<Tag> CriarTags(Guid userId) =>
        TagsPadrao.Select(padrao => Garantir(Tag.Create(userId, padrao.Nome, padrao.Icone))).ToList();

    // Os dados acima são constantes: uma falha aqui é bug na lista, não erro de entrada do usuário.
    private static T Garantir<T>(Result<T> result) => result.IsSuccess
        ? result.Value
        : throw new InvalidOperationException($"Dado inicial inválido: {result.Error.Code}");
}
