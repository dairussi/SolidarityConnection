---
name: solidarity-mudblazor-docs
description: Consult official MudBlazor documentation and repository-specific styling conventions when working on MudBlazor UI in this project. Use when Codex needs to choose components, configure props, improve forms, dialogs, alerts, layouts, icons, or understand MudBlazor APIs used by SolidarityConnection.
---

# Solidarity MudBlazor Docs

Use this skill for component selection and UI implementation in `SolidarityConnection.Frontend`.

## Quick workflow

1. Inspect the target `.razor` component and nearby UI patterns in the repository.
2. Read `references/mudblazor-official-docs.md` to find the official component page before inventing markup.
3. Reuse existing MudBlazor patterns already present in the project where possible.
4. Apply the styling rule in `references/frontend-css-convention.md` before editing CSS.
5. Normalize every edited file before finishing, following `.editorconfig`.

## Component selection rules

- Prefer official MudBlazor docs and examples from `mudblazor.com`.
- Start from the closest existing component already used in the repo before introducing a new pattern.
- Match the visual language already present in login, home, menu, and placeholder pages unless the user asks for a redesign.
- When deciding between raw HTML and MudBlazor, prefer MudBlazor for inputs, buttons, alerts, dialogs, spacing, typography, and paper/card surfaces.

## Implementation rules

- Keep MudBlazor props explicit when they affect appearance or behavior.
- Favor simple composition with `MudStack`, `MudPaper`, `MudText`, `MudButton`, `MudAlert`, and form fields before building custom wrappers.
- When styling a MudBlazor-based component, keep the CSS next to the `.razor` file whenever you create or substantially refactor that component.
- If you need to override MudBlazor classes, keep the overrides scoped and minimal.
- Normalize edited files to the repository standard before ending the task. In this project, `.editorconfig` sets `end_of_line = crlf`.

## References

- Read `references/mudblazor-official-docs.md` when you need official component pages, installation notes, or search hints.
- Read `references/frontend-css-convention.md` before editing styles.

## Examples

- "Use $solidarity-mudblazor-docs para escolher o melhor componente para filtro."
- "Use $solidarity-mudblazor-docs para montar um formulario com validacao."
- "Use $solidarity-mudblazor-docs para revisar as props desse MudTextField."
