
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
            "abdominals","abductors","adductors","biceps","calves",
            "chest","forearms","glutes","hamstrings","lats",
            "lower_back","middle_back","neck","quadriceps","traps","triceps"
            ];


        public async Task<List<Exercise>> GetExercisesAsync(string muscle)
        {

            if (!_muscles.Contains(muscle))
            {
                throw new Exception("Invalid Query Parameter");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/muscles/{muscle}/exercises");
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