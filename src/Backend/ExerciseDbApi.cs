
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
            "abs", "abductors", "biceps","brachialis",
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
            string currentUrl = $"{_baseUrl}/api/v1/muscles/{encodedMuscle}/exercises";
            var allExercises = new List<Exercise>();

            while (!string.IsNullOrEmpty(currentUrl))
            {
                var response = await _httpClient.GetAsync(currentUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);

                // Validate JSON structure
                if (apiResponse?.Data?.Exercises == null)
                {
                    throw new JsonException("The JSON structure is missing the expected 'data' or 'exercises' fields.");
                }

                allExercises.AddRange(apiResponse.Data.Exercises);

                // Update currentUrl to the next page
                if (apiResponse.Data.NextPage != null)
                {
                    currentUrl = apiResponse.Data.NextPage;
                }
                else
                {
                    currentUrl = "";
                }
            }

            return allExercises;
        }

    }
}