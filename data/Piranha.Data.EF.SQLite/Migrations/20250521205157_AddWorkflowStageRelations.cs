using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowStageRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Piranha_WorkflowStages",
                type: "TEXT",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Piranha_Workflows",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Piranha_WorkflowStageRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceStageId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TargetStageId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_WorkflowStageRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_WorkflowStageRelations_Piranha_WorkflowStages_SourceStageId",
                        column: x => x.SourceStageId,
                        principalTable: "Piranha_WorkflowStages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Piranha_WorkflowStageRelations_Piranha_WorkflowStages_TargetStageId",
                        column: x => x.TargetStageId,
                        principalTable: "Piranha_WorkflowStages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Piranha_WorkflowStageRelations_Piranha_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Piranha_Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowStageRelations_SourceStageId",
                table: "Piranha_WorkflowStageRelations",
                column: "SourceStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowStageRelations_TargetStageId",
                table: "Piranha_WorkflowStageRelations",
                column: "TargetStageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowStageRelations_WorkflowId",
                table: "Piranha_WorkflowStageRelations",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_WorkflowStageRelations");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Piranha_WorkflowStages");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Piranha_Workflows");
        }
    }
}
