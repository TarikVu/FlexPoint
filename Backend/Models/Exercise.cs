using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Backend.Models
{
    // Used for Deserializing the Response from ExerciseDbApi
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
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }

    public class Exercise
    {
        [JsonPropertyName("exerciseId")]
        public string ExerciseId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("gifUrl")]
        public string GifUrl { get; set; } = string.Empty;

        [JsonPropertyName("instructions")]
        public List<string> Instructions { get; set; } = new List<string>();

        [JsonPropertyName("targetMuscles")]
        public List<string> TargetMuscles { get; set; } = new List<string>();

        [JsonPropertyName("bodyParts")]
        public List<string> BodyParts { get; set; } = new List<string>();

        [JsonPropertyName("equipments")]
        public List<string> Equipments { get; set; } = new List<string>();

        [JsonPropertyName("secondaryMuscles")]
        public List<string> SecondaryMuscles { get; set; } = new List<string>();
    }
}
