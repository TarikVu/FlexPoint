using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Backend;
using Backend.Models;

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly ExerciseDbApi _exerciseDbApi;
        private ObservableCollection<Exercise> _exercises;
        private int _currentProgress;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private string _hoveredImageSource = "pack://application:,,,/Assets/base.png";

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }
        public ICommand FetchExercisesCommand { get; }

        private Exercise? _selectedExercise;

        public ObservableCollection<Exercise> Exercises
        {
            get => _exercises;
            set
            {
                _exercises = value;
                OnPropertyChanged();
            }
        }

        public int CurrentProgress
        {
            get => _currentProgress;
            private set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility ProgressVisibility
        {
            get => _progressVisibility;
            private set
            {
                if (_progressVisibility != value)
                {
                    _progressVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public string HoveredImageSource
        {
            get => _hoveredImageSource;
            set
            {
                if (_hoveredImageSource != value)
                {
                    _hoveredImageSource = value;
                    OnPropertyChanged();
                }
            }
        }


        public Exercise? SelectedExercise
        {
            get => _selectedExercise;
            set
            {
                _selectedExercise = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedWorkoutSteps)); 
            }
        }

        public string SelectedWorkoutSteps
        {
            get
            {
                if (SelectedExercise == null || 
                    SelectedExercise.Instructions == null || 
                    SelectedExercise.Instructions.Count == 0)
                    return "No steps available";

                // Header
                string workoutHeader = $"{SelectedExercise.Name.ToUpper()}\n" +
                    $"{new string('-', 40)}\n";

                // Body
                string steps = string.Join("\n\n", SelectedExercise.Instructions.Select((step, index) => $"{step}"));

                return $"{workoutHeader}\n{steps}";
            }
        }




        /// <summary>
        /// </summary>
        /// <param name="exerciseDbApi">To test with a Mock API</param>
        public MainViewModel(ExerciseDbApi exerciseDbApi)
        {
            _exerciseDbApi = exerciseDbApi ?? new ExerciseDbApi(new HttpClient());
            _exercises = [];

            // Bind non-async Commands
            MouseEnterCommand = new RelayCommand<string>(OnMouseEnter);
            MouseLeaveCommand = new RelayCommand<object>(_ => OnMouseLeave());

            // Bind async Commands
            FetchExercisesCommand = new RelayCommand<object>(
                async (muscle) => await FetchExercisesAsync(muscle as string),
                () => ProgressVisibility == Visibility.Collapsed
            );
        }

        /// <summary>
        /// Main Constructor Called by Xaml.cs
        /// </summary>
        public MainViewModel() : this(new ExerciseDbApi(new HttpClient())) { }

        private void OnMouseEnter(string muscleName)
        {
            HoveredImageSource = $"pack://application:,,,/Assets/{muscleName}.png";
        }

        private void OnMouseLeave()
        {
            HoveredImageSource = "pack://application:,,,/Assets/base.png";
        }

        private async Task FetchExercisesAsync(string? muscle)
        {
            if (string.IsNullOrEmpty(muscle)) return;

            // Enable the Progress bar
            ProgressVisibility = Visibility.Visible;
            CurrentProgress = 0;
            var progress = new Progress<int>(value => CurrentProgress = value);

            try
            {
                var exerciseList = await _exerciseDbApi.GetExercisesAsync(muscle);

                Exercises.Clear();
                int count = exerciseList.Count;
                for (int i = 0; i < count; i++)
                {
                    // Update progress
                    ((IProgress<int>)progress).Report((i * 100) / count);

                    // Simulate adding exercise (to avoid blocking UI)
                    await Task.Delay(10);
                    Exercises.Add(exerciseList[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exercises: {ex.Message}");
            }
            finally
            {
                ProgressVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates the view upon a Property Change.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
