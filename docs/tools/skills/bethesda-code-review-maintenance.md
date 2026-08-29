# Bethesda Code Review Skill Maintenance

## Purpose

The Bethesda code review skill gives GitHub Copilot repository-specific instructions for reviewing pull requests, commits, diffs, and changed files in this project.

The skill supplements general review knowledge with the conventions and failure modes that matter here, including:

- .NET project boundaries and generated version metadata.
- Application calculations and boundary-value tests.
- Avalonia XAML, compiled bindings, keyboard access, and accessibility.
- Windows publishing and Inno Setup packaging.
- Linux TAR.GZ and Debian packaging.
- GitHub Actions artifact upload, download, and release behavior.
- Documentation that must stay synchronized with release behavior.

The skill definition is stored at:

```text
.github/skills/bethesda-code-review/SKILL.md
```

This guide is for people maintaining that definition. The skill file remains the source of truth for Copilot.

## When To Use It

Use the skill when reviewing:

- A pull request or branch diff.
- A commit or range of commits.
- Uncommitted workspace changes.
- A specific C#, XAML, project, test, workflow, script, or packaging file.
- Release preparation changes that span Windows and Linux.

The skill is designed for defect-oriented review. It prioritizes correctness, regressions, accessibility, compatibility, packaging, release integrity, and missing tests. It should not manufacture findings for optional style changes.

## How To Invoke It

### Automatic discovery

Compatible Copilot review environments can discover the skill from its frontmatter description when a request mentions code review, pull requests, diffs, Avalonia, packaging, release artifacts, or another listed trigger.

### Manual invocation

In Copilot Chat, invoke the skill directly:

```text
/bethesda-code-review Review the current changes.
```

Narrow the target when useful:

```text
/bethesda-code-review Review the changes to the Linux packaging script and Debian metadata.
```

```text
/bethesda-code-review Review the current branch against main, focusing on Avalonia accessibility and regressions.
```

```text
/bethesda-code-review Review .github/workflows/build-release.yml for release artifact failures.
```

The skill does not require every review to run every build and packaging command. It directs the reviewer to choose the cheapest check that can confirm or disprove each candidate finding.

## What The Skill Does

The review workflow requires Copilot to:

1. Establish the changed files and comparison base.
2. Read the owning implementation and directly affected callers or consumers.
3. Classify changes by project layer or release subsystem.
4. Form a falsifiable hypothesis before reporting a defect.
5. Run focused validation when the environment permits it.
6. Check relevant tests and behavioral boundaries.
7. Re-read the changed line before leaving a comment.
8. Report actionable findings in severity order or state clearly that no findings remain.

This evidence-first sequence is intentional. Repository review comments have previously mixed valid defects with assumptions imported from other frameworks or incorrect interpretations of build tooling.

## Skill Structure

### Frontmatter

The YAML frontmatter controls discovery and invocation:

| Field | Purpose |
| --- | --- |
| `name` | Must be `bethesda-code-review` and match the containing folder. |
| `description` | Supplies searchable trigger terms for automatic discovery. |
| `argument-hint` | Shows the expected review target for manual invocation. |
| `user-invocable: true` | Exposes `/bethesda-code-review` in Copilot Chat. |
| `disable-model-invocation: false` | Allows automatic selection when a request matches the description. |

Keep the `name` lowercase and hyphenated. Keep the description below 1,024 characters and quote it because it contains punctuation.

When adding a new review domain, include its likely user terms in the description only if automatic discovery would otherwise miss relevant requests. Do not turn the description into a complete copy of the skill body.

### Review standard and workflow

These sections define what qualifies as a finding. Preserve the requirements to:

- Review changed behavior rather than the whole repository indiscriminately.
- Identify a concrete trigger and impact.
- Avoid reporting pre-existing issues unless the change worsens them.
- Run cheap, focused checks before asserting compile, package, or runtime failure.
- Separate confirmed defects from checks that require another operating system or environment.

Loosening these rules increases speculative comments. Making them too rigid can suppress legitimate findings where runtime validation is unavailable. In that case, the reviewer should state the limitation and use an appropriate confidence level.

### Repository orientation

This section gives the reviewer a starting map of Domain, Application, UI, and test projects. Update it when:

- A project is added, removed, or renamed.
- Dependency direction changes intentionally.
- The target framework or UI framework changes.
- Supported operating systems or architectures change.
- The solution entry point changes.

Treat these statements as orientation rather than permanent facts. The skill already instructs reviewers to verify them against current files.

### Validation commands

Keep commands executable from the repository root. Update them when solution names, project paths, target frameworks, configurations, packaging entry points, or artifact names change.

The baseline commands are:

```powershell
dotnet restore .\BethesdaVoiceLineCharacterCounter.slnx
dotnet build .\BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj -c Release --no-restore
dotnet test .\BethesdaVoiceLineCharacterCounter.slnx -c Release --no-restore
git diff --check
```

The forced rebuild is important for suspected Avalonia XAML resolution problems:

```powershell
dotnet build .\BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj -c Release --no-restore -t:Rebuild
```

Do not add expensive validation to every review by default. Add a command when it discriminates a meaningful class of repository-specific failures.

### Subsystem checks

Each subsystem section records invariants that reviewers should check when related files change.

| Section | Keep synchronized with |
| --- | --- |
| Domain and Application | Domain models, application features and utilities, view models, and their tests |
| Avalonia and accessibility | App and view XAML, code-behind, view models, Avalonia package versions, and UI tests |
| Windows packaging | PowerShell packaging script, Inno Setup script, Windows publish profile, project metadata, and maintenance guide |
| Linux packaging | Bash packaging script, Debian control template, desktop file, Linux publish profile, and maintenance guide |
| GitHub Actions and release | Workflow action versions, artifact patterns, download layout, release command, and package filenames |
| Documentation | README, packaging guides, supported platforms, commands, and artifact names |

Prefer concise invariants over copying entire source files or maintenance guides into the skill. The skill should tell the reviewer what relationship to inspect and link to detailed guides where necessary.

## Lessons Encoded In The Skill

These safeguards come from actual review feedback and should remain unless the underlying implementation or dependency behavior changes.

### Debian control files require a final newline

The Linux package build failed when `packaging/linux/control` ended during the multiline `Description` value without a final LF. The skill requires reviewers to check the final newline and continuation-line indentation.

Remove or revise this safeguard only if Debian metadata generation moves to a structured tool that guarantees valid output. Even then, retain a package-level validation with `dpkg-deb`.

### Packaging versions must reach generated assembly metadata

Passing `-p:Version` does not repair separately hard-coded `AssemblyVersion`, `FileVersion`, or `InformationalVersion` properties. The skill tells reviewers to inspect emitted binary metadata rather than relying only on an unevaluated MSBuild property query.

The project-level `<Version>` remains a valid local default because command-line `-p:Version` overrides it. Do not flag the fallback merely because a release uses another version.

### GitHub artifact paths follow action semantics

The current upload paths contain a wildcard after `artifacts/packages/`. `upload-artifact` uses the path before the first wildcard as the artifact root, so that prefix is not recreated inside the uploaded artifact. The release job also uses `merge-multiple: true`, which extracts both platform artifacts into one destination.

The skill therefore rejects unsupported claims about an automatic `artifacts/artifacts/packages` nesting. Re-check the official documentation for the exact action major version whenever action versions, wildcard positions, `path`, or `merge-multiple` change.

### Avalonia is not WPF

Avalonia's `Label` owns the `Target` property used for access-key focus transfer. `AccessText` does not have that property. The correct project pattern is:

```xml
<Label Target="GameSelector"
       Content="_Game" />
```

Framework API claims should be checked against the referenced Avalonia version or the Avalonia XAML compiler before becoming review findings.

### FluentTheme resolves through Avalonia's default namespace

With the `Avalonia.Themes.Fluent` package referenced, `<FluentTheme />` resolves through:

```xml
xmlns="https://github.com/avaloniaui"
```

An additional XML prefix is not required by the current project. Keep the skill's safeguard until a package upgrade or namespace change proves otherwise with a forced XAML rebuild.

## Maintaining Repository Facts

Review the skill whenever any of these change:

- `global.json` or target frameworks.
- Avalonia or CommunityToolkit.Mvvm versions.
- Solution or project names and dependency boundaries.
- Supported games or character-limit rules.
- Windows or Linux runtime identifiers.
- Publish profiles or self-contained deployment behavior.
- Inno Setup identity, paths, or artifact names.
- Debian package identity, paths, dependencies, or metadata generation.
- GitHub Actions versions, artifact paths, merge behavior, or release commands.
- Test project names or standard validation commands.

When a repository fact changes:

1. Update the implementation and its tests or packaging validation.
2. Update the relevant README or detailed maintenance guide.
3. Update the corresponding invariant in the skill.
4. Update this guide when the maintenance procedure or lesson changes.
5. Run the skill against the resulting diff to check whether its advice still fits the repository.

## Adding A New Review Rule

Add a rule only when it addresses a recurring or high-impact failure mode.

1. Identify the concrete failure and affected files.
2. Decide which subsystem owns the rule.
3. Phrase the rule as an observable relationship or validation, not a guessed implementation preference.
4. Include a disconfirming check when an API, tool, or package behavior can be tested.
5. Avoid embedding volatile details when the reviewer can cheaply read them from the repository.
6. Add discovery terms to the frontmatter only when the new domain should trigger automatic skill selection.
7. Test the updated skill with both a known-valid change and a deliberately broken example when practical.

A useful rule explains what can fail, where to look, and how to verify it. A poor rule merely says that code should be clean, accessible, or correct.

## Removing Or Revising A Rule

Do not preserve a repository-specific rule after its premise becomes false. Revise or remove it when:

- A dependency changes the relevant API or behavior.
- A script is replaced by a structured packaging tool.
- A platform or artifact is no longer supported.
- A deterministic test or lint rule now enforces the invariant more accurately.
- The rule repeatedly causes false positives despite correct repository behavior.

Before removal, search for the original failure mode and confirm another test, tool, or instruction still protects against regression.

## Validation After Skill Changes

After editing `.github/skills/bethesda-code-review/SKILL.md`:

1. Confirm the folder name and frontmatter `name` both equal `bethesda-code-review`.
2. Confirm the YAML frontmatter begins and ends with `---`.
3. Confirm `description` remains specific, quoted, and no longer than 1,024 characters.
4. Confirm `user-invocable: true` and `disable-model-invocation: false` remain present.
5. Keep `SKILL.md` below 500 lines. Move detailed background into this guide or another referenced resource if necessary.
6. Check the file for editor diagnostics.
7. Run `git diff --check`.
8. Confirm `/bethesda-code-review` appears in Copilot Chat after reloading the workspace if necessary.
9. Invoke the skill against a small representative diff.
10. Confirm findings are tied to changed lines, describe a trigger and impact, and distinguish validation limitations.

Use these regression scenarios when changing the corresponding section:

| Scenario | Expected review result |
| --- | --- |
| Debian control template lacks a final LF | Report the package parsing risk. |
| Explicit assembly versions block `-p:Version` | Report inconsistent shipped version metadata. |
| Current artifact upload and merged download paths | Do not invent nested package directories. |
| `AccessText` is given a `Target` property | Reject the invalid Avalonia API; recommend a targeted `Label`. |
| Unqualified `FluentTheme` with the current package and default namespace | Do not report a namespace error when XAML compilation succeeds. |

## Troubleshooting

### The skill is not discovered

- Confirm the file is exactly `.github/skills/bethesda-code-review/SKILL.md`.
- Confirm the frontmatter `name` matches the folder name.
- Confirm the description contains terms used in the review request.
- Confirm `disable-model-invocation` is `false`.
- Reload the VS Code window after changing customization files.
- Check the file for YAML or Markdown diagnostics.

### The slash command is missing

- Confirm `user-invocable` is `true`.
- Confirm the skill exists in the active workspace rather than only another checkout.
- Reload the VS Code window and reopen Copilot Chat.

### Reviews still contain speculative findings

- Check whether the relevant subsystem rule names a cheap disconfirming validation.
- Strengthen the evidence requirement instead of adding a conclusion that is always assumed true.
- Add a narrowly worded false-positive safeguard only after verifying the actual framework or tool behavior.
- Test the revised rule against both the reported case and a real defect.

### Reviews miss a recurring defect

- Confirm the changed file category is covered by the skill description and subsystem sections.
- Add the owning file relationships and failure condition, not just the latest symptom.
- Add or identify a focused test command that can expose the issue.
- Consider enforcing deterministic invariants in tests or CI instead of relying only on review instructions.

### Repository facts become stale

- Compare the skill's orientation and subsystem sections with the solution, project files, workflows, packaging scripts, and maintenance guides.
- Remove obsolete names and assumptions.
- Prefer instructions to inspect current values when exact versions or paths are likely to change.

## Review Ownership

Treat changes to the skill as engineering changes. A maintainer should verify:

- The rule reflects current repository behavior.
- The requested validation is available or clearly marked as platform-specific.
- The wording cannot be mistaken for a universal framework rule.
- The rule reduces a meaningful risk without encouraging noisy review comments.
- This guide and the skill remain consistent.

The best maintenance signal is review quality: confirmed defects should become easier to explain, while assumptions that can be disproved by the repository should stop appearing as findings.