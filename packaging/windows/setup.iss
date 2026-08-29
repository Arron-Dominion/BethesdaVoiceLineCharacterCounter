#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#ifndef PublishDir
  #define PublishDir "..\..\artifacts\publish\win-x64"
#endif
#ifndef OutputDir
  #define OutputDir "..\..\artifacts\packages"
#endif

#define MyAppName "Bethesda Voice Line Character Counter"
#define MyAppExeName "BethesdaVoiceLineCharacterCounter.exe"

[Setup]
AppId={{B901441A-3A27-4914-A405-87A94F533F6C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=Arron Dominion
DefaultDirName={autopf}\Bethesda Voice Line Character Counter
DefaultGroupName={#MyAppName}
OutputDir={#OutputDir}
OutputBaseFilename=BethesdaVoiceLineCharacterCounter-{#MyAppVersion}-win-x64-setup
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
WizardStyle=modern

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent