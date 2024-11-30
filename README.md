
# FlexPoint
A desktop application built with the .NET 8 Framework for helping users select workouts based on muscle group. Workouts are then categorized and printed via a PDF Writer.  
Flexpiont integrates with external APIs to fetch relevant workout recommendations and images, providing a personalized user experience for exercise planning.
<p align="center"> <img src="https://github.com/TarikVu/imgs/blob/main/FlexPoint/flexpoint.png?raw=true" width="75%"> </p>


## Features

- **ExerciseDB API workouts**: Query the API via Specified muscle groups in order to find relevant workouts
- **Workout Planning**: Users can select from the workouts provided to construct a regimen
- **PDF Writer**: Users are able to create a pdf file of their selected workouts.
- Sqlite Database : Utilizes a liteweight local Sqlite Db in order to create simple user profiles.
  
<p align="center"> <img src="https://github.com/TarikVu/imgs/blob/main/FlexPoint/flexpoint_example.png?raw=true" width="75%"> </p>

## Planned Features 
- Interactive UI: 
	- Clickable muscle groups for muscle group selection. Utilizing SVG images
	- [SharpVectors](https://www.nuget.org/packages/SharpVectors)

- Google Calendar Upload:
    - Users will be able to construct workout regimen and upload it to their google Calendars.


## Tech Stack
- **Backend**:
  - .NET 8
  - External APIs: 
    - ExerciseDB API (for stretches and exercises)
- **Frontend**:
  - WPF for 
  - MVVM 
    
- **Testing**:
  - xUnit for unit testing
  - moq (ver Moq 4.20.72 )
  - Fine Code Coverage
 
## Architecture

<p align="center">
  <img src="https://github.com/TarikVu/imgs/blob/main/FlexPoint/FlexPoint_Arch.PNG?raw=true" width="50%">
</p>

[API Response](https://exercisedb-api.vercel.app/docs#tag/muscles/GET/api/v1/muscle) sample:
```bash
GET: /api/v1/bodyparts/{bodyPartName}/exercises
```
```bash
{
  "success": true,
  "data": {
    "previousPage": ,
    "nextPage": "...",
    "totalPages": ,
    "totalExercises": ,
    "currentPage": ,
    "exercises": [
      {
        "exerciseId": "...",
        "name": "...",
        "gifUrl": "...",
        "instructions": [],
        "targetMuscles": [],
        "bodyParts": [],
        "equipments": [],
        "secondaryMuscles": ]
      }
}
```

## References & Packages Utilized
- [Api Mocking for Unit Tests](https://www.damirscorner.com/blog/posts/20231222-MockingHttpClientInUnitTests.html)

- Assets
  - [Body Diagram](https://www.etsy.com/au/listing/1111381930/editable-muscle-map-anatomy-poster)
  
- [ExerciseDB API](https://exercisedb-api.vercel.app/docs) 

- [MVVM Design Patterns](https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern) 
  - [Data Binding](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/?view=netdesktop-8.0)  
  - [Relay Command & ICommand](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/relaycommand)
  -  [Using CallMemberName](https://www.c-sharpcorner.com/article/use-of-callermembername-with-inotifypropertychanged-interface-in-wpf-mvvm/)
    
 - [PDFsharp](https://docs.pdfsharp.net/)
    - [Formatting with PDFsharp](https://docs.pdfsharp.net/MigraDoc/DOM/Document/Formatting.html)
    - [Basic Example of how to use PDFsharp](https://craftmypdf.com/blog/5-ways-to-generate-pdfs-with-c-sharp/)
    - [Drawing Images on PDF pages](https://docs.pdfsharp.net/PDFsharp/Topics/Bitmap-Images/Drawing.html)
    - [How to Use XRect](https://docs.pdfsharp.net/PDFsharp/Topics/Start/First-PDF.html?q=xRect)
    
- StackOverflow
  - [Progress Bar](https://stackoverflow.com/questions/3520359/how-to-implement-a-progress-bar-using-the-mvvm-pattern)


- [XAML Styling](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/button-styles-and-templates?view=netframeworkdesktop-4.8)
	- [Microsoft.Xaml.Behaviors.Wpf](https://www.nuget.org/packages/microsoft.xaml.behaviors.wpf)

## <a name="testing"></a> Testing

### <a name="unit-testing"></a> Unit Testing
To isolate getting a json from the api, we first utilize Moq to simulate an api response for unit testing.
This allows us to simulate the different errors and avoid relying on any usage restrictions.
#### HttpRequestException
The following HttpRequestException codes are caught with our api call, upon an unsucessful api call the user interface reports
the corresponding error.
```bash
Client Errors (4xx)
- 400 Bad Request – Invalid request.
- 401 Unauthorized – Authentication required.
- 403 Forbidden – Access denied.
- 404 Not Found – Resource not found.
- 429 Too Many Requests – Rate limit exceeded.
Server Errors (5xx):
- 500 Internal Server Error – General server-side failure.
- 502 Bad Gateway – Server acting as a gateway/proxy received an invalid response.
- 503 Service Unavailable – Server is temporarily unavailable.
- 504 Gateway Timeout – Server acting as a gateway/proxy timed out.
```

References:
- This [Example blog](https://canro91.github.io/2022/12/01/TestingHttpClient/) shows a good example
of using moq for a simulated api testing as well as a way to streamline setting up the fake client.
- [Official Repo](https://github.com/devlooped/moq?tab=readme-ov-file#readme)



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

11/11
- Added a PDF Writer 
- Read Articles on how to use PDFsharp and it's relevant classes
- Cleaned and refactored code 

11/12
- Added Test for PDF Writer
- Overhauled UI Design
- Installed Fine Code Coverage for both devices.

11/18
- Added Tests for HTPPClient
- Catching errors upon bad requests to API
- Made Wrapper method for RelayCommand Logic

11/20
- Redesigned UI for DB Integration
- Beautified XAML wtih Extensions

11/26
- Configured Sqlite Local Database for development
- Fixed Issue finding the correct path for the database
- Deleteed old .db file that was found in production bin

11/30
- Added CRUD Operations for User Workouts
