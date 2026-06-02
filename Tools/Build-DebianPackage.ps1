[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$OutputDirectory = (Join-Path ([Environment]::GetFolderPath("UserProfile")) "Downloads"),
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path (Join-Path $repositoryRoot "SFRecordCompareEngine") "SFRecordCompareEngine.csproj"
$desktopEntryPath = Join-Path (Join-Path $PSScriptRoot "Linux") "com.sfrecordcompareengine.app.desktop"
$iconPath = Join-Path (Join-Path (Join-Path $repositoryRoot "SFRecordCompareEngine") "Resources\AppIcon") "appicon.svg"
$documentationDirectory = Join-Path $repositoryRoot "Documentation"
$knownIssuesPath = Join-Path $documentationDirectory "KNOWN-ISSUES.md"
$roadmapPath = Join-Path $documentationDirectory "ROADMAP.md"
$changeLogPath = Join-Path $documentationDirectory "CHANGE-LOG.md"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "SFRecordCompareEngine-Debian-$([Guid]::NewGuid())"
$publishDirectory = Join-Path $temporaryDirectory "publish"
$packageRoot = Join-Path $temporaryDirectory "package"
$applicationDirectory = Join-Path (Join-Path $packageRoot "opt") "sfrecordcompareengine"
$controlDirectory = Join-Path $packageRoot "DEBIAN"
$usrDirectory = Join-Path $packageRoot "usr"
$shareDirectory = Join-Path $usrDirectory "share"
$launcherDirectory = Join-Path $usrDirectory "bin"
$desktopEntryDirectory = Join-Path $shareDirectory "applications"
$iconDirectory = Join-Path (Join-Path (Join-Path (Join-Path $shareDirectory "icons") "hicolor") "scalable") "apps"

if ($Version -notmatch "^\d+\.\d+\.\d+$") {
    throw "Release version '$Version' must use the major.minor.patch format."
}

$packagePath = Join-Path $OutputDirectory "SFRecordCompareEngine_${Version}_amd64.deb"

try {
    New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $applicationDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $controlDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $launcherDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $desktopEntryDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $iconDirectory -Force | Out-Null
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

    & dotnet publish $projectPath --configuration $Configuration --runtime "linux-x64" --self-contained true --output $publishDirectory -p:Version=$Version -p:AssemblyVersion=$Version -p:FileVersion=$Version
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE."
    }

    Copy-Item -Path (Join-Path $publishDirectory "*") -Destination $applicationDirectory -Recurse
    Copy-Item -LiteralPath $knownIssuesPath -Destination $applicationDirectory
    Copy-Item -LiteralPath $roadmapPath -Destination $applicationDirectory
    Copy-Item -LiteralPath $changeLogPath -Destination $applicationDirectory
    Copy-Item -LiteralPath $desktopEntryPath -Destination $desktopEntryDirectory
    Copy-Item -LiteralPath $iconPath -Destination (Join-Path $iconDirectory "sfrecordcompareengine.svg")

    $launcherPath = Join-Path $launcherDirectory "sfrecordcompareengine"
    Set-Content -LiteralPath $launcherPath -Encoding utf8 -Value @'
#!/usr/bin/env bash
exec /opt/sfrecordcompareengine/SFRecordCompareEngine "$@"
'@

    Set-Content -LiteralPath (Join-Path $controlDirectory "control") -Encoding utf8 -Value @"
Package: sfrecordcompareengine
Version: $Version
Section: utils
Priority: optional
Architecture: amd64
Depends: libgl1, dbus, libfontconfig1, libxrandr2, libxi6
Maintainer: Venpi
Description: Starfield plugin inspection and comparison tool
 SFRecordCompareEngine imports and compares selected Starfield plugin records.
"@

    & chmod "+x" $launcherPath
    if ($LASTEXITCODE -ne 0) {
        throw "chmod failed for '$launcherPath' with exit code $LASTEXITCODE."
    }

    & chmod "+x" (Join-Path $applicationDirectory "SFRecordCompareEngine")
    if ($LASTEXITCODE -ne 0) {
        throw "chmod failed for the application executable with exit code $LASTEXITCODE."
    }

    if (Test-Path -LiteralPath $packagePath) {
        Remove-Item -LiteralPath $packagePath -Force
    }

    & dpkg-deb --build --root-owner-group $packageRoot $packagePath
    if ($LASTEXITCODE -ne 0) {
        throw "dpkg-deb failed with exit code $LASTEXITCODE."
    }

    Write-Host "Created Debian package: $packagePath"
}
finally {
    if (Test-Path -LiteralPath $temporaryDirectory) {
        Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force
    }
}
