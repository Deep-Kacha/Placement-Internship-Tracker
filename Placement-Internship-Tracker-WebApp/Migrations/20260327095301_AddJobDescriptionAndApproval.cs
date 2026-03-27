using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlacementTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddJobDescriptionAndApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobDescriptionId",
                table: "JobApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobDescriptionId",
                table: "InternshipApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "JobDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageOrStipend = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecruiterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobDescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobDescriptions_AspNetUsers_RecruiterId",
                        column: x => x.RecruiterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobDescriptionId",
                table: "JobApplications",
                column: "JobDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_InternshipApplications_JobDescriptionId",
                table: "InternshipApplications",
                column: "JobDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobDescriptions_RecruiterId",
                table: "JobDescriptions",
                column: "RecruiterId");

            migrationBuilder.AddForeignKey(
                name: "FK_InternshipApplications_JobDescriptions_JobDescriptionId",
                table: "InternshipApplications",
                column: "JobDescriptionId",
                principalTable: "JobDescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_JobDescriptions_JobDescriptionId",
                table: "JobApplications",
                column: "JobDescriptionId",
                principalTable: "JobDescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternshipApplications_JobDescriptions_JobDescriptionId",
                table: "InternshipApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_JobDescriptions_JobDescriptionId",
                table: "JobApplications");

            migrationBuilder.DropTable(
                name: "JobDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_JobApplications_JobDescriptionId",
                table: "JobApplications");

            migrationBuilder.DropIndex(
                name: "IX_InternshipApplications_JobDescriptionId",
                table: "InternshipApplications");

            migrationBuilder.DropColumn(
                name: "JobDescriptionId",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "JobDescriptionId",
                table: "InternshipApplications");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AspNetUsers");
        }
    }
}
