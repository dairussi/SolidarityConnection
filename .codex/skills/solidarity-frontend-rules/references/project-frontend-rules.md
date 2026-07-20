# Project frontend rules

This repository already shows a preferred direction for frontend work.

## Use these local examples as anchors

- `SolidarityConnection.Frontend/Pages/Login.razor`
- `SolidarityConnection.Frontend/Pages/Home.razor`
- `SolidarityConnection.Frontend/Shared/AppMenu.razor`
- `SolidarityConnection.Frontend/Layout/MainLayout.razor`

## Conventions

- Prefer MudBlazor for common UI composition.
- Keep local CSS in the same `.razor` file for new or heavily revised components.
- Preserve current navigation, spacing, and card-like composition patterns unless the task asks for a redesign.
- Treat the frontend as a WebAssembly app talking to the backend API.
- Normalize every edited file before finishing. This repository uses `end_of_line = crlf` in `.editorconfig`.

## Routing to sibling skills

- Use `$solidarity-blazor-docs` for framework-level questions.
- Use `$solidarity-mudblazor-docs` for component catalog and prop decisions.
