using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Dá aos usuários que já existiam as categorias e tags padrão que os novos recebem no cadastro
    /// (DadosIniciaisUsuario). A lista é uma cópia congelada de propósito: migration não pode mudar de
    /// comportamento quando o código evoluir. Quem já tem uma categoria de mesmo nome e tipo, ou uma tag de
    /// mesmo nome (sem diferenciar maiúsculas), não recebe a duplicata.
    /// </summary>
    public partial class SeedCategoriasETagsPadrao : Migration
    {
        private static readonly (string Nome, string Tipo, string Icone, string Cor, string[] Subcategorias)[] Categorias =
        [
            ("Moradia", "Despesa", "building", "#f97316", ["Aluguel", "Condomínio", "Energia", "Água", "Internet", "Manutenção"]),
            ("Alimentação", "Despesa", "food", "#ef4444", ["Mercado", "Restaurante", "Delivery", "Padaria"]),
            ("Transporte", "Despesa", "car", "#3b82f6", ["Combustível", "Transporte público", "Aplicativo", "Estacionamento", "Manutenção do veículo"]),
            ("Saúde", "Despesa", "heart", "#ec4899", ["Plano de saúde", "Farmácia", "Consultas", "Exames"]),
            ("Educação", "Despesa", "graduation", "#8b5cf6", ["Mensalidade", "Cursos", "Livros"]),
            ("Lazer", "Despesa", "film", "#eab308", ["Streaming", "Passeios", "Viagens", "Hobbies"]),
            ("Compras", "Despesa", "cart", "#14b8a6", ["Roupas", "Eletrônicos", "Casa e decoração"]),
            ("Cuidados pessoais", "Despesa", "scissors", "#d946ef", ["Salão e barbearia", "Cosméticos", "Academia"]),
            ("Pets", "Despesa", "paw", "#a16207", ["Ração", "Veterinário", "Banho e tosa"]),
            ("Impostos e tarifas", "Despesa", "receipt", "#64748b", ["Tarifas bancárias", "Juros", "Impostos"]),
            ("Presentes e doações", "Despesa", "gift", "#f43f5e", []),
            ("Outras despesas", "Despesa", null, "#94a3b8", []),
            ("Salário", "Receita", "briefcase", "#22c55e", ["Mensal", "13º salário", "Férias", "Bônus e PLR"]),
            ("Renda extra", "Receita", "coins", "#10b981", ["Freelance", "Vendas"]),
            ("Investimentos", "Receita", "chart-up", "#06b6d4", ["Rendimentos", "Dividendos"]),
            ("Reembolsos", "Receita", "cash", "#84cc16", []),
            ("Outras receitas", "Receita", null, "#94a3b8", []),
        ];

        private static readonly (string Nome, string Icone)[] Tags =
        [
            ("Essencial", "star"),
            ("Supérfluo", null),
            ("Trabalho", "briefcase"),
            ("Viagem", "plane"),
            ("Parcelado", "receipt"),
            ("Reembolsável", "cash"),
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var categoria in Categorias)
            {
                var subcategorias = categoria.Subcategorias.Length == 0
                    ? string.Empty
                    : $"""
                       INSERT INTO subcategorias (id, categoria_id, nome)
                       SELECT gen_random_uuid(), nova.id, s.nome
                       FROM nova CROSS JOIN (VALUES {string.Join(", ", categoria.Subcategorias.Select(s => $"({Literal(s)})"))}) AS s(nome);
                       """;

                migrationBuilder.Sql($"""
                    WITH nova AS (
                        INSERT INTO categorias (id, user_id, nome, tipo, icone, cor)
                        SELECT gen_random_uuid(), u.id, {Literal(categoria.Nome)}, {Literal(categoria.Tipo)}, {Literal(categoria.Icone)}, {Literal(categoria.Cor)}
                        FROM usuarios u
                        WHERE NOT EXISTS (
                            SELECT 1 FROM categorias c
                            WHERE c.user_id = u.id AND c.tipo = {Literal(categoria.Tipo)} AND lower(c.nome) = lower({Literal(categoria.Nome)}))
                        RETURNING id)
                    {(subcategorias.Length == 0 ? "SELECT count(*) FROM nova;" : subcategorias)}
                    """);
            }

            foreach (var tag in Tags)
            {
                migrationBuilder.Sql($"""
                    INSERT INTO tags (id, user_id, nome, icone)
                    SELECT gen_random_uuid(), u.id, {Literal(tag.Nome)}, {Literal(tag.Icone)}
                    FROM usuarios u
                    WHERE NOT EXISTS (
                        SELECT 1 FROM tags t WHERE t.user_id = u.id AND lower(t.nome) = lower({Literal(tag.Nome)}));
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Sem reversão: depois do Up as categorias e tags são dados do usuário (podem ter sido editadas ou
            // usadas em lançamentos e orçamentos), e não há como separá-las das que ele criou por conta própria.
        }

        private static string Literal(string valor) => valor is null ? "NULL" : $"'{valor.Replace("'", "''")}'";
    }
}
