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
- **Testing**:
  - xUnit for unit testing
  - moq (ver Moq 4.20.72 )
  - TDD (Test-Driven Development)

## References
- [ExerciseDB API](https://exercisedb-api.vercel.app/docs)
    -[API Ninja's API parameters](https://www.api-ninjas.com/api/exercises)

## <a name="testing"></a> Testing

### <a name="unit-testing"></a> Unit Testing
To isolate getting a json from the api, we first utilize Moq to simulate an api response for unit testing.
This allows us to simulate the different errors and avoid relying on any usage restrictions.

References:
- This [Example blog](https://canro91.github.io/2022/12/01/TestingHttpClient/) shows a good example
of using moq for a simulated api testing as well as a way to streamline setting up the fake client.
- [Official Repo](https://github.com/devlooped/moq?tab=readme-ov-file#readme)

### <a name="integration-testing"></a> Integration Testing


### Section for developer notes:

11/7:
project setup and initial pull
 
11/8:
- Able to pull from the Open api
- Added simple UI to display the parsed data and query
- Added unit tests for api handler