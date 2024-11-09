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

        public MainViewModel()
        {
            _exerciseDbApi = new ExerciseDbApi(new HttpClient());
            Exercises = new ObservableCollection<Exercise>();
            FetchExercisesCommand = new RelayCommand(async () => await FetchExercisesAsync("biceps"));
        }

        private async Task FetchExercisesAsync(string muscle)
        {
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
