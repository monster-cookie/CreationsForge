[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputDirectory = (Join-Path ([Environment]::GetFolderPath("UserProfile")) "Downloads"),
    [string]$Version
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot "SFRecordCompareEngine\SFRecordCompareEngine.csproj"
$versionPath = Join-Path $PSScriptRoot "Release-Version.txt"
$knownIssuesPath = Join-Path $repositoryRoot "Documentation\KNOWN-ISSUES.md"
$roadmapPath = Join-Path $repositoryRoot "Documentation\ROADMAP.md"
$changeLogPath = Join-Path $repositoryRoot "Documentation\CHANGE-LOG.md"
$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "SFRecordCompareEngine-$([Guid]::NewGuid())"
$publishDirectory = Join-Path $temporaryDirectory "publish"

if (-not $Version) {
    $Version = (Get-Content -LiteralPath $versionPath -Raw).Trim()
}

if ($Version -notmatch "^\d+\.\d+\.\d+$") {
    throw "Release version '$Version' must use the major.minor.patch format."
}

$archivePath = Join-Path $OutputDirectory "SFRecordCompareEngine-$RuntimeIdentifier-$Version.zip"

try {
    New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null

    & dotnet publish $projectPath --configuration $Configuration --runtime $RuntimeIdentifier --self-contained true --output $publishDirectory -p:Version=$Version -p:AssemblyVersion=$Version -p:FileVersion=$Version
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE."
    }

    Copy-Item -LiteralPath $knownIssuesPath -Destination $publishDirectory
    Copy-Item -LiteralPath $roadmapPath -Destination $publishDirectory
    Copy-Item -LiteralPath $changeLogPath -Destination $publishDirectory
    New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

    if (Test-Path -LiteralPath $archivePath) {
        Remove-Item -LiteralPath $archivePath -Force
    }

    Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $archivePath
    Write-Host "Created application package: $archivePath"
}
finally {
    if (Test-Path -LiteralPath $temporaryDirectory) {
        Remove-Item -LiteralPath $temporaryDirectory -Recurse -Force
    }
}
