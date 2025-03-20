using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseAttendance.Migrations
{
    /// <inheritdoc />
    public partial class _25320 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Attendances");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Attendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PassWord",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "PassWord",
                table: "Attendances");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Attendances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
