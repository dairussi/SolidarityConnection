---
name: solidarity-frontend-rules
description: Use for any frontend work in SolidarityConnection.Frontend, especially when implementing, reviewing, refactoring, or styling Razor components, pages, layouts, forms, navigation, or MudBlazor UI. Prefer this skill for project-specific frontend conventions, then consult the sibling Blazor and MudBlazor skills as directed.
---

# Solidarity Frontend Rules

Use this as the default entry skill for frontend work in this repository.

## Default behavior

1. Start here for any task inside `SolidarityConnection.Frontend`.
2. Read `references/project-frontend-rules.md` first.
3. If the task is about Blazor architecture, lifecycle, routing, hosting, HttpClient, or client/server boundaries, also use `$solidarity-blazor-docs`.
4. If the task is about choosing or configuring UI components, forms, alerts, dialogs, typography, layout, or input controls, also use `$solidarity-mudblazor-docs`.
5. Keep component-specific CSS in the same `.razor` file when creating or substantially refactoring a component.
6. Normalize every edited file before finishing the task, following the project's `.editorconfig` line ending rule.

## Scope

- Pages
- Shared components
- Layouts
- Authentication screens
- MudBlazor forms and interactions
- Frontend refactors that need to preserve repository conventions

## Rules

- Inspect nearby files before introducing a new pattern.
- Reuse the existing visual language unless the user asks for a redesign.
- Prefer same-file `<style>` blocks for component-local CSS.
- Avoid introducing a new `.razor.css` file when the component already follows the same-file pattern.
- Use the project's existing Blazor WebAssembly plus ASP.NET Core split as the default architectural assumption.
- Normalize edited files to the repository standard before ending the work. In this project, `.editorconfig` sets `end_of_line = crlf`.

## References

- Read `references/project-frontend-rules.md` first.
- Then route to `$solidarity-blazor-docs` or `$solidarity-mudblazor-docs` when the task needs official framework or component documentation.
