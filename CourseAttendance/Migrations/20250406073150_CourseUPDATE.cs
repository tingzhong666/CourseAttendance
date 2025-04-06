using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseAttendance.Migrations
{
    /// <inheritdoc />
    public partial class CourseUPDATE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "CourseTimes");

            migrationBuilder.DropColumn(
                name: "Weekday",
                table: "CourseTimes");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "CourseTimes",
                newName: "DateDay");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateDay",
                table: "CourseTimes",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "CourseTimes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Weekday",
                table: "CourseTimes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
