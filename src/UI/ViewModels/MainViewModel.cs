using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Backend;
using Backend.Models;

namespace UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ExerciseDbApi _exerciseDbApi;
        private ObservableCollection<Exercise> _exercises;
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

        /// <summary>
        /// Main Constructor to set up our ExerciseDbApi, exercises, and RelayCommand
        /// </summary>
        /// <param name="exerciseDbApi">
        /// Optional Parameter Used for Tests when mocking the api, used to reduce
        /// Calling the acutal api in case of rate limitations</param>
        public MainViewModel(ExerciseDbApi? exerciseDbApi = null)
        {
            _exerciseDbApi = exerciseDbApi ?? new ExerciseDbApi(new HttpClient());
            _exercises = new ObservableCollection<Exercise>();
            FetchExercisesCommand = new RelayCommand(async (muscle) => await FetchExercisesAsync(muscle as string));
        }

        // Parameterless constructor for XAML instantiation
        public MainViewModel() : this(new ExerciseDbApi(new HttpClient()))
        {
        }

        /// <summary>
        /// Calls API to fetch Exercises.
        /// </summary>
        /// <param name="muscle"></param>
        /// <returns></returns>
        private async Task FetchExercisesAsync(string? muscle)
        {
            if (string.IsNullOrEmpty(muscle))
                return;
            try
            {
                var exerciseList = await _exerciseDbApi.GetExercisesAsync(muscle);
                Exercises.Clear();
                foreach (var exercise in exerciseList)
                {
                    Exercises.Add(exercise);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exercises: {ex.Message}");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
