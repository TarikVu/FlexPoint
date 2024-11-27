using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Backend;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using UI.Services;

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;


        private string? _newUserName;
        private readonly ExerciseDbApi _exerciseDbApi;
        private readonly FlexPointDbContext _dbContext;
        private readonly PdfWriter _pdfWriter;


        private User? _selectedUser;
        public ObservableCollection<User> Users { get; } = [];

        private ObservableCollection<Exercise> _exercises;
        public ObservableCollection<Exercise> AddedExercises { get; } = [];

        public Exercise? CurrentSelectedExercise => SelectedAddedExercise ?? SelectedExercise;
        private Exercise? _selectedExercise;
        private Exercise? _selectedAddedExercise;

        private int _currentProgress;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private string _hoveredImageSource = "pack://application:,,,/Assets/base.png";
        private string _currentMuscle = "base";

        public ICommand AddNewUserCommand { get; }
        public ICommand MouseHoverCommand { get; }
        public ICommand MouseLeaveCommand { get; }
        public ICommand FetchExercisesCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }
        public ICommand ClearAddedExercisesCommand { get; }
        public ICommand SaveCommand { get; }

        public User SelectedUser
        {
            get => _selectedUser!;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                AddedExercises.Clear();

                var user = _dbContext.Users
                    .Include(u => u.UserExercises)
                    .ThenInclude(ue => ue.Exercise)
                    .FirstOrDefault(u => u.UserId == _selectedUser.UserId);

                var exercises = user!.UserExercises.Select(ue => ue.Exercise).ToList();

                foreach (Exercise e in exercises)
                {
                    AddedExercises.Add(e);
                }

            }
        }

        public ObservableCollection<Exercise> Exercises
        {
            get => _exercises;
            set
            {
                _exercises = value;
                OnPropertyChanged();
            }
        }

        public string NewUserName
        {
            get => _newUserName!;
            set
            {
                _newUserName = value;
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
                    SwapSelection();
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
                    SwapSelection();
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

                string steps = string.Join("\n\n", CurrentSelectedExercise.Instructions.Select((step, index) => $"{step}"));

                return $"{steps}";
            }
        }

        public MainViewModel(ExerciseDbApi mockTestApi)
        {
            NewUserName = "";
            _exercises = [];
            _exerciseDbApi = mockTestApi ?? new ExerciseDbApi(new HttpClient());
            _pdfWriter = new PdfWriter();


            _dbContext = new FlexPointDbContext();
            _dbContext.Database.EnsureCreated();

            AddNewUserCommand = new RelayCommand(AddNewUser);

            MouseHoverCommand = new RelayCommand<string>(OnMouseHover);
            MouseLeaveCommand = new RelayCommand(OnMouseLeave);

            Users = new ObservableCollection<User>(_dbContext.Users);

            SaveCommand = new RelayCommand(SaveToPdf, () => AddedExercises.Count > 0);
            AddExerciseCommand = new RelayCommand(AddExercise, () => SelectedExercise != null);
            RemoveExerciseCommand = new RelayCommand(RemoveExercise, () => SelectedAddedExercise != null);
            ClearAddedExercisesCommand = new RelayCommand(ClearAddedExercises);

            FetchExercisesCommand = new RelayCommand<object>(
                async (muscle) => await FetchExercisesAsync((muscle as string)!),
                () => ProgressVisibility == Visibility.Collapsed
            );

            AddedExercises.CollectionChanged += (s, e) =>
            {
                RelayChanged(SaveCommand);
            };
        }


        public MainViewModel() : this(new ExerciseDbApi(new HttpClient())) { }

        private async Task FetchExercisesAsync(string muscle)
        {
            CurrentProgress = 0;
            _currentMuscle = muscle;
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
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ProgressVisibility = Visibility.Collapsed;
            }
        }

        public void AddNewUser()
        {
            if (NewUserName == "")
            {
                MessageBox.Show("Please enter a name for the new user.");
                return;
            }

            User user = new() { Name = _newUserName! };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            Users.Add(user);

            NewUserName = "";
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

        private void SwapSelection()
        {
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentSelectedExercise));
            OnPropertyChanged(nameof(SelectedExercise));
            OnPropertyChanged(nameof(SelectedExerciseSteps));
            RelayChanged(AddExerciseCommand);
            RelayChanged(RemoveExerciseCommand);
        }

        private static void RelayChanged(ICommand command)
        {
            ((RelayCommand<object>)command).RaiseCanExecuteChanged();
        }

        private void AddExercise()
        {
            if (SelectedExercise == null || AddedExercises.Any(ex => ex.ExerciseId == SelectedExercise.ExerciseId))
                return;

            AddedExercises.Add(SelectedExercise);

            if (_selectedUser != null)
            {
                var user = _dbContext.Users.Find(_selectedUser.UserId);
                user!.UserExercises.Add(new UserExercise
                {
                    UserId = _selectedUser.UserId,
                    User = _selectedUser,
                    ExerciseId = SelectedExercise.ExerciseId,
                    Exercise = SelectedExercise
                });
            }

            Console.WriteLine("added exercise");
        }

        private void RemoveExercise()
        {
            if (SelectedAddedExercise != null)
            {
                AddedExercises.Remove(SelectedAddedExercise);
                SelectedAddedExercise = null;
            }
        }
        private void ClearAddedExercises()
        {
            AddedExercises.Clear();
        }

        private void OnMouseHover(string muscleName)
        {
            HoveredImageSource = $"pack://application:,,,/Assets/{muscleName}.png";
        }

        private void OnMouseLeave()
        {
            HoveredImageSource = $"pack://application:,,,/Assets/{_currentMuscle}.png";
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
