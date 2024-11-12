
using System.Text.Json;
using Backend.Models;

namespace Backend
{
    public class ExerciseDbApi
    {
        private readonly HttpClient _httpClient; 
        private readonly List<string> _validMuscles;
        private readonly string _baseUrl = "https://exercisedb-api.vercel.app";

        public ExerciseDbApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _validMuscles = configuration.GetSection("ExerciseApi:Muscles").Get<List<string>>() ?? [];
        }


        public virtual async Task<List<Exercise>> GetExercisesAsync(string muscle)
        {
            if (!_validMuscles.Contains(muscle))
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

                if (apiResponse?.Data?.Exercises == null)
                {
                    throw new JsonException("The JSON is missing'data' or 'exercises'.");
                }

                allExercises.AddRange(apiResponse.Data.Exercises);

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