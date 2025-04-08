using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseAttendance.Migrations
{
    /// <inheritdoc />
    public partial class CourseTimeAddId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTimes_Courses_CourseId",
                table: "CourseTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTimes_TimeTables_TimeTableId",
                table: "CourseTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseTimes",
                table: "CourseTimes");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CourseTimes",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseTimes",
                table: "CourseTimes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTimes_CourseId",
                table: "CourseTimes",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTimes_Courses_CourseId",
                table: "CourseTimes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTimes_TimeTables_TimeTableId",
                table: "CourseTimes",
                column: "TimeTableId",
                principalTable: "TimeTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTimes_Courses_CourseId",
                table: "CourseTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTimes_TimeTables_TimeTableId",
                table: "CourseTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseTimes",
                table: "CourseTimes");

            migrationBuilder.DropIndex(
                name: "IX_CourseTimes_CourseId",
                table: "CourseTimes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CourseTimes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseTimes",
                table: "CourseTimes",
                columns: new[] { "CourseId", "TimeTableId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTimes_Courses_CourseId",
                table: "CourseTimes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTimes_TimeTables_TimeTableId",
                table: "CourseTimes",
                column: "TimeTableId",
                principalTable: "TimeTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
