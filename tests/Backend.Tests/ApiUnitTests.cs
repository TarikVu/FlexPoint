using System.Net;
using System.Text;
using Backend.Models;
using Moq;
using Moq.Protected;
using System.Text.Json;

namespace Backend.Tests
{
    public class ApiUnitTests
    {
        private readonly string basicResponse;
        public ApiUnitTests()
        {
            basicResponse = JsonSerializer.Serialize(new ApiResponse
            {
                Success = true,
                Data = new ExerciseData
                {
                    Exercises = [new Exercise { ExerciseId = "1", Name = "Bicep Curl" }]
                }
            });
        }

        private static Mock<HttpMessageHandler> CreateMock(HttpStatusCode statusCode, string content)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                });

            return mockHttpMessageHandler;
        }

        [Fact]
        public async Task GetSimpleExercise()
        {
            var mockHandler = CreateMock(HttpStatusCode.OK, basicResponse);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            var result = await api.GetExercisesAsync("biceps");

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Bicep Curl", result[0].Name);
        }

        [Fact]
        public async Task GetExercise_EmptyList()
        {
            var emptyExercises = JsonSerializer.Serialize(new ApiResponse
            {
                Success = true,
                Data = new ExerciseData
                {
                    Exercises = [] // Empty list
                }
            });

            var mockHandler = CreateMock(HttpStatusCode.OK, emptyExercises);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            var result = await api.GetExercisesAsync("biceps");

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetExercises_NullFields()
        {
            // Arrange
            var nullMock = JsonSerializer.Serialize(new ApiResponse
            {
                Success = true,
                Data = new ExerciseData
                {
                    Exercises = [new Exercise { ExerciseId = null, Name = null }]
                }
            });

            var mockHandler = CreateMock(HttpStatusCode.OK, nullMock);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            var result = await api.GetExercisesAsync("biceps");

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Null(result[0].ExerciseId);
            Assert.Null(result[0].Name);
        }

        [Fact]
        public async Task GetExercises_ApiReturnsSuccessFalse()
        {

            var mockResponseContent = JsonSerializer.Serialize(new ApiResponse
            {
                Success = false, // API indicates failure
                Data = new ExerciseData
                {
                    Exercises =
            [
                new Exercise { ExerciseId = "1", Name = "Bicep Curl" }
            ]
                }
            });

            var mockHandler = CreateMock(HttpStatusCode.OK, mockResponseContent);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            var result = await api.GetExercisesAsync("biceps");

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Bicep Curl", result[0].Name);
        }

        [Fact]
        public async Task GetExercises_InvalidParameter()
        {
            var mockHandler = CreateMock(HttpStatusCode.OK, basicResponse);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<Exception>(() => api.GetExercisesAsync("JohnnyCash"));
        }

        [Fact]
        public async Task GetExercises_InvalidHttpStatus()
        {
            var mockHandler = CreateMock(HttpStatusCode.NotFound, ""); // 404 Not Found
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(() => api.GetExercisesAsync("biceps"));
        }

        [Fact]
        public async Task GetExercises_InvalidJsonStructure()
        {
            var mockResponseContent = "{\"crazySteve\":true}"; // Invalid JSON structure
            var mockHandler = CreateMock(HttpStatusCode.OK, mockResponseContent);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<JsonException>(() => api.GetExercisesAsync("biceps"));
        }

        [Fact]
        public async Task Exception500_GetExercise()
        {
            var mockHandler = CreateMock(HttpStatusCode.InternalServerError, "");
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(() => api.GetExercisesAsync("biceps"));
        }
    }
}
