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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;
using PdfSharp.Drawing.Layout;
using Microsoft.Win32;
using UI.Services;

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ExerciseDbApi _exerciseDbApi;

        private ObservableCollection<Exercise> _exercises;
        public ObservableCollection<Exercise> AddedExercises { get; } = [];
        public Exercise? CurrentSelectedExercise => SelectedAddedExercise ?? SelectedExercise;

        private readonly PdfWriter _pdfWriter;

        private int _currentProgress;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private string _hoveredImageSource = "pack://application:,,,/Assets/base.png";

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }
        public ICommand FetchExercisesCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }
        public ICommand SaveCommand { get; }

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
                    _selectedAddedExercise = null;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentSelectedExercise));
                    OnPropertyChanged(nameof(SelectedExerciseSteps));
                    ((RelayCommand<object>)AddExerciseCommand).RaiseCanExecuteChanged();
                    ((RelayCommand<object>)RemoveExerciseCommand).RaiseCanExecuteChanged();
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
                    _selectedExercise = null; 
                    OnPropertyChanged(); 
                    OnPropertyChanged(nameof(CurrentSelectedExercise)); 
                    OnPropertyChanged(nameof(SelectedExerciseSteps)); 
                    ((RelayCommand<object>)AddExerciseCommand).RaiseCanExecuteChanged(); 
                    ((RelayCommand<object>)RemoveExerciseCommand).RaiseCanExecuteChanged(); 
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
            _exercises = [];
            _exerciseDbApi = mockTestApi ?? new ExerciseDbApi(new HttpClient());
            _pdfWriter = new PdfWriter();
            SaveCommand = new RelayCommand<object>(_ => SaveToPdf(), () => AddedExercises.Count > 0);
            MouseEnterCommand = new RelayCommand<string>(OnMouseEnter);
            MouseLeaveCommand = new RelayCommand<object>(_ => OnMouseLeave());
            AddExerciseCommand = new RelayCommand<object>(_ => AddExercise(), () => SelectedExercise != null);
            RemoveExerciseCommand = new RelayCommand<object>(_ => RemoveExercise(), () => SelectedAddedExercise != null);
            FetchExercisesCommand = new RelayCommand<object>(
                async (muscle) => await FetchExercisesAsync((muscle as string)!),
                () => ProgressVisibility == Visibility.Collapsed
            );

            AddedExercises.CollectionChanged += (s, e) =>
            {
                ((RelayCommand<object>)SaveCommand).RaiseCanExecuteChanged();
            };
        }

        public MainViewModel() : this(new ExerciseDbApi(new HttpClient())) { }

        private void AddExercise()
        {
            if (SelectedExercise == null || AddedExercises.Any(ex => ex.ExerciseId == SelectedExercise.ExerciseId))
                return;

            AddedExercises.Add(SelectedExercise);
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

        private async Task FetchExercisesAsync(string muscle)
        { 
            CurrentProgress = 0;
            ProgressVisibility = Visibility.Visible;
            var progress = new Progress<int>(value => CurrentProgress = value);

            try
            {
                Exercises.Clear();
                var exerciseList = await _exerciseDbApi.GetExercisesAsync(muscle);
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
                ProgressVisibility = Visibility.Collapsed;
            }
        }
        public void SaveToPdf()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = "Exercises.pdf",
                DefaultExt = ".pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                _pdfWriter.Save(filename, AddedExercises);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
