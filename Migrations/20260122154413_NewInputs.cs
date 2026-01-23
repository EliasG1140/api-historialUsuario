using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class NewInputs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoordinadorId",
                table: "Personas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Familia",
                table: "Personas",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personas_CoordinadorId",
                table: "Personas",
                column: "CoordinadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_Personas_CoordinadorId",
                table: "Personas",
                column: "CoordinadorId",
                principalTable: "Personas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_Personas_CoordinadorId",
                table: "Personas");

            migrationBuilder.DropIndex(
                name: "IX_Personas_CoordinadorId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "CoordinadorId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "Familia",
                table: "Personas");
        }
    }
}
