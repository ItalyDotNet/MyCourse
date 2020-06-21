using Microsoft.EntityFrameworkCore.Migrations;

namespace MyCourse.Migrations
{
    public partial class TriggersCourseVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TRIGGER CoursesSetRowVersionOnInsert
                                   AFTER INSERT ON Courses
                                   BEGIN
                                   UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql(@"CREATE TRIGGER CoursesSetRowVersionOnUpdate
                                   AFTER UPDATE ON Courses WHEN NEW.RowVersion <= OLD.RowVersion
                                   BEGIN
                                   UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP WHERE Id=NEW.Id;
                                   END;");
            migrationBuilder.Sql("UPDATE Courses SET RowVersion = CURRENT_TIMESTAMP;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER CoursesSetRowVersionOnInsert;");
            migrationBuilder.Sql("DROP TRIGGER CoursesSetRowVersionOnUpdate;");
        }
    }
}
