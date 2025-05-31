using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowStageRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_WorkflowStageRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WorkflowStageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_WorkflowStageRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_WorkflowStageRoles_Piranha_WorkflowStages_WorkflowStageId",
                        column: x => x.WorkflowStageId,
                        principalTable: "Piranha_WorkflowStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowStageRoles_WorkflowStageId",
                table: "Piranha_WorkflowStageRoles",
                column: "WorkflowStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowStageRoles_RoleId",
                table: "Piranha_WorkflowStageRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowStageRoles_WorkflowStageId_RoleId",
                table: "Piranha_WorkflowStageRoles",
                columns: new[] { "WorkflowStageId", "RoleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_WorkflowStageRoles");
        }
    }
}
