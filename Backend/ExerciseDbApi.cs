using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public async Task<List<Exercise>> GetExercisesAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/exercises"); // Assuming this endpoint provides exercises
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();


            return JsonSerializer.Deserialize<List<Exercise>>(content) ?? [];
        }
    }
}

