using System.Net;
using System.Text;
using Backend.Models;
using Moq;
using Moq.Protected;
using System.Text.Json;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;

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
                    Exercises =
                    [
                        new() { ExerciseId = "1", Name = "One Punch Exercise", Instructions = ["100 push-up,situps,and squats, then run 10km."] }
                    ]
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
            Assert.Equal("One Punch Exercise", result[0].Name);
        }

        [Fact]
        public async Task Error_GetExer_EmptyList()
        {
            var emptyExercises = JsonSerializer.Serialize(new ApiResponse
            {
                Success = true,
                Data = new ExerciseData
                {
                    Exercises = []  
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
        public async Task GetExercises_Simple()
        {
            var mockResponseContent = JsonSerializer.Serialize(new ApiResponse
            {
                Success = false, 
                Data = new ExerciseData
                {
                    Exercises =
                    [
                        new Exercise { ExerciseId = "1", Name = "Bicep Curl", Instructions = ["Curl your arms."] }
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
        public async Task Error_GetExer_BadArg()
        {
            var mockHandler = CreateMock(HttpStatusCode.OK, basicResponse);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<ArgumentException>(() => api.GetExercisesAsync("JohnnyCash"));
        }

        [Fact]
        public async Task Error_GetExer_InvalidHttpStatus()
        {
            var mockHandler = CreateMock(HttpStatusCode.NotFound, ""); 
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(() => api.GetExercisesAsync("biceps"));
        }

        [Fact]
        public async Task Error_GetExer_InvalidJsonReturn()
        {
            var mockResponseContent = "{\"crazySteve\":true}"; 
            var mockHandler = CreateMock(HttpStatusCode.OK, mockResponseContent);
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<JsonException>(() => api.GetExercisesAsync("biceps"));
        }

        [Fact]
        public async Task Error_GetExercise_Server500()
        {
            var mockHandler = CreateMock(HttpStatusCode.InternalServerError, "");
            var httpClient = new HttpClient(mockHandler.Object);
            var api = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(() => api.GetExercisesAsync("biceps"));
        }
    }
}
