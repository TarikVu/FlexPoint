# Felxare
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
  - TDD (Test-Driven Development)

### References
- [ExerciseDB API](https://exercisedb-api.vercel.app/docs)

### Section for developer notes:

11/7:
project setup and initial pull
 
Added the following packages to Testing directory for api testing
``bash
dotnet add package Moq
dotnet add package xUnit
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package Newtonsoft.Json
```