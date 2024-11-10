
using System.Text.Json;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;

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
            
            // Load Api Configurations for valid keys
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            _validMuscles = configuration.GetSection("ExerciseApi:Muscles").Get<List<string>>() ?? [];
        }


        /// <summary>
        /// The Primary Method used to communicate and query Exercises from The API.
        /// It also handles Json Deserialization and reconstructs an Exercise to be displayed.
        /// This method is made virtual in order to be intercepted for mocking with unit tests.
        /// </summary>
        /// <param name="muscle">Muscle to Query</param>
        /// <returns>List of Exercises.</returns>
        /// <exception cref="Exception">Invalid Query</exception>
        /// <exception cref="JsonException">API did not return expected fields</exception>
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