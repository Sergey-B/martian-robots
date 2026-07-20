using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;
    
/// <inheritdoc />
public partial class CreateDatabase : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.CreateTable(
            name: "scents",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                x = table.Column<int>(type: "integer", nullable: false),
                y = table.Column<int>(type: "integer", nullable: false),
                world_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_scents", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "worlds",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                width = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                height = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_worlds", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_scents_world_id",
            schema: "public",
            table: "scents",
            column: "world_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "scents",
            schema: "public");

        migrationBuilder.DropTable(
            name: "worlds",
            schema: "public");
    }
}
