# Blazor official docs

Use Microsoft Learn as the default source.

## Primary URLs

- Blazor overview: https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-8.0
- Blazor hosting models: https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-8.0
- Blazor render modes: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0
- Razor components: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/?view=aspnetcore-8.0
- ASP.NET Core web API guidance: https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-8.0
- SignalR with Blazor: https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-8.0

## How to interpret them for this repository

- This repo uses a dedicated WebAssembly frontend project and a separate ASP.NET Core backend project.
- Microsoft Learn for .NET 8+ often explains newer Blazor Web App render modes. That is still useful for concepts, but do not assume this repo should be reshaped to the latest template without an explicit request.
- The hosting models article is the best starting point when the user says "hosted," "WebAssembly," "server," "render no cliente," or "render no servidor."
- The render modes article matters when comparing old hosted WebAssembly patterns with newer .NET 8+ guidance.

## Search hints

- Search: `site:learn.microsoft.com aspnet core blazor <topic>`
- Search: `site:learn.microsoft.com blazor render modes <topic>`
- Search: `site:learn.microsoft.com blazor webassembly <topic>`

## Local files to inspect before changing code

- `SolidarityConnection.Frontend/SolidarityConnection.Frontend.csproj`
- `SolidarityConnection.Frontend/Program.cs`
- `SolidarityConnection.Frontend/App.razor`
- `SolidarityConnection.Frontend/_Imports.razor`
- `SolidarityConnection.Presentation/SolidarityConnection.Presentation.csproj`
- `SolidarityConnection.Presentation/Program.cs`
