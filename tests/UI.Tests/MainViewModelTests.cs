using Xunit;
using Moq;
using UI.ViewModels;
using Backend;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Tests
{
    public class MainViewModelTests
    {
        // Reusable underline for test cases
        private static readonly string _underline = new('-', 40);

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
        public async Task FetchExer_UpdatesCollection(string muscleGroup)
        {
            var mockApi = new Mock<ExerciseDbApi>(new HttpClient());
            var sampleExercises = new List<Exercise> {
                new() { Name = "Exercise 1" },
                new() { Name = "Exercise 2" }
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

        [Fact]
        public void Steps_SimpleTest()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                Name = "Push Up",
                Instructions = ["Step 1", "Step 2", "Step 3"]
            };


            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"PUSH UP\n{_underline}\n\nStep 1\n\nStep 2\n\nStep 3";

            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_EmptyCase()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                Name = "A Workout Without Steps",
                Instructions = []
            };

            viewModel.SelectedExercise = exercise;

            Assert.Equal("", viewModel.SelectedExerciseSteps);
        }
        [Fact]
        public void Steps_SingleInstruction()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                Name = "Exercise name",
                Instructions = ["123"]
            };

            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"EXERCISE NAME\n{_underline}\n\n123";
            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_UpdateOnSwap()
        {
            // Arrange
            var viewModel = new MainViewModel();
            var firstExercise = new Exercise
            {
                Name = "Push Up",
                Instructions = ["Step 1", "Step 2"]
            };
            var secondExercise = new Exercise
            {
                Name = "Squat",
                Instructions = ["Step A", "Step B"]
            };

            // VM picks first Exercise
            viewModel.SelectedExercise = firstExercise;
            var firstExpectedOutput = $"PUSH UP\n{_underline}\n\nStep 1\n\nStep 2";
            Assert.Equal(firstExpectedOutput, viewModel.SelectedExerciseSteps);

            // VM switches Exercise
            viewModel.SelectedExercise = secondExercise;
            var secondExpectedOutput = $"SQUAT\n{_underline}\n\nStep A\n\nStep B";
            Assert.Equal(secondExpectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_SpecialCharactersInName()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                Name = "pushup! #1 @Home",
                Instructions = ["ABC"]
            };

            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"PUSHUP! #1 @HOME\n{_underline}\n\nABC";
            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_LongExerciseName()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                Name = new('a', 100),
                Instructions = ["1", "2"]
            };

            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"{new('A',100)}\n{_underline}\n\n1\n\n2";
            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_NullInstruction()
        {  
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                Name = "Null Instructions",
                Instructions = null 
            }; 

            viewModel.SelectedExercise = exercise;

            Assert.Equal("", viewModel.SelectedExerciseSteps);
        }

       
    }
}
