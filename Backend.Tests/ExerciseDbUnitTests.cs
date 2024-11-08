using System.Net;
using System.Text;
using Moq;
using Moq.Protected;

namespace Backend.Tests
{
    public class ExerciseDbApiTests
    {
        // Creates a mock httpMessageHandler for unit testing. (DRY)
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
        public async Task GetExercises_SimpleList()
        {
            string response = "[{\"Id\":\"1\",\"Name\":\"Push-up\",\"Category\":\"Strength\"}]";
            var mockHandler = CreateMock(HttpStatusCode.OK, response);
            var httpClient = new HttpClient(mockHandler.Object);

            // Inject mock HTTP
            var exerciseDbApi = new ExerciseDbApi(httpClient);
            var exercises = await exerciseDbApi.GetExercisesAsync();

            Assert.NotNull(exercises);
            Assert.Single(exercises);
            Assert.Equal("1", exercises[0].Id);
            Assert.Equal("Push-up", exercises[0].Name);
            Assert.Equal("Strength", exercises[0].Category);
        }

        [Fact]
        public async Task GetExercises_EmptyList()
        {
            var mockHandler = CreateMock(HttpStatusCode.OK, "[]");
            var httpClient = new HttpClient(mockHandler.Object);

            var exerciseDbApi = new ExerciseDbApi(httpClient);
            var exercises = await exerciseDbApi.GetExercisesAsync();

            Assert.NotNull(exercises);
            Assert.Empty(exercises);
        }


        //Error Testing
        [Fact]
        public async Task ErrorThrowException_GetExercisesAsync()
        {
            var mockHandler = CreateMock(HttpStatusCode.InternalServerError, string.Empty);
            var httpClient = new HttpClient(mockHandler.Object);
            var exerciseDbApi = new ExerciseDbApi(httpClient);

            await Assert.ThrowsAsync<HttpRequestException>(() => exerciseDbApi.GetExercisesAsync());
        }
        
    }

}
