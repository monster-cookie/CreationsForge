[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputDirectory = (Join-Path ([Environment]::GetFolderPath("UserProfile")) "Downloads"),
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

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

if ($Version -notmatch "^\d+\.\d+\.\d+$") {
    throw "Release version '$Version' must use the major.minor.patch format."
}

if ($RuntimeIdentifier -ne "win-x64") {
    throw "The Inno Setup installer is supported only for the win-x64 runtime identifier."
}

$outputDirectoryPath = [System.IO.Path]::GetFullPath($OutputDirectory)
$archivePath = Join-Path $outputDirectoryPath "SFRecordCompareEngine-$RuntimeIdentifier-$Version.zip"
$installerPath = Join-Path $outputDirectoryPath "SFRecordCompareEngine-Setup-$Version.exe"

if (-not $compilerPath) {
    throw "Inno Setup Compiler was not found. Install it with: winget install --id JRSoftware.InnoSetup --exact"
}

if (-not (Test-Path -LiteralPath $archivePath)) {
    throw "Application package was not found at '$archivePath'. Run Package-Application.ps1 first."
}

try {
    New-Item -ItemType Directory -Path $temporaryDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $outputDirectoryPath -Force | Out-Null
    Expand-Archive -LiteralPath $archivePath -DestinationPath $temporaryDirectory

    if (Test-Path -LiteralPath $installerPath) {
        Remove-Item -LiteralPath $installerPath -Force
    }

    & $compilerPath "/DSourceDirectory=$temporaryDirectory" "/DOutputDirectory=$outputDirectoryPath" "/DApplicationVersion=$Version" $installerDefinitionPath
    if ($LASTEXITCODE -ne 0) {
        throw "Inno Setup Compiler failed with exit code $LASTEXITCODE."
    }

    if (-not (Test-Path -LiteralPath $installerPath)) {
        throw "Inno Setup Compiler completed without creating the expected installer at '$installerPath'."
    }

    Write-Host "Created application installer: $installerPath"
}
finally {
    if (Test-Path -LiteralPath $temporaryDirectory) {
        Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force
    }
}
