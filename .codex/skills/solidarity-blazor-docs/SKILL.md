---
name: solidarity-blazor-docs
description: Consult official Microsoft documentation and repository-specific conventions when working on Blazor in this project. Use when Codex needs to implement, review, debug, or explain Razor components, routing, dependency injection, HttpClient usage, authentication flow, project structure, frontend/backend interaction, or Blazor WebAssembly hosting questions for SolidarityConnection.
---

# Solidarity Blazor Docs

Use this skill for Blazor work in `SolidarityConnection.Frontend` and for integration points with `SolidarityConnection.Presentation`.

## Quick workflow

1. Inspect the local project first.
2. If the question is conceptual or version-sensitive, consult `references/blazor-official-docs.md`.
3. Prefer official Microsoft Learn pages over blogs, forums, or AI summaries.
4. Adapt the guidance to the repository's actual structure instead of forcing the latest template shape onto the codebase.
5. Apply the CSS convention in `references/frontend-css-convention.md` before editing any component styles.
6. Normalize every edited file before finishing, following `.editorconfig`.

## Project-specific guidance

- Treat this repository as a Blazor WebAssembly frontend plus ASP.NET Core backend, not as a stock template.
- Read the frontend `.csproj`, `Program.cs`, `App.razor`, `_Imports.razor`, and the target component before changing architecture decisions.
- Verify whether the task belongs in the frontend app, the presentation/API app, or both.
- When explaining "hosted WebAssembly," clarify that official guidance in .NET 8 and later often describes render modes and Blazor Web Apps, while older hosted WebAssembly guidance still helps for client/server split architectures.

## Decision rules

- For Razor syntax, component lifecycle, routing, layout, forms, validation, dependency injection, and rendering behavior, use Microsoft Learn first.
- For questions about whether something should run in the browser or on the server, check the hosting model and render mode guidance before coding.
- For API calls from the frontend, keep existing project patterns unless the user asked for an architectural refactor.
- For styling, keep component-specific CSS together in the same `.razor` file whenever you create or significantly refactor a component, following the reference file.
- Normalize edited files to the repository standard before ending the task. In this project, `.editorconfig` sets `end_of_line = crlf`.

## References

- Read `references/blazor-official-docs.md` when you need trusted URLs, search hints, or a reminder of which Microsoft docs to consult first.
- Read `references/frontend-css-convention.md` before editing component styles.

## Examples

- "Use $solidarity-blazor-docs para ajustar o fluxo de login em Blazor."
- "Use $solidarity-blazor-docs para entender onde colocar essa chamada HTTP."
- "Use $solidarity-blazor-docs para revisar se essa pagina deve ficar no cliente ou no servidor."
