
using System.Text.Json;
using Backend.Models;

namespace Backend
{
    public class ExerciseDbApi(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;
        private const string _baseUrl = "https://exercisedb-api.vercel.app";
        private readonly List<string> _muscles =
            [
            "abs", "abductors", "adductors", "biceps","brachialis",
            "calves","delts", "forearms", "glutes", 
            "hamstrings", "lats", "pectorals",
            "quads",  "traps", "triceps",  
            ];


        /// <summary>
        /// The Primary Method used to communicate and query Exercises from The API.
        /// It also handles Json Deserialization and reconstructs an Exercise to be displayed.
        /// This method is made virtual in order to be intercepted for mocking with unit tests.
        /// </summary>
        /// <param name="muscle"></param>
        /// <returns>List of Exercises.</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="JsonException">API did not return expected fields</exception>
        public virtual async Task<List<Exercise>> GetExercisesAsync(string muscle)
        {

            if (!_muscles.Contains(muscle))
            {
                throw new ArgumentException("Invalid Query Parameter");
            }

            string encodedMuscle = Uri.EscapeDataString(muscle);
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/muscles/{encodedMuscle}/exercises");
            response.EnsureSuccessStatusCode();

            // Parse the response
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

            // Check For correct Json Structure after parsing
            if (apiResponse?.Data?.Exercises == null)
            {
                throw new JsonException("The JSON structure is missing the expected 'data' or 'exercises' fields.");
            }

            // Return the exercises list or an empty list if it's null
            return apiResponse?.Data?.Exercises ?? [];
        }
    }
}