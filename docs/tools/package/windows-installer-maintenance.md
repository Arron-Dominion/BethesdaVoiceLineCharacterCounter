# Windows Installer Maintenance

This guide explains how the Windows installer is built and how to maintain its Inno Setup configuration.

## Files involved

| File | Responsibility |
| --- | --- |
| `packaging/windows/setup.iss` | Defines the installer identity, destination, shortcuts, wizard behavior, and installed files. |
| `scripts/package-windows.ps1` | Publishes the application, creates the portable ZIP, compiles the installer, and generates SHA-256 checksums. |
| `BethesdaVoiceLineCharacterCounter/Properties/PublishProfiles/win-x64.pubxml` | Defines the self-contained Windows x64 publish settings. |
| `BethesdaVoiceLineCharacterCounter/BethesdaVoiceLineCharacterCounter.csproj` | Defines the product and default application version metadata. |

The PowerShell script is the normal entry point. Avoid compiling `setup.iss` manually for a release because the packaging script supplies the version and absolute input/output directories.

## Prerequisites

- The .NET SDK selected by `global.json`.
- Inno Setup 7 or 6 to create the installer.
- PowerShell 7 or Windows PowerShell 5.1.

The packaging script searches for `ISCC.exe` on `PATH` and in the standard Inno Setup 7 and 6 installation directories under `Program Files` and `Program Files (x86)`. If the compiler is unavailable, the script still creates the portable ZIP and prints a warning.

## Build the installer

Run this command from the repository root:

```powershell
.\scripts\package-windows.ps1 -Version 1.0.0
```

Use a three-part release version unless the release process deliberately requires a prerelease suffix. The supplied version controls the published assembly version, installer display version, artifact filenames, and checksum selection.

The script performs these steps:

1. Deletes and recreates `artifacts/publish/win-x64`.
2. Publishes a Release, self-contained `win-x64` application.
3. Creates the portable ZIP.
4. Calls the Inno Setup command-line compiler with release-specific preprocessor values.
5. Creates one `.sha256` file beside each Windows artifact.

Expected outputs:

```text
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-win-x64-portable.zip
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-win-x64-portable.zip.sha256
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-win-x64-setup.exe
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-win-x64-setup.exe.sha256
```

## Preprocessor values

The top of `setup.iss` defines three overridable values:

| Value | Purpose | Release value supplied by |
| --- | --- | --- |
| `MyAppVersion` | Installer version and output filename version. | `-Version` in `package-windows.ps1` |
| `PublishDir` | Directory whose complete contents are installed. | The script's `artifacts/publish/win-x64` path |
| `OutputDir` | Directory where Inno Setup writes the installer. | The script's `artifacts/packages` path |

The defaults make the script convenient to open and compile in the Inno Setup IDE, but release builds should use the PowerShell entry point.

## Installer identity

`AppId` is the permanent identity Windows uses to associate upgrades and uninstallation records:

```ini
AppId={{B901441A-3A27-4914-A405-87A94F533F6C}
```

Do not change this value for ordinary releases, product renames, or version updates. Changing it makes Windows treat the installer as a different product and can leave the previous version installed alongside it.

Update `AppName`, `AppPublisher`, or `MyAppExeName` only when the corresponding product metadata or executable name changes. If `MyAppExeName` changes, update all entries in `[Icons]` and `[Run]` that launch the executable.

## Install location and privileges

The installer currently uses:

```ini
DefaultDirName={autopf}\Bethesda Voice Line Character Counter
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
```

The normal default is therefore:

```text
C:\Program Files\Bethesda Voice Line Character Counter
```

The installer requests elevation because writing to Program Files requires administrator access. `PrivilegesRequiredOverridesAllowed=dialog` permits a user to select a current-user installation when needed. In current-user mode, Inno Setup resolves `{autopf}` to the user's Programs directory instead.

Keep `PrivilegesRequired=admin` if Program Files must remain the default. Setting it to `lowest` changes the default to a per-user location under the user's profile.

## Installed files

The `[Files]` section copies the entire self-contained publish directory:

```ini
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
```

This is intentional: a self-contained Avalonia deployment includes the application, .NET runtime, native libraries, Avalonia assemblies, and resources. Do not replace this with a short hand-maintained file list.

If a file must not ship, remove it from the publish output at the project or publish-profile level rather than excluding it only in Inno Setup. That keeps the portable and installed packages consistent.

## Shortcuts and post-install launch

The installer always creates a Start menu shortcut. A desktop shortcut is optional and is controlled by the `desktopicon` task.

The `[Run]` entry offers to launch the application after an interactive installation. The `skipifsilent` flag prevents automated or silent builds from unexpectedly starting the UI.

When changing shortcut names or locations, verify that uninstall removes them and that an upgrade does not create duplicate shortcuts.

## Compression and architecture

`Compression=lzma2` and `SolidCompression=yes` reduce installer size at the cost of compile time. The package supports x64-compatible Windows systems and installs in 64-bit mode.

Do not add ARM64 files to this installer. Create a separate runtime publish and installer artifact when ARM64 support is introduced.

## Validation

At minimum, perform these checks after changing `setup.iss` or the Windows packaging script:

1. Run the packaging command and confirm Inno Setup reports `Successful compile`.
2. Verify the installer and checksum files exist in `artifacts/packages`.
3. Install over an existing version and confirm the application is upgraded in place.
4. Launch the installed executable and complete the calculator workflow.
5. Confirm the Start menu shortcut and optional desktop shortcut work.
6. Uninstall and confirm the application files and shortcuts are removed.
7. Test on a clean Windows 10 or 11 x64 virtual machine before release.

For a non-elevated automated smoke test, use Inno Setup's `/CURRENTUSER` override and an isolated destination. This tests file deployment and uninstall behavior but does not prove the normal Program Files elevation flow:

```powershell
$installer = Resolve-Path .\artifacts\packages\BethesdaVoiceLineCharacterCounter-1.0.0-win-x64-setup.exe
$destination = Join-Path (Resolve-Path .\artifacts) installer-smoke-test

$install = Start-Process $installer `
    -ArgumentList '/CURRENTUSER', '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', "/DIR=$destination" `
    -Wait -PassThru

if ($install.ExitCode -ne 0) {
    throw "Installer failed with exit code $($install.ExitCode)."
}

$uninstall = Start-Process (Join-Path $destination unins000.exe) `
    -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART' `
    -Wait -PassThru

if ($uninstall.ExitCode -ne 0) {
    throw "Uninstaller failed with exit code $($uninstall.ExitCode)."
}
```

Do not automate the normal administrative mode from an unelevated terminal because its UAC prompt requires interactive approval.

## Common maintenance changes

### Change the product version

Pass the release version to `package-windows.ps1`. Do not edit the fallback `MyAppVersion` for each release.

### Change the default directory

Edit `DefaultDirName`. Keep `{autopf}` unless there is a specific reason to hard-code a Windows directory.

### Rename the executable

Update the assembly name or project output first, then update `MyAppExeName`, shortcut targets, launch entries, scripts, documentation, and CI artifact checks together.

### Add an installer icon

Create a Windows `.ico` containing common sizes and add this under `[Setup]`:

```ini
SetupIconFile=path\to\application.ico
```

The installed executable icon is controlled by the application project, not by `SetupIconFile`.

### Add code signing

Keep signing certificates and passwords outside the repository. Configure an Inno Setup signing command or sign the completed executable in CI using secret-backed credentials. Verify both the application executable and installer signature before publishing.

## Troubleshooting

### The portable ZIP exists but the installer does not

The script could not locate `ISCC.exe`, or Inno Setup compilation failed. Review the warning or compiler output and confirm Inno Setup is installed in a standard directory or available on `PATH`.

### The installer defaults to AppData

Confirm `PrivilegesRequired=admin` is still present and the installer was not started with `/CURRENTUSER`. Per-user mode intentionally changes `{autopf}` to a user-scoped Programs directory.

### The installer contains stale files

The packaging script recreates the publish directory, so stale files usually mean an installer was compiled manually against a different `PublishDir`. Delete `artifacts/publish/win-x64` and run the PowerShell packaging command again.

### Upgrade creates a second installation

Confirm `AppId` has not changed and that the previous installer used the same architecture and privilege mode.

### Silent validation reports no exit code

Use `Start-Process -Wait -PassThru` and read its `ExitCode`. Direct invocation of a Windows GUI-subsystem installer may not populate PowerShell's `$LASTEXITCODE` reliably.