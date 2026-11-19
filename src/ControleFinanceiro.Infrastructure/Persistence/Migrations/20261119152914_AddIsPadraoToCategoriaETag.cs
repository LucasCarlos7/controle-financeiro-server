using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPadraoToCategoriaETag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_padrao",
                table: "tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_padrao",
                table: "categorias",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Marcar categorias e tags criadas pelo seeder como padrão em usuários existentes.
            migrationBuilder.Sql(@"
                UPDATE categorias SET is_padrao = true
                WHERE nome IN (
                    'Moradia','Alimentação','Transporte','Saúde','Educação','Lazer',
                    'Compras','Cuidados pessoais','Pets','Impostos e tarifas',
                    'Presentes e doações','Outras despesas','Salário','Renda extra',
                    'Investimentos','Reembolsos','Outras receitas'
                );
                UPDATE tags SET is_padrao = true
                WHERE nome IN ('Essencial','Supérfluo','Trabalho','Viagem','Parcelado','Reembolsável');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_padrao",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "is_padrao",
                table: "categorias");
        }
    }
}
