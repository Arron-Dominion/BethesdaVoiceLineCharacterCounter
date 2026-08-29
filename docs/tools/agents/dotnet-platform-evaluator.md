# .NET Platform Evaluator Guide

## Purpose

The `.NET Platform Evaluator` is a repository-specific GitHub Copilot custom agent for assessing whether this application can safely upgrade to a proposed .NET release while continuing to support desktop Windows and Linux.

The agent evaluates more than target framework compatibility. It also determines whether the UI framework, application layers, tests, native dependencies, publishing configuration, packaging, or CI workflows should be retained, upgraded, isolated, or replaced.

The agent definition is stored at:

```text
.github/agents/dotnet-platform-evaluator.agent.md
```

Every completed evaluation creates a new immutable Markdown report under:

```text
docs/reports/
```

## When To Use It

Run the evaluator when:

- Planning an upgrade to a new .NET release.
- Checking whether the current .NET version is still supported.
- Adding or confirming Windows or Linux support.
- Upgrading or replacing Avalonia or another UI framework.
- Introducing a package with native or platform-specific dependencies.
- Changing runtime identifiers, publish settings, installers, or Linux packages.
- Preparing a major application release.
- Reviewing whether architectural changes have introduced platform coupling.
- Verifying cross-platform release readiness after a migration.

A useful minimum cadence is:

- Before beginning a major .NET or UI framework upgrade.
- After completing the upgrade and packaging work.
- Before each major public release.
- When Microsoft, Avalonia, or a critical dependency changes its support policy.

## How To Run The Agent

### Run from the agent selector

1. Open GitHub Copilot Chat in this workspace.
2. Open the agent selector.
3. Select `.NET Platform Evaluator`.
4. Enter the evaluation request.
5. Allow the agent to inspect files, run non-destructive commands, consult current documentation, and create its report.
6. Open the report linked in the agent's final response.

### Example requests

Evaluate a specific .NET upgrade:

```text
Evaluate upgrading this solution to .NET 11 while preserving supported Windows 10/11 x64 and desktop Linux x64 releases.
```

Audit the current state against the latest stable .NET release:

```text
Audit the current solution for the latest stable supported .NET release and determine whether any UI or other component must be replaced.
```

Focus on a UI framework upgrade:

```text
Evaluate whether the current Avalonia version can be upgraded while retaining Windows and Linux support. Include package, publish, and test impacts.
```

Evaluate release readiness:

```text
Evaluate the current codebase and packaging configuration for Windows and Linux release readiness. Use the currently targeted .NET version.
```

Narrow the scope when necessary:

```text
Evaluate the proposed package changes on this branch for .NET 10, Windows x64, and Linux x64 compatibility. Do not assess ARM64.
```

## What The Agent Does

For each run, the agent:

1. Identifies the requested .NET target or assumes the latest stable supported release.
2. Generates a UTC timestamp for the report filename.
3. Inspects solution and project configuration, package versions, source boundaries, UI code, tests, publishing, packaging, and CI.
4. Records the installed .NET SDK and workload environment when available.
5. Runs focused, non-destructive build and test diagnostics.
6. Assesses Windows and Linux separately.
7. Checks current support claims against authoritative documentation.
8. Classifies findings by severity and confidence.
9. Assigns a `Retain`, `Upgrade in place`, `Isolate`, or `Replace` decision to each major component.
10. Writes and links a timestamped report.

The evaluator does not implement migrations or modify product code. Its only expected repository write is the new report.

## Reports

### Filename format

Reports use a UTC timestamp with milliseconds:

```text
dotnet-platform-evaluation-YYYYMMDDTHHmmssfffZ.md
```

Example:

```text
docs/reports/dotnet-platform-evaluation-20260829T143012123Z.md
```

### Immutability

Treat generated reports as historical records of the repository, environment, and external support policies at the time of evaluation.

- Do not edit an old report to represent a later state.
- Run the evaluator again to produce a new report.
- Keep reports used for upgrade or release decisions in source control.
- A report may become outdated as framework and operating-system support policies change.

### Expected contents

Each report includes:

- Executive summary and final recommendation.
- Current SDK, framework, package, UI, test, and release baseline.
- Windows and Linux support matrix.
- Findings ordered by severity.
- Dedicated UI layer assessment.
- Decisions for Domain, Application, UI, tests, packaging, and CI.
- Commands and validations performed.
- Recommended implementation sequence.
- Release and packaging impact.
- Authoritative sources with access dates.
- Assumptions, unavailable checks, and limitations.

### Interpreting platform status

The report uses these terms:

| Status | Meaning |
| --- | --- |
| `Supported` | Repository and authoritative documentation support the target, subject to stated minimum versions. |
| `Unsupported` | The component or framework does not support the target. |
| `Conditional` | Support depends on stated changes, native packages, operating-system versions, or deployment constraints. |
| `Not verified` | Available evidence is insufficient or the required target environment was not tested. |

A successful portable library build does not prove that a graphical application runs on Linux. Review compile, publish, package, and clean-machine execution results separately.

### Severity

| Severity | Meaning |
| --- | --- |
| `Blocker` | Prevents the requested upgrade or supported Windows/Linux delivery. |
| `High` | Likely runtime, lifecycle, security servicing, or packaging failure. |
| `Medium` | Material maintainability, testing, portability, or release risk. |
| `Low` | Improvement that does not currently prevent delivery. |
| `Informational` | Relevant context with no required action. |

Findings also include `High`, `Medium`, or `Low` confidence. Low confidence is a signal to perform the missing target-machine or package verification before making a release decision.

## Prerequisites

For the most complete evaluation, the development environment should provide:

- A Git checkout with the solution and project files available.
- The proposed .NET SDK, when evaluating executable build compatibility.
- Network access to official Microsoft, Avalonia, and package-owner documentation.
- Restored NuGet assets or access to all configured package feeds.
- Windows for Windows-specific publish or launch checks.
- Linux or Linux CI for definitive Linux package and launch checks.

The agent must still produce a report when a prerequisite is missing. Missing tools and unperformed checks are recorded under `Assumptions And Limitations` and must not be presented as successful validation.

## Maintaining The Agent

### Source of truth

Edit only the custom agent definition:

```text
.github/agents/dotnet-platform-evaluator.agent.md
```

Do not copy the agent into user-level prompts for repository work. The workspace definition should remain the shared source of truth for contributors.

### Frontmatter requirements

The file begins with YAML frontmatter. Preserve:

- `name`: The display and invocation name.
- `description`: The discovery text used by Copilot to select the agent for relevant requests.
- `tools`: Capabilities available to the evaluator.
- `user-invocable: true`: Keeps the agent visible in the agent selector.
- `argument-hint`: Gives users an example of the expected request.

Descriptions should continue to contain concrete discovery terms such as `.NET upgrade`, `Windows`, `Linux`, `UI framework`, `component replacement`, and `release readiness`.

Quote YAML values that contain punctuation such as colons. Use spaces rather than tabs in frontmatter.

### Tool permissions

The current tool aliases are:

| Tool | Reason |
| --- | --- |
| `read` | Inspect project files, source, reports, and configuration. |
| `search` | Locate platform APIs, package usage, and architecture boundaries. |
| `web` | Verify current support and lifecycle information. |
| `execute` | Run SDK, restore, build, test, and publish diagnostics. |
| `edit` | Create the required report. |

Do not remove `edit` unless report creation is moved to a deterministic hook or another writing mechanism. Do not add broad capabilities unless a concrete evaluation requirement needs them.

The agent body restricts writes to reports even though the `edit` alias is technically broader. Review agent-generated changes after each run with `git status --short`.

### Updating evaluation scope

When the supported product matrix changes, update all relevant parts together:

1. The frontmatter description and argument hint.
2. The `Scope` section.
3. The `Required Workflow` and `Evaluation Rules`.
4. The required report template.
5. The completion criteria.
6. This human-readable guide.

Examples include adding ARM64 as a first-class target, dropping Windows 10, changing the UI framework, or adopting a new package format.

### Updating the report contract

When changing report sections:

- Keep the executive summary, support matrix, findings, validation, sources, limitations, and final decision sections.
- Preserve separate Windows and Linux conclusions.
- Preserve the explicit UI decision.
- Preserve explicit decisions for other major components.
- Add new required sections to both the agent definition and this guide.
- Do not rename historical report files.

### Changing the filename format

If the filename convention must change:

1. Update the report path in the agent workflow.
2. Update the required report completion criteria.
3. Update `docs/reports/README.md`.
4. Update this guide.
5. Continue to use UTC and enough timestamp precision to avoid collisions.
6. Do not rename existing historical reports solely to match the new convention.

## Validation After Agent Changes

After editing the agent definition:

1. Confirm VS Code discovers `.NET Platform Evaluator` in the agent selector.
2. Check the customization file for YAML or Markdown diagnostics.
3. Start a small evaluation with an explicit target version.
4. Confirm a new uniquely timestamped file appears under `docs/reports`.
5. Confirm the report contains every required heading.
6. Confirm Windows and Linux are evaluated separately.
7. Confirm the UI receives an explicit retain, upgrade, isolate, or replace decision.
8. Confirm command failures and skipped validations are reported accurately.
9. Confirm authoritative sources include access dates and URLs.
10. Run `git status --short` and verify that the report is the only unexpected write.

A useful validation prompt is:

```text
Evaluate the current target framework and UI layer for Windows x64 and Linux x64. Keep the run focused, but produce the complete required report.
```

## Troubleshooting

### The agent does not appear in the selector

- Confirm the file is under `.github/agents` and ends in `.agent.md`.
- Confirm the YAML frontmatter starts and ends with `---`.
- Confirm `user-invocable` is `true`.
- Confirm the `description` value is present and valid YAML.
- Reload the VS Code window after changing customization files.
- Check the file for editor diagnostics.

### Copilot does not select the agent automatically

- Select it manually from the agent selector.
- Make the request use terms present in the description, such as `.NET upgrade`, `Windows and Linux compatibility`, `UI replacement`, or `release readiness`.
- Improve the frontmatter description if a recurring trigger phrase is missing.

### No report is created

- Confirm the agent has the `edit` tool alias.
- Confirm the repository permits creating files under `docs/reports`.
- Remind the agent that the report is mandatory and a chat response is not a substitute.
- Check whether a tool failure is shown in chat.
- Run the evaluation again; do not manually convert the chat response into an official report unless the failed run is clearly labeled as incomplete.

### The report overwrites another report

This violates the agent contract. Restore the original report if necessary, then update the agent instructions to regenerate the UTC timestamp when a filename collision exists. Every run must create a new file.

### Linux is marked supported without Linux execution

The report should distinguish documented framework support from verified application execution. Without a Linux build, package, or launch check, the relevant execution status should be `Not verified` or `Conditional`, with the missing validation documented.

### The evaluation changes product code

Stop the run and review the working tree. The evaluator is not a migration implementation agent. Revert only the unintended changes from that run, preserving unrelated user work, and strengthen the no-product-code constraint if the behavior recurs.

## Review And Retention

Reports are engineering decision records, not permanent proof of compatibility. During review:

- Confirm cited support policies were current on the report date.
- Challenge unsupported claims and low-confidence conclusions.
- Require target-machine verification before public release.
- Link significant reports from upgrade plans, pull requests, or release notes.
- Retain reports that informed shipped releases or major architecture decisions.
- Remove only reports that were accidental, empty, or clearly incomplete and never used for a decision.

The implementation plan for the initial .NET 10 and Avalonia migration is stored at `docs/dotnet-10-avalonia-migration-plan.md`. Use evaluator reports to confirm assumptions before implementation and to verify the resulting Windows/Linux architecture afterward.
