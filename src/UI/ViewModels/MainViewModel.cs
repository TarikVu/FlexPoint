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
        public ObservableCollection<Exercise> AddedExercises { get; } = [];
        public Exercise? CurrentSelectedExercise => SelectedAddedExercise ?? SelectedExercise;

        private int _currentProgress;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private string _hoveredImageSource = "pack://application:,,,/Assets/base.png";

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }
        public ICommand FetchExercisesCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }

        private Exercise? _selectedExercise;
        private Exercise? _selectedAddedExercise;

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
                if (_selectedExercise != value)
                {
                    _selectedExercise = value;
                    _selectedAddedExercise = null; // Clear other selection
                    OnPropertyChanged(); // Notify UI to refresh anything bound to SelectedExercise
                    OnPropertyChanged(nameof(CurrentSelectedExercise)); // Trigger update for CurrentSelectedExercise
                    OnPropertyChanged(nameof(SelectedExerciseSteps)); // Updates UI with new steps text
                    ((RelayCommand<object>)AddExerciseCommand).RaiseCanExecuteChanged(); // Re-evaluates AddExerciseCommand
                    ((RelayCommand<object>)RemoveExerciseCommand).RaiseCanExecuteChanged(); // Re-evaluates RemoveExerciseCommand
                }
            }
        }

        public Exercise? SelectedAddedExercise
        {
            get => _selectedAddedExercise;
            set
            {
                if (_selectedAddedExercise != value)
                {
                    _selectedAddedExercise = value;
                    _selectedExercise = null; // Clear other selection
                    OnPropertyChanged(); // Notify UI to refresh anything bound to SelectedAddedExercise
                    OnPropertyChanged(nameof(CurrentSelectedExercise)); // Trigger update for CurrentSelectedExercise
                    OnPropertyChanged(nameof(SelectedExerciseSteps)); // Updates UI with new steps text
                    ((RelayCommand<object>)AddExerciseCommand).RaiseCanExecuteChanged(); // Re-evaluates AddExerciseCommand
                    ((RelayCommand<object>)RemoveExerciseCommand).RaiseCanExecuteChanged(); // Re-evaluates RemoveExerciseCommand
                }
            }
        }

        public string SelectedExerciseSteps
        {
            get
            {
                if (CurrentSelectedExercise == null ||
                    CurrentSelectedExercise.Instructions == null ||
                    CurrentSelectedExercise.Instructions.Count == 0)
                    return "";

                string workoutHeader = $"{CurrentSelectedExercise.Name.ToUpper()}\n" +
                                       $"{new string('-', 40)}\n";

                string steps = string.Join("\n\n", CurrentSelectedExercise.Instructions.Select((step, index) => $"{step}"));

                return $"{workoutHeader}\n{steps}";
            }
        }


        public MainViewModel(ExerciseDbApi mockTestApi)
        {
            _exerciseDbApi = mockTestApi ?? new ExerciseDbApi(new HttpClient());
            _exercises = [];

            MouseEnterCommand = new RelayCommand<string>(OnMouseEnter);
            MouseLeaveCommand = new RelayCommand<object>(_ => OnMouseLeave());
            AddExerciseCommand = new RelayCommand<object>(_ => AddExercise(), () => SelectedExercise != null);
            RemoveExerciseCommand = new RelayCommand<object>(_ => RemoveExercise(), () => SelectedAddedExercise != null);

            FetchExercisesCommand = new RelayCommand<object>(
                async (muscle) => await FetchExercisesAsync(muscle as string),
                () => ProgressVisibility == Visibility.Collapsed
            );
        }

        public MainViewModel() : this(new ExerciseDbApi(new HttpClient())) { }

        private void AddExercise()
        {
            if (SelectedExercise != null && !AddedExercises.Contains(SelectedExercise))
            {
                AddedExercises.Add(SelectedExercise);
            }
        }

        private void RemoveExercise()
        {
            if (SelectedAddedExercise != null)
            {
                AddedExercises.Remove(SelectedAddedExercise);
                SelectedAddedExercise = null; 
            }
        }


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
             
            Mouse.OverrideCursor = Cursors.Wait;

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
                    ((IProgress<int>)progress).Report((i * 100) / count);
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
                 
                Mouse.OverrideCursor = null;   
                ProgressVisibility = Visibility.Collapsed;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
