using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Backend.Models
{
 
    public class ApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public ExerciseData? Data { get; set; }
    }

    public class ExerciseData
    {
        [JsonPropertyName("previousPage")]
        public string? PreviousPage { get; set; }

        [JsonPropertyName("nextPage")]
        public string? NextPage { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("totalExercises")]
        public int TotalExercises { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("exercises")]
        public List<Exercise> Exercises { get; set; } = [];
    }

    public class Exercise
    {
        public ICollection<UserExercises> UserExercises { get; set; } = new List<UserExercises>();
        [JsonPropertyName("exerciseId")]
        public required string ExerciseId { get; set; }  

        [JsonPropertyName("name")]
        public required string Name { get; set; } = string.Empty;

        [JsonPropertyName("gifUrl")]
        public string GifUrl { get; set; } = string.Empty;

        [JsonPropertyName("instructions")]
        public required List<string> Instructions { get; set; }

        [JsonPropertyName("targetMuscles")]
        public List<string> TargetMuscles { get; set; } = [];

        [JsonPropertyName("bodyParts")]
        public List<string> BodyParts { get; set; } = [];

        [JsonPropertyName("equipments")]
        public List<string> Equipments { get; set; } = [];

        [JsonPropertyName("secondaryMuscles")]
        public List<string> SecondaryMuscles { get; set; } = [];
    }
}




