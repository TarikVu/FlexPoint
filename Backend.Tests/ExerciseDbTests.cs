using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Moq.Protected;
using Xunit;

namespace Backend.Tests
{
    public class ExerciseDbApiTests
    {
        [Fact]
        public async Task GetExercisesAsync_ReturnsExerciseList()
        {
            // Arrange
            var exerciseJson = "[{\"Id\":\"1\",\"Name\":\"Push-up\",\"Category\":\"Strength\"}]";
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(exerciseJson, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var exerciseDbApi = new ExerciseDbApi(httpClient);

            // Act
            var exercises = await exerciseDbApi.GetExercisesAsync();

            // Assert
            Assert.NotNull(exercises);
            Assert.Single(exercises);
            Assert.Equal("1", exercises[0].Id);
            Assert.Equal("Push-up", exercises[0].Name);
            Assert.Equal("Strength", exercises[0].Category);
        }

        [Fact]
        public async Task GetExercisesAsync_HandlesEmptyResponse()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var exerciseDbApi = new ExerciseDbApi(httpClient);

            // Act
            var exercises = await exerciseDbApi.GetExercisesAsync();

            // Assert
            Assert.NotNull(exercises);
            Assert.Empty(exercises);
        }
    }
}
