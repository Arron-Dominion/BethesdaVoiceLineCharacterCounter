param(
    [Parameter(Mandatory = $false)]
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot "BethesdaVoiceLineCharacterCounter\BethesdaVoiceLineCharacterCounter.csproj"
$publishDirectory = Join-Path $repositoryRoot "artifacts\publish\win-x64"
$packageDirectory = Join-Path $repositoryRoot "artifacts\packages"
$archiveName = "BethesdaVoiceLineCharacterCounter-$Version-win-x64-portable.zip"
$archivePath = Join-Path $packageDirectory $archiveName

Remove-Item $publishDirectory -Recurse -Force -ErrorAction SilentlyContinue
New-Item $publishDirectory -ItemType Directory -Force | Out-Null
New-Item $packageDirectory -ItemType Directory -Force | Out-Null

dotnet publish $projectPath `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishProfile=win-x64 `
    -p:Version=$Version `
    -o $publishDirectory

if ($LASTEXITCODE -ne 0) {
    throw "Windows publish failed."
}

Remove-Item $archivePath -Force -ErrorAction SilentlyContinue
Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $archivePath

$innoCompilerPath = Get-Command ISCC.exe -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty Source -First 1

if ($null -eq $innoCompilerPath) {
    $innoCompilerPath = @(
        "$env:ProgramFiles\Inno Setup 7\ISCC.exe"
        "${env:ProgramFiles(x86)}\Inno Setup 7\ISCC.exe"
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
    ) | Where-Object { Test-Path $_ } | Select-Object -First 1
}

if ($null -ne $innoCompilerPath) {
    $installerScript = Join-Path $repositoryRoot "packaging\windows\setup.iss"
    & $innoCompilerPath `
        "/DMyAppVersion=$Version" `
        "/DPublishDir=$publishDirectory" `
        "/DOutputDir=$packageDirectory" `
        $installerScript

    if ($LASTEXITCODE -ne 0) {
        throw "Inno Setup compilation failed."
    }
}
else {
    Write-Warning "ISCC.exe was not found; the portable ZIP was created without an installer."
}

Get-ChildItem $packageDirectory -File |
    Where-Object { $_.Name -like "BethesdaVoiceLineCharacterCounter-$Version-win-x64-*" -and $_.Extension -ne ".sha256" } |
    ForEach-Object {
        $hash = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $($_.Name)" | Set-Content "$($_.FullName).sha256" -Encoding ascii
    }