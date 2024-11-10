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
        private readonly ExerciseDbApi _exerciseDbApi;
        private ObservableCollection<Exercise> _exercises;
        private int _currentProgress;
        private Visibility _progressVisibility = Visibility.Collapsed;

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand FetchExercisesCommand { get; }

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

        private string _hoveredImageSource = "pack://application:,,,/Assets/base.png"; // Default image

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


        public MainViewModel(ExerciseDbApi? exerciseDbApi = null)
        {
            _exerciseDbApi = exerciseDbApi ?? new ExerciseDbApi(new HttpClient());
            _exercises = new ObservableCollection<Exercise>();

            MouseEnterCommand = new RelayCommand<string>(OnMouseEnter);
            MouseLeaveCommand = new RelayCommandWithoutParams(OnMouseLeave);

            // Initialize FetchExercisesCommand with async support
            FetchExercisesCommand = new RelayCommand(
                async (muscle) => await FetchExercisesAsync(muscle as string),
                () => ProgressVisibility == Visibility.Collapsed
            );
        }


        public MainViewModel() : this(new ExerciseDbApi(new HttpClient())) { }

        private void OnMouseEnter(string muscleName)
        {
            HoveredImageSource = $"pack://application:,,,/Assets/{muscleName}.png";
        }

        private void OnMouseLeave()
        {
            HoveredImageSource = "pack://application:,,,/Assets/base.png";
        }

        /// <summary>
        /// Asynchronously fetch exercises with progress reporting.
        /// </summary>
        private async Task FetchExercisesAsync(string? muscle)
        {
            if (string.IsNullOrEmpty(muscle)) return;

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

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
