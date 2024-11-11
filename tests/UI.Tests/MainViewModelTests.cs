using Xunit;
using Moq;
using UI.ViewModels;
using Backend;
using Backend.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace UI.Tests
{
    public class MainViewModelTests
    {
        private static readonly string _underlineForAssert = new('-', 40);

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
            var sampleExercises = new List<Exercise>
            {
                new() { ExerciseId = "1", Name = "Exercise 1", Instructions = new List<string> { "Instruct" } },
                new() { ExerciseId = "2", Name = "Exercise 2", Instructions = new List<string> { "Instruct" } }
            };

            mockApi.Setup(api => api.GetExercisesAsync(muscleGroup)).ReturnsAsync(sampleExercises);
            var viewModel = new MainViewModel(mockApi.Object);
            viewModel.FetchExercisesCommand.Execute(muscleGroup);

            await Task.Delay(100);

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
                ExerciseId = "1",
                Name = "Push Up",
                Instructions = new List<string> { "Step 1", "Step 2", "Step 3" }
            };

            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"PUSH UP\n{_underlineForAssert}\n\nStep 1\n\nStep 2\n\nStep 3";

            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_EmptyCase()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                ExerciseId = "2",
                Name = "A Workout Without Steps",
                Instructions = new List<string>()
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
                ExerciseId = "3",
                Name = "Exercise name",
                Instructions = new List<string> { "123" }
            };

            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"EXERCISE NAME\n{_underlineForAssert}\n\n123";
            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_UpdateOnSwap()
        {
            var viewModel = new MainViewModel();
            var firstExercise = new Exercise
            {
                ExerciseId = "4",
                Name = "Push Up",
                Instructions = new List<string> { "Step 1", "Step 2" }
            };
            var secondExercise = new Exercise
            {
                ExerciseId = "5",
                Name = "Squat",
                Instructions = new List<string> { "Step A", "Step B" }
            };

            viewModel.SelectedExercise = firstExercise;
            var firstExpectedOutput = $"PUSH UP\n{_underlineForAssert}\n\nStep 1\n\nStep 2";
            Assert.Equal(firstExpectedOutput, viewModel.SelectedExerciseSteps);

            viewModel.SelectedExercise = secondExercise;
            var secondExpectedOutput = $"SQUAT\n{_underlineForAssert}\n\nStep A\n\nStep B";
            Assert.Equal(secondExpectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_SpecialCharactersInName()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                ExerciseId = "6",
                Name = "pushup! #1 @Home",
                Instructions = new List<string> { "ABC" }
            };

            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"PUSHUP! #1 @HOME\n{_underlineForAssert}\n\nABC";
            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_LongExerciseName()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                ExerciseId = "7",
                Name = new string('a', 100),
                Instructions = new List<string> { "1", "2" }
            };

            viewModel.SelectedExercise = exercise;
            var expectedOutput = $"{new string('A', 100)}\n{_underlineForAssert}\n\n1\n\n2";
            Assert.Equal(expectedOutput, viewModel.SelectedExerciseSteps);
        }

        [Fact]
        public void Steps_NullInstruction()
        {
            var viewModel = new MainViewModel();
            var exercise = new Exercise
            {
                ExerciseId = "8",
                Name = "Null Instructions",
                Instructions = new List<string>() // Even though instructions can be empty, they should not be null
            };

            viewModel.SelectedExercise = exercise;

            Assert.Equal("", viewModel.SelectedExerciseSteps);
        }
    }
}
