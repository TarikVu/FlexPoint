using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UserExercisesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserExercises",
                columns: new[] { "ExerciseId", "UserId" },
                values: new object[] { "PushUp", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserExercises",
                keyColumns: new[] { "ExerciseId", "UserId" },
                keyValues: new object[] { "PushUp", 1 });
        }
    }
}
