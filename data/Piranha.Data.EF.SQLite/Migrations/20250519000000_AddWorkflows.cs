using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Piranha.Data.EF.SQLite.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddWorkflows : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_Workflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Workflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_WorkflowStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WorkflowId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_WorkflowStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_WorkflowStages_Piranha_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Piranha_Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowStages_WorkflowId",
                table: "Piranha_WorkflowStages",
                column: "WorkflowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_WorkflowStages");

            migrationBuilder.DropTable(
                name: "Piranha_Workflows");
        }
    }
}
