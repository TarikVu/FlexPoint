namespace Backend.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<UserExercises> UserExercises { get; set; } = new List<UserExercises>();

    }
    public class UserExercises
    {
        public required int UserId { get; set; }
        public User? User { get; set; }

        public required string ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }

    }



}
