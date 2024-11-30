using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserExercises",
                keyColumns: new[] { "ExerciseId", "UserId" },
                keyValues: new object[] { "PushUp", 1 });

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "ExerciseId",
                keyValue: "PushUp");

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "ExerciseId", "BodyParts", "Equipments", "GifUrl", "Instructions", "Name", "SecondaryMuscles", "TargetMuscles" },
                values: new object[] { "PushUps", "[]", "[]", "https://example.com/pushup.gif", "[\"Do a push-up.\"]", "Push-Up", "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Name",
                value: "John2 Doe");

            migrationBuilder.InsertData(
                table: "UserExercises",
                columns: new[] { "ExerciseId", "UserId" },
                values: new object[] { "PushUps", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserExercises",
                keyColumns: new[] { "ExerciseId", "UserId" },
                keyValues: new object[] { "PushUps", 1 });

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "ExerciseId",
                keyValue: "PushUps");

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "ExerciseId", "BodyParts", "Equipments", "GifUrl", "Instructions", "Name", "SecondaryMuscles", "TargetMuscles" },
                values: new object[] { "PushUp", "[]", "[]", "https://example.com/pushup.gif", "[\"Do a push-up.\"]", "Push-Up", "[]", "[]" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Name",
                value: "John Doe");

            migrationBuilder.InsertData(
                table: "UserExercises",
                columns: new[] { "ExerciseId", "UserId" },
                values: new object[] { "PushUp", 1 });
        }
    }
}
