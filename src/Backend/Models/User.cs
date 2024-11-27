namespace Backend.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<UserExercise> UserExercises { get; set; } = new List<UserExercise>();

    }
    public class UserExercise
    {
        public required int UserId { get; set; }
        public required User User { get; set; }

        public required string ExerciseId { get; set; }
        public required Exercise Exercise { get; set; }

    }



}
