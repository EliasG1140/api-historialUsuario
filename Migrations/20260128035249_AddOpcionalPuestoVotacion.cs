using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddOpcionalPuestoVotacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_mesa_votacion_MesaVotacionId",
                table: "Personas");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Personas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "MesaVotacionId",
                table: "Personas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "VerfAdres",
                table: "Personas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VerfPuestoVotacion",
                table: "Personas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_mesa_votacion_MesaVotacionId",
                table: "Personas",
                column: "MesaVotacionId",
                principalTable: "mesa_votacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personas_mesa_votacion_MesaVotacionId",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "VerfAdres",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "VerfPuestoVotacion",
                table: "Personas");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Personas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MesaVotacionId",
                table: "Personas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Personas_mesa_votacion_MesaVotacionId",
                table: "Personas",
                column: "MesaVotacionId",
                principalTable: "mesa_votacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
