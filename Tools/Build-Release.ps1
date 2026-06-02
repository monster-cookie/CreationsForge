[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$OutputDirectory = (Join-Path ([Environment]::GetFolderPath("UserProfile")) "Downloads"),
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

if ($Version -notmatch "^\d+\.\d+\.\d+$") {
    throw "Release version '$Version' must use the major.minor.patch format."
}

& (Join-Path $PSScriptRoot "Package-Application.ps1") -Configuration $Configuration -RuntimeIdentifier "win-x64" -OutputDirectory $OutputDirectory -Version $Version
& (Join-Path $PSScriptRoot "Build-Installer.ps1") -Configuration $Configuration -RuntimeIdentifier "win-x64" -OutputDirectory $OutputDirectory -Version $Version
& (Join-Path $PSScriptRoot "Package-Application.ps1") -Configuration $Configuration -RuntimeIdentifier "linux-x64" -OutputDirectory $OutputDirectory -Version $Version

Write-Host "Created release version: $Version"
