using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourseAttendance.Migrations
{
    /// <inheritdoc />
    public partial class SeddRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0F93B20D-BEA0-1628-F7A3-6E9E8A817A4E", null, "Student", "STUDENT" },
                    { "33F1A562-4F0B-C638-D624-2E92FE629D4D", null, "Academic", "ACADEMIC" },
                    { "523A6FB2-7348-7F06-8DC0-8697BED79A68", null, "Teacher", "TEACHER" },
                    { "C897C093-BEEA-798C-A116-0DA80A51784A", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0F93B20D-BEA0-1628-F7A3-6E9E8A817A4E");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "33F1A562-4F0B-C638-D624-2E92FE629D4D");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "523A6FB2-7348-7F06-8DC0-8697BED79A68");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "C897C093-BEEA-798C-A116-0DA80A51784A");
        }
    }
}
