# Bethesda Voice Line Character Counter

A cross-platform Avalonia desktop utility that counts characters in a voice line and calculates the dialogue sections required by Fallout 4, Skyrim, Skyrim Special Edition, or Starfield.

## Supported systems

- Windows 10 or 11, x64
- Desktop Linux, x64

Release packages are self-contained and do not require a separately installed .NET runtime.

## Install on Windows

Choose one of the release artifacts:

- `BethesdaVoiceLineCharacterCounter-<version>-win-x64-setup.exe` requests administrator access and installs the application under `C:\Program Files\Bethesda Voice Line Character Counter` by default, with a Start menu shortcut.
- `BethesdaVoiceLineCharacterCounter-<version>-win-x64-portable.zip` can be extracted and run without installation.

Run `BethesdaVoiceLineCharacterCounter.exe` from the installed or extracted directory.

## Install on Linux

The DEB package installs the application under `/usr/lib/bethesda-voice-line-character-counter` and adds `bethesda-voice-line-character-counter` to `/usr/bin`.

```bash
sudo apt install ./BethesdaVoiceLineCharacterCounter-<version>-linux-x64.deb
```

For portable use, extract the TAR.GZ and run the application host:

```bash
tar -xzf BethesdaVoiceLineCharacterCounter-<version>-linux-x64.tar.gz
./BethesdaVoiceLineCharacterCounter
```

Linux packages require X11, ICE, SM, and Fontconfig libraries. On Debian or Ubuntu they can be installed with:

```bash
sudo apt install libx11-6 libice6 libsm6 libfontconfig1
```

## Develop

The repository uses the .NET SDK selected by `global.json`. MAUI workloads are not required.

```powershell
dotnet restore .\BethesdaVoiceLineCharacterCounter.slnx
dotnet test .\BethesdaVoiceLineCharacterCounter.slnx -c Debug --no-restore
dotnet run --project .\BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj
```

## Publish

Create self-contained folder deployments with the checked-in profiles:

```powershell
dotnet publish .\BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj -c Release -r win-x64 --self-contained true -p:PublishProfile=win-x64 -o .\artifacts\publish\win-x64
dotnet publish .\BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj -c Release -r linux-x64 --self-contained true -p:PublishProfile=linux-x64 -o .\artifacts\publish\linux-x64
```

Create Windows release artifacts with PowerShell. Inno Setup 7 or 6 must be installed to build the installer; the script searches `PATH` and the standard installation directories. The portable ZIP is created without it.

```powershell
.\scripts\package-windows.ps1 -Version 1.0.0
```

See the [Windows installer maintenance guide](docs/tools/package/windows-installer-maintenance.md) before changing installer identity, privileges, paths, shortcuts, or signing behavior.

Create Linux TAR.GZ and DEB artifacts on a Debian-based build host:

```bash
bash ./scripts/package-linux.sh 1.0.0
```

See the [Linux packaging maintenance guide](docs/tools/package/linux-packaging-maintenance.md) before changing package identity, install paths, dependencies, desktop integration, architecture, or artifact names.

Both scripts write SHA-256 checksum files beside the release artifacts. Push a version tag matching `v*`, such as `v2.0.0`, to run the GitHub Actions release workflow and publish all generated packages. The workflow creates the GitHub Release when it does not exist; if the tag was created by publishing a release in the GitHub UI, the workflow uploads the generated packages to that existing release.
