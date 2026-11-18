using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    icone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    saldo_inicial = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recorrencias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    intervalo_meses = table.Column<int>(type: "integer", nullable: true),
                    total_parcelas = table.Column<int>(type: "integer", nullable: true),
                    parcela_atual = table.Column<int>(type: "integer", nullable: true),
                    data_fim = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recorrencias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subcategorias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subcategorias", x => x.id);
                    table.ForeignKey(
                        name: "FK_subcategorias_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cartoes_credito",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    limite = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    dia_fechamento = table.Column<int>(type: "integer", nullable: false),
                    dia_vencimento = table.Column<int>(type: "integer", nullable: false),
                    conta_pagamento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cartoes_credito", x => x.id);
                    table.ForeignKey(
                        name: "FK_cartoes_credito_contas_conta_pagamento_id",
                        column: x => x.conta_pagamento_id,
                        principalTable: "contas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "transacoes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    conta_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cartao_credito_id = table.Column<Guid>(type: "uuid", nullable: true),
                    conta_destino_id = table.Column<Guid>(type: "uuid", nullable: true),
                    categoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subcategoria_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    recorrencia_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tag_ids = table.Column<List<Guid>>(type: "uuid[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transacoes", x => x.id);
                    table.ForeignKey(
                        name: "FK_transacoes_cartoes_credito_cartao_credito_id",
                        column: x => x.cartao_credito_id,
                        principalTable: "cartoes_credito",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transacoes_categorias_categoria_id",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transacoes_contas_conta_destino_id",
                        column: x => x.conta_destino_id,
                        principalTable: "contas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transacoes_contas_conta_id",
                        column: x => x.conta_id,
                        principalTable: "contas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_transacoes_recorrencias_recorrencia_id",
                        column: x => x.recorrencia_id,
                        principalTable: "recorrencias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_transacoes_subcategorias_subcategoria_id",
                        column: x => x.subcategoria_id,
                        principalTable: "subcategorias",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cartoes_credito_conta_pagamento_id",
                table: "cartoes_credito",
                column: "conta_pagamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_cartoes_credito_user_id",
                table: "cartoes_credito",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_categorias_user_id",
                table: "categorias",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_contas_user_id",
                table: "contas",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_subcategorias_categoria_id",
                table: "subcategorias",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_tags_user_id_nome",
                table: "tags",
                columns: new[] { "user_id", "nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_cartao_credito_id",
                table: "transacoes",
                column: "cartao_credito_id");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_categoria_id",
                table: "transacoes",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_conta_destino_id",
                table: "transacoes",
                column: "conta_destino_id");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_conta_id",
                table: "transacoes",
                column: "conta_id");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_recorrencia_id",
                table: "transacoes",
                column: "recorrencia_id");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_subcategoria_id",
                table: "transacoes",
                column: "subcategoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_user_id_data",
                table: "transacoes",
                columns: new[] { "user_id", "data" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "transacoes");

            migrationBuilder.DropTable(
                name: "cartoes_credito");

            migrationBuilder.DropTable(
                name: "recorrencias");

            migrationBuilder.DropTable(
                name: "subcategorias");

            migrationBuilder.DropTable(
                name: "contas");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
