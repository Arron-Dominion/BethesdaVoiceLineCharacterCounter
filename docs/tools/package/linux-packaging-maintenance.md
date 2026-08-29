# Linux Packaging Maintenance

This guide explains how the portable Linux archive and Debian package are built, how their files are arranged, and which values must stay synchronized when the packaging process changes.

## Files involved

| File | Responsibility |
| --- | --- |
| `scripts/package-linux.sh` | Publishes the application, creates the TAR.GZ and DEB packages, and generates SHA-256 checksums. |
| `packaging/linux/control` | Defines Debian package identity, architecture, dependencies, maintainer, and description. |
| `packaging/linux/bethesda-voice-line-character-counter.desktop` | Defines the desktop application menu entry. |
| `BethesdaVoiceLineCharacterCounter/Properties/PublishProfiles/linux-x64.pubxml` | Defines the self-contained Linux x64 publish settings. |
| `BethesdaVoiceLineCharacterCounter/Resources/AppIcon/appicon.svg` | Supplies the scalable icon installed by the DEB package. |
| `.github/workflows/build-release.yml` | Runs Linux restore, tests, packaging, and artifact upload in CI. |

The Bash script is the release entry point. It generates both Linux package formats from the same clean self-contained publish directory so their application files remain consistent.

## Prerequisites

Build on a Linux system with:

- The .NET SDK selected by `global.json`.
- Bash.
- GNU `tar`.
- `dpkg-deb`, normally provided by the `dpkg` package.
- `sha256sum`, normally provided by GNU Coreutils.
- Standard tools used by the script: `chmod`, `cp`, `install`, `ln`, and `sed`.

Ubuntu GitHub Actions runners provide these tools. A Debian or Ubuntu workstation is the simplest local packaging environment.

The script cannot create the DEB package in ordinary Windows PowerShell. Use a current WSL distribution, a Linux virtual machine, a Linux container with the required SDK and tools, or the GitHub Actions Linux job.

## Build Linux packages

Run this command from the repository root on Linux:

```bash
bash ./scripts/package-linux.sh 1.0.0
```

The first positional argument is the package version. It defaults to `1.0.0` when omitted. For public releases, pass the release tag without its leading `v`.

The script performs these steps:

1. Deletes the previous `artifacts/publish/linux-x64` and `artifacts/deb-root` directories.
2. Publishes a Release, self-contained `linux-x64` application.
3. Marks the native application host as executable.
4. Archives the complete publish output as a portable TAR.GZ.
5. Creates a Debian filesystem tree under `artifacts/deb-root`.
6. Copies the application, launcher, desktop entry, icon, and generated package metadata into that tree.
7. Builds the DEB with root ownership recorded in the archive.
8. Generates SHA-256 checksum files beside both packages.

Expected outputs:

```text
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-linux-x64.tar.gz
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-linux-x64.tar.gz.sha256
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-linux-x64.deb
artifacts/packages/BethesdaVoiceLineCharacterCounter-<version>-linux-x64.deb.sha256
```

## Portable TAR.GZ

The portable archive contains the contents of the self-contained publish directory at its root. Users extract it and run:

```bash
./BethesdaVoiceLineCharacterCounter
```

The archive must preserve the executable permission set by `chmod +x`. Always create release archives on Linux and verify the permission after extraction:

```bash
mkdir /tmp/bvlcc-portable-test
tar -xzf artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.tar.gz \
  -C /tmp/bvlcc-portable-test
test -x /tmp/bvlcc-portable-test/BethesdaVoiceLineCharacterCounter
```

Do not package only the native host or application DLL. A self-contained Avalonia deployment also requires its .NET runtime, managed assemblies, native libraries, and resources.

## Debian filesystem layout

The generated package installs these primary paths:

```text
/usr/lib/bethesda-voice-line-character-counter/
    BethesdaVoiceLineCharacterCounter
    application assemblies, runtime files, native libraries, and resources
/usr/bin/bethesda-voice-line-character-counter
/usr/share/applications/bethesda-voice-line-character-counter.desktop
/usr/share/icons/hicolor/scalable/apps/bethesda-voice-line-character-counter.svg
```

`/usr/bin/bethesda-voice-line-character-counter` is a symbolic link to the application host under `/usr/lib`. This gives users a conventional lowercase command while preserving the established executable name.

The DEB staging tree is temporary build output under `artifacts/deb-root`. Never hand-edit it because the packaging script deletes and recreates it.

## Values that must stay aligned

Several names appear in multiple files. Update all related values together.

| Concept | Current value | Locations |
| --- | --- | --- |
| Application host | `BethesdaVoiceLineCharacterCounter` | `application_name`, TAR instructions, `/usr/lib` symlink target |
| Debian package | `bethesda-voice-line-character-counter` | `control`, launcher name, desktop filename |
| Install directory | `/usr/lib/bethesda-voice-line-character-counter` | `install_directory`, launcher target |
| Launcher command | `bethesda-voice-line-character-counter` | `/usr/bin` symlink, desktop `Exec` |
| Desktop icon name | `bethesda-voice-line-character-counter` | installed SVG filename, desktop `Icon` |

If these values drift, the package may build successfully but fail to launch from the terminal or desktop menu.

## Debian control metadata

`packaging/linux/control` is a template. The packaging script replaces `@VERSION@` with the requested build version and writes the result to `artifacts/deb-root/DEBIAN/control`.

Important fields:

- `Package` is the stable Debian package identity used for installation, upgrades, and removal.
- `Version` must follow Debian version syntax and must increase for upgrades.
- `Architecture` is `amd64`, matching the `linux-x64` runtime identifier.
- `Depends` lists native operating-system libraries, not the .NET runtime.
- `Description` uses Debian's multiline format: continuation lines begin with one space.

Do not rename `Package` for an ordinary release. A different package name is treated as a different installed product and does not automatically upgrade or replace the old package.

Inspect generated metadata without installing the package:

```bash
dpkg-deb --info artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.deb
dpkg-deb --contents artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.deb
```

When available, run `lintian` for additional Debian policy checks:

```bash
lintian artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.deb
```

## Native dependencies

The application is self-contained, so users do not need a separately installed .NET runtime. Avalonia still depends on native Linux desktop libraries. The current DEB declares:

```text
libx11-6, libice6, libsm6, libfontconfig1
```

When adding or removing native dependencies:

1. Verify the requirement on every supported distribution.
2. Update `Depends` in `packaging/linux/control`.
3. Update the Linux installation section in `README.md`.
4. Test installation on a clean Debian or Ubuntu system.
5. Confirm the portable package documentation names any libraries users must install manually.

Library package names vary across distribution families. The DEB metadata applies only to Debian-based systems; TAR.GZ users on other distributions must install equivalent native libraries through their distribution.

## Desktop entry and icon

The freedesktop desktop entry controls how the application appears in graphical menus:

- `Name` is the visible application name.
- `Comment` is the short menu description.
- `Exec` must match the launcher installed under `/usr/bin`.
- `Icon` must match the installed icon filename without its extension.
- `Terminal=false` prevents a terminal window from opening with the graphical application.
- `Categories=Utility;` places the application in a standard menu category.

Validate changes with `desktop-file-validate` when the tool is installed:

```bash
desktop-file-validate packaging/linux/bethesda-voice-line-character-counter.desktop
```

After installing a test package, confirm the application appears in the desktop menu and launches with the expected icon. Desktop environments may cache entries and icons, so log out and back in or refresh the relevant cache when testing updates.

## Permissions and ownership

The script uses `dpkg-deb --root-owner-group` so package contents are recorded as owned by root even when the package is built by an ordinary CI user. The application host is explicitly marked executable before either package is created.

Verify these properties with:

```bash
dpkg-deb --contents artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.deb
```

The application host and `/usr/bin` launcher must be executable. Desktop entries and icons should normally be mode `0644`.

## Install, upgrade, and remove tests

Install through APT so declared dependencies are resolved:

```bash
sudo apt install ./artifacts/packages/BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.deb
```

Verify the package and launcher:

```bash
dpkg-query -W bethesda-voice-line-character-counter
command -v bethesda-voice-line-character-counter
bethesda-voice-line-character-counter
```

Test an upgrade by installing a package with a higher version over the existing version. Confirm the package manager reports an upgrade, the application launches, and no obsolete files remain.

Remove the package and inspect the installed paths:

```bash
sudo apt remove bethesda-voice-line-character-counter
test ! -e /usr/bin/bethesda-voice-line-character-counter
test ! -e /usr/lib/bethesda-voice-line-character-counter
```

Perform final release checks on a clean Debian or Ubuntu virtual machine. Also extract and run the portable archive on at least one non-Debian distribution supported by the release.

## Checksums

The packaging script runs `sha256sum` from `artifacts/packages`, which keeps checksum entries portable by recording only the artifact filename. Verify them from that directory:

```bash
cd artifacts/packages
sha256sum --check BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.tar.gz.sha256
sha256sum --check BethesdaVoiceLineCharacterCounter-1.0.0-linux-x64.deb.sha256
```

Regenerate checksums whenever an artifact is rebuilt. Never publish an artifact with a checksum from an earlier build, even when the version did not change.

## Continuous integration

The Linux job in `.github/workflows/build-release.yml` runs on `ubuntu-latest`. It restores and tests the `.slnx` solution before invoking the packaging script.

For branch and pull-request builds, CI uses a version containing the workflow run number. For version tags, it removes the leading `v` and passes the remaining value to the script. The workflow uploads files matching `artifacts/packages/*-linux-x64.*`; tagged workflows later attach them to the GitHub release.

When changing artifact names, update the packaging script, README examples, workflow upload pattern, release expectations, and this guide together.

## Common maintenance changes

### Change the release version

Pass the version as the first script argument. Do not hard-code each release version in `control` or the script default.

### Rename the application executable

Update the project output name first. Then update `application_name`, the `/usr/bin` symlink target, portable launch documentation, and any CI checks that reference the host.

### Change the Debian package name

Update `Package` in `control`, the launcher and desktop filenames if appropriate, README install/remove commands, and CI conventions. Decide whether the new package must declare `Replaces`, `Breaks`, or `Conflicts` against the old package before publishing it.

### Change the install directory

Update both `install_directory` and the absolute `/usr/bin` symlink target. Rebuild and inspect the DEB contents before installation.

### Add ARM64 packages

Create a separate `linux-arm64` publish profile and use Debian architecture `arm64`. Produce distinct artifact names and CI jobs. Do not mix x64 and ARM64 files in one package.

### Add RPM or other formats

Treat additional package formats as separate outputs derived from the same clean publish directory. Keep package-manager metadata, dependencies, paths, and validation specific to each distribution family.

## Troubleshooting

### `dpkg-deb` is not found

Install the distribution's `dpkg` tooling or run the package job on Debian, Ubuntu, or the configured GitHub Actions runner.

### The application is not executable after extracting TAR.GZ

Confirm the script reached its `chmod +x` step and that the archive was created by Linux `tar`. Rebuild on Linux rather than recreating the archive with a Windows ZIP or TAR tool.

### The terminal command is missing or broken

Inspect `/usr/bin/bethesda-voice-line-character-counter` and confirm its target exactly matches the executable under `/usr/lib/bethesda-voice-line-character-counter`.

### The desktop menu item does not launch

Run the `/usr/bin` launcher directly, validate the desktop file, and confirm `Exec` matches the launcher name. If direct launch works, refresh the desktop environment's application cache.

### The desktop icon is missing

Confirm the SVG is installed under the hicolor scalable application icon directory and that the desktop entry's `Icon` value matches the filename without `.svg`.

### Package installation reports missing dependencies

Use `apt install ./package.deb` rather than `dpkg -i` for normal testing so APT can resolve dependencies. If a declared package does not exist on the target distribution version, determine the correct package name for the supported baseline.

### The package contains stale files

The script recreates the publish and DEB staging directories. Stale files usually indicate that packaging was interrupted, a package was assembled manually, or a different output directory was inspected. Remove `artifacts/publish/linux-x64` and `artifacts/deb-root`, then rerun the script.

### A new version does not upgrade the installed package

Confirm the `Package` field is unchanged and compare versions with:

```bash
dpkg --compare-versions NEW_VERSION gt OLD_VERSION && echo upgrade
```

Debian version ordering is not identical to semantic-version string ordering, especially for prerelease versions. Check ordering before publishing prerelease packages.