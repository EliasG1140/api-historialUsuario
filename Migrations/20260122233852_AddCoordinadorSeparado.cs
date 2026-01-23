using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinadorSeparado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_CoordinadorId",
                table: "Personas");

            migrationBuilder.AddColumn<bool>(
                name: "IsCoordinador",
                table: "Personas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_CoordinadorId",
                table: "Personas",
                column: "CoordinadorId",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_CoordinadorId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "IsCoordinador",
                table: "Personas");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_CoordinadorId",
                table: "Personas",
                column: "CoordinadorId",
                principalTable: "Personas",
                principalColumn: "Id");
        }
    }
}
