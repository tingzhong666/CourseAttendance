using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseAttendance.Migrations
{
    /// <inheritdoc />
    public partial class _25318 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignOutTime",
                table: "Attendances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SignOutTime",
                table: "Attendances",
                type: "datetime2",
                nullable: true);
        }
    }
}
