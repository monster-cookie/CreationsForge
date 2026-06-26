$ErrorActionPreference = "Stop"

$crashDumpPath = "C:\ProgramData\CreationsForge\CrashDumps"

if (-not (Test-Path $crashDumpPath)) {
    New-Item -ItemType Directory -Path $crashDumpPath -Force | Out-Null
}

# Purge old dumps before starting a new run.
Get-ChildItem $crashDumpPath -File -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue

$env:DOTNET_DbgEnableMiniDump = "1"
$env:DOTNET_DbgMiniDumpType = "4" # Full dump use 2 if file space is an issue
$env:DOTNET_DbgMiniDumpName = "$crashDumpPath\%e-%p-%t.dmp"
$env:DOTNET_CreateDumpDiagnostics = "1"
$env:DOTNET_CreateDumpLogToFile = "$crashDumpPath\createdump.log"

dotnet build
.\CreationsForge.Console\bin\Debug\net10.0\CreationsForge.Console.exe --reset-all
$exitCode = $LASTEXITCODE

# Only show dump info if the console app crashed or otherwise failed.
if ($exitCode -ne 0) {
    Get-ChildItem C:\ProgramData\CreationsForge\CrashDumps |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 10 FullName, LastWriteTime, Length
}

exit $exitCode