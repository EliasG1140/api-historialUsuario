using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "barrio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barrio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Minimo = table.Column<int>(type: "integer", nullable: false),
                    Maximo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "codigo_b",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codigo_b", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "codigo_c",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codigo_c", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lengua",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lengua", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "puesto_votacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_puesto_votacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mesa_votacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PuestoVotacionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mesa_votacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mesa_votacion_puesto_votacion_PuestoVotacionId",
                        column: x => x.PuestoVotacionId,
                        principalTable: "puesto_votacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claim_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cedula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Apodo = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsLider = table.Column<bool>(type: "boolean", nullable: false),
                    BarrioId = table.Column<int>(type: "integer", nullable: false),
                    CodigoCId = table.Column<int>(type: "integer", nullable: false),
                    LiderId = table.Column<int>(type: "integer", nullable: true),
                    MesaVotacionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personas_Personas_LiderId",
                        column: x => x.LiderId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_barrio_BarrioId",
                        column: x => x.BarrioId,
                        principalTable: "barrio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_codigo_c_CodigoCId",
                        column: x => x.CodigoCId,
                        principalTable: "codigo_c",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personas_mesa_votacion_MesaVotacionId",
                        column: x => x.MesaVotacionId,
                        principalTable: "mesa_votacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonaCodigoB",
                columns: table => new
                {
                    PersonaId = table.Column<int>(type: "integer", nullable: false),
                    CodigoBId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaCodigoB", x => new { x.PersonaId, x.CodigoBId });
                    table.ForeignKey(
                        name: "FK_PersonaCodigoB_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonaCodigoB_codigo_b_CodigoBId",
                        column: x => x.CodigoBId,
                        principalTable: "codigo_b",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonasLengua",
                columns: table => new
                {
                    PersonaId = table.Column<int>(type: "integer", nullable: false),
                    LenguaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonasLengua", x => new { x.PersonaId, x.LenguaId });
                    table.ForeignKey(
                        name: "FK_PersonasLengua_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonasLengua_lengua_LenguaId",
                        column: x => x.LenguaId,
                        principalTable: "lengua",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonaId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_persona",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claim_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_login", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_user_login_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_role_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_token", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_user_token_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "barrio",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Barrio 1" },
                    { 2, "Barrio 2" },
                    { 3, "Barrio 3" },
                    { 4, "Barrio 4" }
                });

            migrationBuilder.InsertData(
                table: "categoria",
                columns: new[] { "Id", "Maximo", "Minimo", "Nombre" },
                values: new object[,]
                {
                    { 1, 9, 0, "Grupo A" },
                    { 2, 15, 10, "Grupo B" },
                    { 3, 30, 16, "Grupo C" },
                    { 4, 60, 31, "Grupo D" },
                    { 5, 100, 61, "Grupo E" },
                    { 6, 300, 101, "Grupo F" }
                });

            migrationBuilder.InsertData(
                table: "codigo_b",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "CodigoB 1" },
                    { 2, "CodigoB 2" },
                    { 3, "CodigoB 3" },
                    { 4, "CodigoB 4" }
                });

            migrationBuilder.InsertData(
                table: "codigo_c",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "CodigoC 1" },
                    { 2, "CodigoC 2" },
                    { 3, "CodigoC 3" },
                    { 4, "CodigoC 4" }
                });

            migrationBuilder.InsertData(
                table: "lengua",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Lengua 1" },
                    { 2, "Lengua 2" },
                    { 3, "Lengua 3" },
                    { 4, "Lengua 4" }
                });

            migrationBuilder.InsertData(
                table: "puesto_votacion",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "PuestoVotacion 1" },
                    { 2, "PuestoVotacion 2" },
                    { 3, "PuestoVotacion 3" },
                    { 4, "PuestoVotacion 4" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("019a4721-233e-78b7-b7af-c71a11a344e4"), null, "Administrador", "ADMINISTRADOR" },
                    { new Guid("072de9db-ea58-418f-a43d-39f846821b4e"), null, "Digitalizador", "DIGITALIZADOR" }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PersonaId", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("96cb56e3-def8-4433-8341-ecf5424c7a7f"), 0, "16de0733-137a-4fa7-b65e-7bfc548e2be3", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEAKqLP59ArNX3MAlKYFmKMlc2Rc8QJDlgQL/CWRVH63Cg4Z1JYLKTP9BTUeJOTnMuA==", null, null, false, "d21bd88e-7f24-4fb9-b9f2-34924342ef90", false, "admin" });

            migrationBuilder.InsertData(
                table: "mesa_votacion",
                columns: new[] { "Id", "Nombre", "PuestoVotacionId" },
                values: new object[,]
                {
                    { 1, "Mesa 1", 1 },
                    { 2, "Mesa 2", 1 },
                    { 3, "Mesa 3", 1 },
                    { 4, "Mesa 4", 1 },
                    { 5, "Mesa 1", 2 },
                    { 6, "Mesa 2", 2 },
                    { 7, "Mesa 3", 2 },
                    { 8, "Mesa 4", 2 },
                    { 9, "Mesa 1", 3 },
                    { 10, "Mesa 2", 3 },
                    { 11, "Mesa 3", 3 },
                    { 12, "Mesa 4", 3 },
                    { 13, "Mesa 1", 4 },
                    { 14, "Mesa 2", 4 },
                    { 15, "Mesa 3", 4 },
                    { 16, "Mesa 4", 4 }
                });

            migrationBuilder.InsertData(
                table: "user_role",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("019a4721-233e-78b7-b7af-c71a11a344e4"), new Guid("96cb56e3-def8-4433-8341-ecf5424c7a7f") });

            migrationBuilder.CreateIndex(
                name: "IX_barrio_Nombre",
                table: "barrio",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categoria_Nombre",
                table: "categoria",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_codigo_b_Nombre",
                table: "codigo_b",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_codigo_c_Nombre",
                table: "codigo_c",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lengua_Nombre",
                table: "lengua",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mesa_votacion_PuestoVotacionId",
                table: "mesa_votacion",
                column: "PuestoVotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaCodigoB_CodigoBId",
                table: "PersonaCodigoB",
                column: "CodigoBId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_BarrioId",
                table: "Personas",
                column: "BarrioId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_CodigoCId",
                table: "Personas",
                column: "CodigoCId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_LiderId",
                table: "Personas",
                column: "LiderId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_MesaVotacionId",
                table: "Personas",
                column: "MesaVotacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasLengua_LenguaId",
                table: "PersonasLengua",
                column: "LenguaId");

            migrationBuilder.CreateIndex(
                name: "IX_puesto_votacion_Nombre",
                table: "puesto_votacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claim_RoleId",
                table: "role_claim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_user_PersonaId",
                table: "user",
                column: "PersonaId",
                unique: true,
                filter: "\"PersonaId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "user",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_claim_UserId",
                table: "user_claim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_login_UserId",
                table: "user_login",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_RoleId",
                table: "user_role",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categoria");

            migrationBuilder.DropTable(
                name: "PersonaCodigoB");

            migrationBuilder.DropTable(
                name: "PersonasLengua");

            migrationBuilder.DropTable(
                name: "role_claim");

            migrationBuilder.DropTable(
                name: "user_claim");

            migrationBuilder.DropTable(
                name: "user_login");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "user_token");

            migrationBuilder.DropTable(
                name: "codigo_b");

            migrationBuilder.DropTable(
                name: "lengua");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "barrio");

            migrationBuilder.DropTable(
                name: "codigo_c");

            migrationBuilder.DropTable(
                name: "mesa_votacion");

            migrationBuilder.DropTable(
                name: "puesto_votacion");
        }
    }
}
