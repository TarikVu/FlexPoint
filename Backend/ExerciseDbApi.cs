using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Backend.Models;

namespace Backend
{
    public class ExerciseDbApi
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://exercisedb-api.vercel.app"; 
        public ExerciseDbApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Exercise>> GetAllExercisesAsync(string muscle)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/v1/muscles/{muscle}/exercises");
            response.EnsureSuccessStatusCode();

            // Parse the response
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);
          
            // Return the exercises list or an empty list if it's null
            return apiResponse?.Data?.Exercises ?? [];
        }
    }
}

