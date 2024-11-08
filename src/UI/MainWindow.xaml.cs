using System.Windows;
using Backend.Controllers;
using Backend.Models;


namespace UI
{
    public partial class MainWindow : Window
    {
        private readonly ExercisesController _controller;

        public MainWindow()
        {
            InitializeComponent();
            _controller = new ExercisesController();
        }

        private async void BicepsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Exercise> exercises = await _controller.GetExercises("biceps");
                ExercisesList.Items.Clear();

                foreach (var exercise in exercises)
                {
                    ExercisesList.Items.Add($"{exercise.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching exercises: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
