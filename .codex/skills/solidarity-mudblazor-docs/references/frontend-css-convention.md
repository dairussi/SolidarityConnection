# Frontend CSS convention

Follow this repository convention when editing MudBlazor-based components.

## Rule

Keep component-specific CSS in the same `.razor` file in a single `<style>` block whenever you create a new component or substantially refactor an existing one.

## Apply the rule safely

- If the component already has a `<style>` block, continue there.
- If the component already uses `.razor.css` for a small existing tweak, avoid migrating it unless the user asked for standardization.
- Do not create a new `.razor.css` file when the component is already following the same-file pattern.
- Avoid mixing inline styles, `.razor.css`, and global CSS for the same concern.

## Prefer these existing patterns

- `SolidarityConnection.Frontend/Pages/Login.razor`
- `SolidarityConnection.Frontend/Pages/Home.razor`
- `SolidarityConnection.Frontend/Shared/AppMenu.razor`
- `SolidarityConnection.Frontend/Layout/MainLayout.razor`
