using Xunit;
using Moq;
using UI.ViewModels;
using Backend;
using Backend.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Tests
{
    public class MainViewModelTests
    {
        [Fact]
        public void FetchExer_Initialized()
        {
            var viewModel = new MainViewModel();
            var fetchCommand = viewModel.FetchExercisesCommand;
            Assert.NotNull(fetchCommand);
        }

        [Theory]
        [InlineData("abs")]
        [InlineData("biceps")]
        [InlineData("triceps")]
        public async Task FetchExerUpdatesCollection(string muscleGroup)
        {

            var mockApi = new Mock<ExerciseDbApi>(new HttpClient());
            var sampleExercises = new List<Exercise> {
                new Exercise { Name = "Exercise 1" },
                new Exercise { Name = "Exercise 2" }
            };

            // Set up mock API 
            mockApi.Setup(api => api.GetExercisesAsync(muscleGroup)).ReturnsAsync(sampleExercises);
            var viewModel = new MainViewModel(mockApi.Object);
            viewModel.FetchExercisesCommand.Execute(muscleGroup);

            // Wait for async
            await Task.Delay(100);

            // Assert
            Assert.Equal(2, viewModel.Exercises.Count);
            Assert.Equal("Exercise 1", viewModel.Exercises[0].Name);
            Assert.Equal("Exercise 2", viewModel.Exercises[1].Name);
        }

    }
}
