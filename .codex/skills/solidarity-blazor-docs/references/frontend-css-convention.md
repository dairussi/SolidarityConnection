# Frontend CSS convention

Follow this repository convention when editing Blazor UI files.

## Rule

Keep component-specific CSS in the same `.razor` file in a single `<style>` block whenever you create a new component or substantially refactor an existing one.

## Why

- Recent pages and shared components in this repo already use inline `<style>` blocks.
- Keeping markup and CSS together makes maintenance easier for the current team workflow.

## Apply the rule safely

- If the target component already has a same-file `<style>` block, extend that block.
- If the target component already uses a separate `.razor.css` file and the task is a small tweak, it is acceptable to keep the existing file to avoid unnecessary churn.
- Do not split one component's styling across multiple new places without a reason.
- If the user asks to standardize or refactor the component styling, prefer consolidating it into the `.razor` file instead of keeping a mixed setup.

## Local examples of the preferred pattern

- `SolidarityConnection.Frontend/Pages/Login.razor`
- `SolidarityConnection.Frontend/Pages/Home.razor`
- `SolidarityConnection.Frontend/Shared/AppMenu.razor`
- `SolidarityConnection.Frontend/Layout/MainLayout.razor`
