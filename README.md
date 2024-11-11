# FlexPoint
A desktop application built with .NET 7 and WPF for helping users select workouts based on muscle group. 
It integrates with external APIs to fetch relevant workout recommendations and images, providing a personalized user experience for exercise planning.

## Features
- **Interactive UI**: Clickable muscle groups for pain input. 
- **ExerciseDB API workouts**: Query the API via Specified muscle groups in order to find relevant workouts
- **Workout Planning**: Users can select from the workouts provided to construct a regimen

## Planned Features 
- Web Application Migration:
    - Due to this application being developed in WPF with a MVVM focused design, the modularity that MVVM provides make the application more modular and will therefore help aid in switching to a web application
    - Furthermore, during this stage of development the application is more focused on the core backend functionality of communicating with Open API's and parsing their data.  
    - Developing in WPF keeps the UI Simple for now as It is not the main focus of the application at this moment.

- Database Integration: 
    - At this stage in development, There is no database implmented, but **MySQL or PostgreSQL** would be a good choice for a database in the future in order to create user tables and save user workouts.
- Google Calendar Upload:
    - Users will be able to construct workout regimen and upload it to their google Calendars.


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

## References & Packages Utilized

- [ExerciseDB API](https://exercisedb-api.vercel.app/docs) 
- [MVVM Design Patterns](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)
- [MVVM & Data Binding](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)
  - [Data Binding](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/?view=netdesktop-8.0)  
  - [Relay Command & ICommand](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/relaycommand)
- StackOverflow
  - [Progress Bar](https://stackoverflow.com/questions/3520359/how-to-implement-a-progress-bar-using-the-mvvm-pattern)
- Assets
  - [Body Diagram](https://www.etsy.com/au/listing/1111381930/editable-muscle-map-anatomy-poster)

- [PDFsharp](https://docs.pdfsharp.net/)
    - [Formatting with PDFsharp](https://docs.pdfsharp.net/MigraDoc/DOM/Document/Formatting.html)
    - [Basic Example of how to use PDFsharp](https://craftmypdf.com/blog/5-ways-to-generate-pdfs-with-c-sharp/)
    - [Drawing Images on PDF pages](https://docs.pdfsharp.net/PDFsharp/Topics/Bitmap-Images/Drawing.html)

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
- Made unit tests for api handler
- Able to pull from the Open api
- Added simple UI to display the parsed data and query

11/9:
- Refactored from MVC to MVVM 
- Added Responsive image changes for Muscle Group Diagram
- Added a Loading progress indicator
- Added .png Assets to UI
- Displays All workouts from a Muscle Group by utilizing "NextPage" In JSon 

11/10
- Added Display for chosen Exercise's steps & Unit Tests
- Added a SelectedExercise Container to add and remove from
- Steps Display works for both ListBox's of Exercises
- Added Overlay to prevent race conditions
- Added Place holders for Google API Integration 


## For the Wiki:


## Developer Notes 
### Refresher on RelayCommand and ViewModels for WPF:
ViewModel is the delegate between the Model and View.  It exposes data from the Model to the View.

### Using Virtual vs Interfaces for testing:
- [Good StackOverflow Advice](https://stackoverflow.com/questions/691725/mocking-objects-declare-all-methods-as-virtual-or-use-interface)


### In-depth with the Code
RelayCommand<T> leverages ICommand to run async and non-async commands as well as handles command execution.

#### MainViewModel
- [Using CallMemberName for Cleaner code for OnPropertyChange](https://www.c-sharpcorner.com/article/use-of-callermembername-with-inotifypropertychanged-interface-in-wpf-mvvm/)