---
name: bethesda-code-review
description: "Review pull requests, commits, diffs, and changed files in BethesdaVoiceLineCharacterCounter. Use for GitHub Copilot code review of .NET, C#, Avalonia XAML, tests, GitHub Actions, Windows Inno Setup, Linux DEB/TAR packaging, release artifacts, versioning, and documentation. Requires evidence-backed findings and checks repository-specific false-positive traps before commenting."
argument-hint: "Review the current changes or a specified pull request, commit, diff, or file."
user-invocable: true
disable-model-invocation: false
---

# Bethesda Code Review

Review changes for defects, regressions, release risks, and missing tests. Produce findings that are specific, actionable, and supported by the changed code or a directly affected call path.

Human maintainers should keep this skill synchronized with `docs/tools/skills/bethesda-code-review-maintenance.md`.

## Review Standard

- Review the change, not the repository in the abstract. Establish the comparison base and inspect the complete diff before reporting findings.
- Focus on behavior, correctness, accessibility, compatibility, security, packaging, and maintainability risks that can cause a concrete failure.
- Do not report style preferences, speculative concerns, or pre-existing issues unless the change materially worsens them.
- Trace each candidate issue from the changed line to the code that computes, consumes, packages, or presents the affected behavior.
- State the conditions needed to trigger the problem and the observable impact.
- Prefer one precise finding over several overlapping comments about the same root cause.
- Treat missing tests as a finding only when the change adds or alters meaningful behavior and a practical regression could escape the existing suite.
- Never claim that code will not compile, package, or run when a focused command can cheaply verify the claim. Run the check when tools permit.
- Distinguish confirmed defects from limitations that require another operating system, clean machine, installer, or desktop session to verify.

## Repository Orientation

Verify these facts against the current files when they are relevant; they are orientation, not substitutes for reading the diff.

- `BethesdaVoiceLineCharacterCounter.Domain` owns enums and domain models and should remain independent of higher layers.
- `BethesdaVoiceLineCharacterCounter.Application` owns DTOs, features, and calculation utilities and depends on Domain.
- `BethesdaVoiceLineCharacterCounter` is the .NET 10 Avalonia desktop UI and depends on Application and Domain.
- `BethesdaVoiceLineCharacterCounter.Application.Tests` covers application behavior. `BethesdaVoiceLineCharacterCounter.Test` covers view-model and presentation-adjacent behavior.
- `BethesdaVoiceLineCharacterCounter.slnx` is the solution entry point. Do not assume a legacy `.sln` or MAUI workload exists.
- Windows 10/11 x64 and desktop Linux x64 are first-class targets. Publishes are self-contained.

## Required Workflow

1. Identify changed files and read enough surrounding code to understand the owning behavior.
2. Classify each changed file into Domain, Application, UI, tests, CI, Windows packaging, Linux packaging, or documentation.
3. Follow changed symbols to callers, bindings, generated properties, scripts, package templates, or release steps that rely on them.
4. Form a falsifiable hypothesis for each candidate defect. Identify the cheapest check that could disprove it.
5. Run focused validation where available. Start with the touched project or subsystem, then widen only when risk warrants it.
6. Check whether tests cover new branches, boundary values, command eligibility, bindings, and package/release contracts introduced by the change.
7. Re-read the exact changed lines before writing each finding. Ensure the comment points to code the author can change in this pull request.
8. Report findings in descending severity. If no actionable defects remain, say so and name any validation that could not be performed.

## Validation Commands

Use commands appropriate to the environment and change. Do not report a check as passed unless it completed successfully.

```powershell
dotnet restore .\BethesdaVoiceLineCharacterCounter.slnx
dotnet build .\BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj -c Release --no-restore
dotnet test .\BethesdaVoiceLineCharacterCounter.slnx -c Release --no-restore
git diff --check
```

For a suspected Avalonia XAML resolution or binding problem, force XAML recompilation when incremental output could hide it:

```powershell
dotnet build .\BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj -c Release --no-restore -t:Rebuild
```

For version-flow changes, inspect the emitted binary rather than relying only on unevaluated MSBuild properties. A packaging version must reach the assembly version, file version, informational/product version, installer metadata, artifact names, and checksums as intended.

Run Linux package checks on Linux, WSL, or CI when available:

```bash
bash ./scripts/package-linux.sh 1.0.0-test
dpkg-deb --info artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-test-linux-x64.deb
dpkg-deb --contents artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-test-linux-x64.deb
```

Run the Windows packaging entry point when Windows packaging changes. Absence of Inno Setup is a validation limitation because the script can still create the portable ZIP:

```powershell
.\scripts\package-windows.ps1 -Version 1.0.0-test
```

## Domain And Application Checks

- Preserve dependency direction. Domain must not acquire UI, packaging, or application dependencies.
- Review character-limit and dialogue-section calculations at zero, exact limit, one above the limit, and multiple-section boundaries.
- When games are added or renamed, check the enum/model mapping, display name, character limit, generated game list, UI selection, and theory data together.
- For nullable inputs, verify both static nullability and runtime command eligibility. Do not accept null-forgiving operators as proof that a value is present.
- CommunityToolkit.Mvvm generated properties and commands must notify every dependent command or displayed result when their source values change.

## Avalonia And Accessibility Checks

- The project enables compiled bindings by default. Check `x:DataType`, binding paths, item-template data types, and two-way bindings against the actual view model and model types.
- Validate XAML API claims against the referenced Avalonia version or the XAML compiler. Do not import WPF assumptions into Avalonia.
- In Avalonia, `Label` owns the `Target` property used to focus an associated input on click or access-key press. `AccessText` itself does not expose `Target`.
- An unqualified `FluentTheme` can resolve through the default `xmlns="https://github.com/avaloniaui"` when `Avalonia.Themes.Fluent` is referenced. Do not require a namespace prefix without a failing build or version-specific evidence.
- Check keyboard focus, unique access keys, automation names, tab navigation, command enablement, text wrapping, and layout behavior for user-facing changes.
- A successful XAML build proves type/property resolution, not keyboard behavior or visual layout. Require a UI-level check when the suspected defect depends on runtime focus or rendering.

## Windows Packaging Checks

Read `docs/tools/package/windows-installer-maintenance.md` when reviewing `scripts/package-windows.ps1`, `packaging/windows/setup.iss`, the Windows publish profile, project version metadata, or release workflow.

- Treat the Inno Setup `AppId` as permanent installer identity. Changing it for an ordinary release breaks upgrade continuity.
- Keep the executable name, publish directory, installer source, shortcuts, run entry, product name, architecture, and artifact patterns aligned.
- The `-Version` argument must flow through `dotnet publish`, generated assembly metadata, `MyAppVersion`, output filenames, and checksum selection.
- `-p:Version` overrides the project file's default `Version`; do not flag the fallback value merely because it differs from a release tag.
- Explicit `AssemblyVersion`, `FileVersion`, or `InformationalVersion` properties can override the .NET SDK values derived from `Version`. Inspect evaluated or emitted metadata before claiming propagation works.
- The installer and portable ZIP intentionally consume the same complete self-contained publish directory.
- Windows and Linux publish independently to separate `win-x64` and `linux-x64` directories; do not report a cross-platform output collision unless those paths change.
- Do not claim the installer was validated when Inno Setup was unavailable or when installation, upgrade, launch, and uninstall were not exercised.

## Linux Packaging Checks

Read `docs/tools/package/linux-packaging-maintenance.md` when reviewing `scripts/package-linux.sh`, `packaging/linux/control`, the desktop entry, Linux publish profile, or release workflow.

- `packaging/linux/control` is a template. `@VERSION@` is replaced by the packaging script; it is not an unresolved hard-coded package version.
- Debian control metadata must end with LF. A missing final newline can make `dpkg-deb` fail while parsing a multiline `Description`.
- Continuation lines in a multiline Debian `Description` must begin with one space.
- Keep package identity, install directory, `/usr/bin` symlink, executable name, desktop `Exec`, desktop `Icon`, installed icon name, architecture, and dependencies aligned.
- The packaging script explicitly applies `chmod +x` before creating the TAR.GZ. Preserve that ordering and verify the archived host remains executable; do not infer Linux package validity from a Windows-only build.
- Verify both `.deb` and `.tar.gz` artifacts and their `.sha256` files when package selection logic changes.

## GitHub Actions And Release Checks

- Read action documentation for the exact major version before asserting upload or download path behavior. Do not infer archive layout from the source path alone.
- With the current `upload-artifact` wildcard paths, hierarchy before the first wildcard is not retained as an `artifacts/packages` prefix inside the artifact.
- With `download-artifact` and `merge-multiple: true`, matched artifacts are extracted into the same configured destination rather than separate artifact-name directories.
- Therefore, do not report nested `artifacts/artifacts/packages` paths or claim `artifacts/*` misses release files unless the workflow patterns or action behavior have changed and evidence supports it.
- Check tag normalization, prerelease-compatible version syntax, job dependencies, permissions, artifact-name uniqueness, checksum inclusion, and shell-specific quoting.
- A job that invokes `gh` without checking out the repository must provide repository context through `GH_REPO` or `--repo`; otherwise GitHub CLI may fail while trying to discover a Git repository.
- Account for both supported tag paths: a pushed tag may need `gh release create`, while publishing a release in the GitHub UI creates the release before packaging finishes and requires `gh release upload`.
- Do not use a separate release-existence check before creation. Attempt creation first, then fall back to upload only after confirming that the failed creation left or encountered an existing release; preserve unrelated failures.
- Confirm release globs select only intended package files and that a missing required artifact fails visibly rather than silently producing an incomplete release.

## Documentation Checks

- Require documentation changes when commands, supported platforms, artifact names, install paths, native dependencies, installer behavior, or maintenance procedures change.
- Do not request documentation churn for an implementation correction that restores already documented behavior.
- Cross-check README user instructions with the detailed Windows and Linux maintenance guides.

## Severity

- **Critical**: Data loss, credential exposure, arbitrary code execution, or a release process that can publish compromised artifacts.
- **High**: Build, startup, installation, upgrade, or core calculation failure for a supported target.
- **Medium**: User-visible incorrect behavior, accessibility failure, incomplete release, or meaningful regression likely to escape existing checks.
- **Low**: Narrow correctness or maintainability defect with limited impact. Do not use Low for optional polish.

## Finding Format

Use a short imperative title and include:

- Severity and confidence.
- A precise changed-file location.
- The triggering condition and observable impact.
- The repository evidence or failed validation that confirms the issue.
- A focused remediation direction without prescribing an unrelated refactor.

Keep summaries secondary to findings. Do not invent findings to fill severity categories. When no findings exist, state that clearly and list only material residual risks or checks that could not run.