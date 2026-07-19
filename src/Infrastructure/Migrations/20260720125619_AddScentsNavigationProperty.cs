using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;
    
/// <inheritdoc />
public partial class AddScentsNavigationProperty : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "fk_scents_worlds_world_id",
            schema: "public",
            table: "scents",
            column: "world_id",
            principalSchema: "public",
            principalTable: "worlds",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_scents_worlds_world_id",
            schema: "public",
            table: "scents");
    }
}
