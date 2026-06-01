[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$OutputDirectory = (Join-Path ([Environment]::GetFolderPath("UserProfile")) "Downloads")
)

$ErrorActionPreference = "Stop"

$versionPath = Join-Path $PSScriptRoot "Release-Version.txt"
$currentVersion = (Get-Content -LiteralPath $versionPath -Raw).Trim()
$versionMatch = [regex]::Match($currentVersion, "^(?<Major>\d+)\.(?<Minor>\d+)\.(?<Patch>\d+)$")

if (-not $versionMatch.Success) {
    throw "Release version '$currentVersion' must use the major.minor.patch format."
}

$version = "{0}.{1}.{2}" -f $versionMatch.Groups["Major"].Value, $versionMatch.Groups["Minor"].Value, ([int]$versionMatch.Groups["Patch"].Value + 1)
Set-Content -LiteralPath $versionPath -Value $version

& (Join-Path $PSScriptRoot "Package-Application.ps1") -Configuration $Configuration -RuntimeIdentifier "win-x64" -OutputDirectory $OutputDirectory -Version $version
& (Join-Path $PSScriptRoot "Build-Installer.ps1") -Configuration $Configuration -RuntimeIdentifier "win-x64" -OutputDirectory $OutputDirectory -Version $version
& (Join-Path $PSScriptRoot "Package-Application.ps1") -Configuration $Configuration -RuntimeIdentifier "linux-x64" -OutputDirectory $OutputDirectory -Version $version

Write-Host "Created release version: $version"
