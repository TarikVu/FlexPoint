# FlexPoint
A desktop application built with .NET 7 and WPF for helping users select stretches based on muscle pain and group. It integrates with external APIs to fetch relevant stretch recommendations and images, providing a personalized user experience for pain relief.

## Features
- **Interactive UI**: Clickable muscle groups for pain input.
- **Stretch Recommendations**: Fetch stretches via the Wger API based on selected muscle group.
- **Image Display**: Fetch relevant images from Pixabay to visualize stretches.
- **Pain Input**: Users can input pain type, intensity, duration, and frequency for personalized recommendations.

## Tech Stack
- **Backend**:
  - .NET 8
  - WPF (Windows Presentation Foundation)
  - External APIs: 
    - ExerciseDB API (for stretches and exercises)
    - Pixabay API (for images)
- **Frontend**:
  - WPF for UI/UX
  - MVVM Pattern for app structure
    - Microsoft.Xaml.Behaviors.Wpf: Downloaded for Mouse hover behaviors in MVVM 
- **Testing**:
  - xUnit for unit testing
  - moq (ver Moq 4.20.72 )
  - TDD (Test-Driven Development)

## References
- [ExerciseDB API](https://exercisedb-api.vercel.app/docs) 
- [MVVM & Data Binding](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)
  - [Data Binding](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/?view=netdesktop-8.0)  
  - [Relay Command](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/relaycommand)
- StackOverflow
  - [Progress Bar](https://stackoverflow.com/questions/3520359/how-to-implement-a-progress-bar-using-the-mvvm-pattern)
- Assets
  - [Body Diagram](https://www.etsy.com/au/listing/1111381930/editable-muscle-map-anatomy-poster)
  

## <a name="testing"></a> Testing

### <a name="unit-testing"></a> Unit Testing
To isolate getting a json from the api, we first utilize Moq to simulate an api response for unit testing.
This allows us to simulate the different errors and avoid relying on any usage restrictions.

References:
- This [Example blog](https://canro91.github.io/2022/12/01/TestingHttpClient/) shows a good example
of using moq for a simulated api testing as well as a way to streamline setting up the fake client.
- [Official Repo](https://github.com/devlooped/moq?tab=readme-ov-file#readme)

### <a name="integration-testing"></a> Integration Testing


### Dev Log:

11/7:
- Project setup and initial pull
 
11/8:
- Able to pull from the Open api
- Added simple UI to display the parsed data and query
- Added unit tests for api handler

11/9:
- Refactored from MVC to MVVM 
- Added Responsive image changes for Muscle Group Diagram
- Added a Loading indicator
- Added .png Assets to UI
- Displays All workouts from a Muscle Group by utilizing "NextPage" In JSon 