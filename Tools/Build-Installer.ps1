[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputDirectory = (Join-Path ([Environment]::GetFolderPath("UserProfile")) "Downloads"),
    [string]$Version
)

$ErrorActionPreference = "Stop"

$versionPath = Join-Path $PSScriptRoot "Release-Version.txt"
$installerDefinitionPath = Join-Path $PSScriptRoot "SFRecordCompareEngine.iss"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "SFRecordCompareEngine-Installer-$([Guid]::NewGuid())"
$compilerCommand = Get-Command "ISCC.exe" -ErrorAction SilentlyContinue
$compilerPaths = @(
    $(if ($compilerCommand) { $compilerCommand.Source }),
    (Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"),
    (Join-Path $env:ProgramFiles "Inno Setup 6\ISCC.exe"),
    (Join-Path $env:LOCALAPPDATA "Programs\Inno Setup 6\ISCC.exe")
)
$compilerPath = $compilerPaths | Where-Object { $_ -and (Test-Path -LiteralPath $_) } | Select-Object -First 1

if (-not $Version) {
    $Version = (Get-Content -LiteralPath $versionPath -Raw).Trim()
}

if ($Version -notmatch "^\d+\.\d+\.\d+$") {
    throw "Release version '$Version' must use the major.minor.patch format."
}

$archivePath = Join-Path $OutputDirectory "SFRecordCompareEngine-$RuntimeIdentifier-$Version.zip"
$installerPath = Join-Path $OutputDirectory "SFRecordCompareEngine-Setup-$Version.exe"

if (-not $compilerPath) {
    throw "Inno Setup Compiler was not found. Install it with: winget install --id JRSoftware.InnoSetup --exact"
}

if (-not (Test-Path -LiteralPath $archivePath)) {
    throw "Application package was not found at '$archivePath'. Run Package-Application.ps1 first."
}

try {
    New-Item -ItemType Directory -Path $temporaryDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
    Expand-Archive -LiteralPath $archivePath -DestinationPath $temporaryDirectory

    if (Test-Path -LiteralPath $installerPath) {
        Remove-Item -LiteralPath $installerPath -Force
    }

    & $compilerPath "/DSourceDirectory=$temporaryDirectory" "/DOutputDirectory=$OutputDirectory" "/DApplicationVersion=$Version" $installerDefinitionPath
    if ($LASTEXITCODE -ne 0) {
        throw "Inno Setup Compiler failed with exit code $LASTEXITCODE."
    }

    Write-Host "Created application installer: $installerPath"
}
finally {
    if (Test-Path -LiteralPath $temporaryDirectory) {
        Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force
    }
}
