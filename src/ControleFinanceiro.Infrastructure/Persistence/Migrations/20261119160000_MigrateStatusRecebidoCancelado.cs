using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigrateStatusRecebidoCancelado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // O status é armazenado como string (varchar 20) sem check constraint — apenas
            // precisamos fazer a migração de dados: receitas que estavam marcadas como 'Pago'
            // passam para 'Recebido', que é o status semântico correto para receitas confirmadas.
            migrationBuilder.Sql(@"
                UPDATE transacoes
                SET status = 'Recebido'
                WHERE tipo = 'Receita'
                  AND status = 'Pago';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE transacoes
                SET status = 'Pago'
                WHERE tipo = 'Receita'
                  AND status = 'Recebido';
            ");
        }
    }
}
