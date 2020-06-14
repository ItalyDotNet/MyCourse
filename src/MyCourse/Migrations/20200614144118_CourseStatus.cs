using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCourse.Migrations
{
    public partial class CourseStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Courses",
                nullable: false,
                defaultValue: nameof(Models.Enums.CourseStatus.Deleted)
            );
            migrationBuilder.Sql($"UPDATE Courses SET Status='{nameof(Models.Enums.CourseStatus.Draft)}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Courses");
        }
    }
}
