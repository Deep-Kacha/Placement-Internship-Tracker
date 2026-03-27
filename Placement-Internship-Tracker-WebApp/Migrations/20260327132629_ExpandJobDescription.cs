using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlacementTracker.Migrations
{
    /// <inheritdoc />
    public partial class ExpandJobDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalDetails",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyWebsite",
                table: "JobDescriptions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "JobDescriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactMobile",
                table: "JobDescriptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "JobDescriptions",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfJoining",
                table: "JobDescriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternshipDuration",
                table: "JobDescriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stipend",
                table: "JobDescriptions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalDetails",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "CompanyWebsite",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "ContactMobile",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "DateOfJoining",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "InternshipDuration",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "Stipend",
                table: "JobDescriptions");
        }
    }
}
