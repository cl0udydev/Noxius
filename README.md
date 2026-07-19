# Noxius

Noxius is a desktop audio player built with C# and Avalonia UI. It features a compact, single-window user interface inspired by the classic AIMP layout, using a dark purple color palette with neon accents.

The application follows the MVVM pattern for clean separation of UI and player logic.

## Key Features

- **AIMP-style Layout:** Compact controls, track position slider, and playlist integrated into a single view.
- **Cross-Platform:** Runs on Windows, Linux, and macOS via Avalonia UI.
- **Custom Theme:** Dark purple UI styling with pink and cyan accents.
- **MVVM Pattern:** Built using Avalonia's MVVM templates for easier maintenance.

## Repository Structure

- `Views/` — XAML windows and control layouts.
- `ViewModels/` — ViewModels handling UI state and command bindings.
- `Models/` — Data structures (tracks, playlist items) and player logic.
- `Assets/` — Application icons and static resources.

## Requirements

- .NET 8.0 SDK or newer
- IDE with Avalonia support (JetBrains Rider or Visual Studio 2022)

## Building from Source

1. Clone this repository:
   ```bash
   git clone https://github.com/cl0udydev/Noxius.git
   ```

2. Go to the project folder:
   ```bash
   cd Noxius
   ```

3. Build and launch the application:
   ```bash
   dotnet run
   ```

## Tech Stack

- **Language:** C#
- **Framework:** Avalonia UI (MVVM)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
