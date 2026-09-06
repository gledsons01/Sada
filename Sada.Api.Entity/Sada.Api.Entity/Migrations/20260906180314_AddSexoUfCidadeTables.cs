using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sada.Api.Entity.Migrations
{
    /// <inheritdoc />
    public partial class AddSexoUfCidadeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "TBL_SEXO",
                schema: "dbo",
                columns: table => new
                {
                    ID_SEXO = table.Column<int>(type: "int", nullable: false),
                    DESCRICAO = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    SIGLA = table.Column<string>(type: "char(2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_SEXO", x => x.ID_SEXO);
                });

            migrationBuilder.CreateTable(
                name: "TBL_UF",
                schema: "dbo",
                columns: table => new
                {
                    ID_UF = table.Column<int>(type: "int", nullable: false),
                    SIGLA_UF = table.Column<string>(type: "char(2)", nullable: true),
                    DESCRICAO_IF = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_UF", x => x.ID_UF);
                });

            migrationBuilder.CreateTable(
                name: "TBL_CIDADE",
                schema: "dbo",
                columns: table => new
                {
                    ID_CIDADE = table.Column<int>(type: "int", nullable: false),
                    ID_UF = table.Column<int>(type: "int", nullable: false),
                    DESCRICAO_CIDADE = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_CIDADE", x => x.ID_CIDADE);
                    table.ForeignKey(
                        name: "FK_TBL_CIDADE_TBL_UF",
                        column: x => x.ID_UF,
                        principalSchema: "dbo",
                        principalTable: "TBL_UF",
                        principalColumn: "ID_UF",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBL_CIDADE_ID_UF",
                schema: "dbo",
                table: "TBL_CIDADE",
                column: "ID_UF");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_CIDADE",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TBL_SEXO",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TBL_UF",
                schema: "dbo");
        }
    }
}
