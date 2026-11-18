# Uno Chefs + Infragistics UI

The [Uno Platform](https://platform.uno) **Chefs** sample app, with much of its UI rebuilt using [Infragistics UI for Uno Platform](https://www.infragistics.com/) (public beta).

Chefs is a cross-platform recipe app built with WinUI XAML, MVUX, Uno Toolkit and the Material theme. This version swaps native controls and third-party libraries for Infragistics charts, gauges, maps, data grid and input controls, and adds a new Meal Planner page.

## Hot Design and Hot Reload

The Infragistics controls work with Uno Platform's design-time tooling:

- **[Hot Design](https://platform.uno/hot-design/)** lets you select and edit Infragistics controls visually while the app is running: change properties, layout and bindings, and see the result straight away.
- **[Hot Reload](https://platform.uno/docs/articles/features/working-with-xaml-hot-reload.html)** writes those changes back to the XAML in your IDE or on the command line, so the source stays in sync with what you see in the running app.

You don't need to restart the app to iterate on the Infragistics UI.

## What uses Infragistics

| Page | Infragistics controls | Replaces |
|---|---|---|
| Recipe Details → Nutrition | `XamDoughnutChart` (macro split), `XamBulletGraph` (fat, carbs, protein vs. daily reference) | LiveCharts2 |
| Map / "Near me" | `XamGeographicMap` with OpenStreetMap imagery; contributor circles sized by recipe count | Mapsui + Bing tiles |
| Meal Planner (new) | `XamCategoryChart` + `Legend` (calories by day), `XamRadialGauge` (average daily calories vs. goal), `XamDataGrid` (weekly schedule grouped by day, with summaries) | — |
| Login | `XamButton`, `XamCheckbox` | Uno `Button`, `CheckBox` |
| Registration | `XamButton` | Uno `Button` |

[INFRAGISTICS_EVALUATION.md](INFRAGISTICS_EVALUATION.md) has the details: what was swapped, what was tried and reverted, required setup, and feedback for the Infragistics team.

## Getting started

### Prerequisites

- .NET SDK 10.0 (see [global.json](global.json))
- Uno Platform tooling: run [`uno-check`](https://platform.uno/docs/articles/external/uno.check/doc/using-uno-check.html) to set up your environment
- An IDE with the Uno Platform extension (Visual Studio, VS Code or Rider) for Hot Design and Hot Reload

### Run

```bash
dotnet run --project UnoChefsInfragistics/UnoChefsInfragistics.csproj -f net10.0-desktop
```

Or open `UnoChefsInfragistics.sln` and run the desktop target.

The project also targets Android, iOS, WebAssembly and Windows, but so far it has only been built and tested on `net10.0-desktop` (Skia).

### Infragistics packages

The app uses the `Infragistics.Uno.*.Trial` beta packages from nuget.org (Core, Charts, Gauges, Maps, Inputs, Layouts, DataGrid). Versions are in [Directory.Packages.props](Directory.Packages.props). The native UI factories are registered at startup in [App.xaml.cs](UnoChefsInfragistics/App.xaml.cs); some controls fail to construct without them.

## Project layout

```
UnoChefsInfragistics/
├── Views/          XAML pages and controls
├── Presentation/   MVUX models
├── Business/       Services and domain models
├── Client/         Generated API client (Kiota)
├── Styles/         Theme and control styles
└── Assets/         Images, icons, fonts
AppData/            Mock JSON data (used when UseMocks=true, the default)
```

## Credits

Based on the [Uno Chefs](https://github.com/unoplatform/uno.chefs) sample by Uno Platform.
