using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Piranha.Data.EF.SQLServer.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddWorkflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_ContentWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContentId = table.Column<Guid>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 50, nullable: false),
                    CurrentState = table.Column<int>(nullable: false),
                    AssignedTo = table.Column<Guid>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ContentWorkflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WorkflowId = table.Column<Guid>(nullable: false),
                    FromState = table.Column<int>(nullable: false),
                    ToState = table.Column<int>(nullable: false),
                    Action = table.Column<string>(maxLength: 100, nullable: false),
                    PerformedBy = table.Column<Guid>(nullable: false),
                    PerformedAt = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_WorkflowSteps_Piranha_ContentWorkflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Piranha_ContentWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentWorkflows_ContentId",
                table: "Piranha_ContentWorkflows",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ContentWorkflows_CurrentState",
                table: "Piranha_ContentWorkflows",
                column: "CurrentState");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowSteps_WorkflowId",
                table: "Piranha_WorkflowSteps",
                column: "WorkflowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_WorkflowSteps");

            migrationBuilder.DropTable(
                name: "Piranha_ContentWorkflows");
        }
    }
}
