using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeRequestComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PreviousStageId",
                table: "Piranha_ChangeRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Piranha_ChangeRequestComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsApprovalComment = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApprovalType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    StageId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_ChangeRequestComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_ChangeRequestComments_Piranha_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalTable: "Piranha_ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_ChangeRequestComments_Piranha_WorkflowStages_StageId",
                        column: x => x.StageId,
                        principalTable: "Piranha_WorkflowStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ChangeRequestComments_ChangeRequestId",
                table: "Piranha_ChangeRequestComments",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_ChangeRequestComments_StageId",
                table: "Piranha_ChangeRequestComments",
                column: "StageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_ChangeRequestComments");

            migrationBuilder.DropColumn(
                name: "PreviousStageId",
                table: "Piranha_ChangeRequests");
        }
    }
}
