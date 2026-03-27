using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlacementTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTerminologyAndFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PPOPackage",
                table: "InternshipApplications",
                newName: "FullTimePackage");

            migrationBuilder.RenameColumn(
                name: "IsPPOConverted",
                table: "InternshipApplications",
                newName: "IsFullTimeOffered");

            migrationBuilder.AddColumn<string>(
                name: "AnnualCTC",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bond",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CampusDriveDate",
                table: "JobDescriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentsPath",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EligibleBatches",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EligibleCourses",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationLink",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectionProcess",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CGPA",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DiplomaPercentage",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentMobile",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentAddress",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pincode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TenthPercentage",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TwelfthPercentage",
                table: "AspNetUsers",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualCTC",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "Bond",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "CampusDriveDate",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "DocumentsPath",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "EligibleBatches",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "EligibleCourses",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "RegistrationLink",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "SelectionProcess",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "Branch",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CGPA",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DiplomaPercentage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ParentEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ParentMobile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ParentName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PermanentAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Pincode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TenthPercentage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwelfthPercentage",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IsFullTimeOffered",
                table: "InternshipApplications",
                newName: "IsPPOConverted");

            migrationBuilder.RenameColumn(
                name: "FullTimePackage",
                table: "InternshipApplications",
                newName: "PPOPackage");
        }
    }
}
