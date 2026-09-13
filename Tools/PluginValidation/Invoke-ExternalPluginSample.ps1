#requires -Version 7.2
<#
.SYNOPSIS
Runs one explicit external plugin-sample test in a bounded child process.
.DESCRIPTION
Uses the existing Release unit-test executable and a process-local manifest.
Retains stdout, stderr, TRX, and resource evidence in a fresh directory beneath
the repository's .work directory. A stopped process is an incomplete validation.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $ManifestPath,

    [Parameter(Mandatory)]
    [ValidatePattern('^FullyQualifiedName~CreationsForge\.UnitTests\.Engine\.ExternalSamples\.[A-Za-z0-9_.]+$')]
    [string] $TestFilter,

    [Parameter(Mandatory)]
    [string] $ResultsDirectory,

    [ValidateRange(1, 43200)]
    [int] $MaximumDurationSeconds = 1200,

    [ValidateRange(256, 131072)]
    [int] $MaximumPrivateMemoryMiB = 24576,

    [ValidateRange(1, 1024)]
    [int] $MaximumCapturedOutputMiB = 16
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$workRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot '.work'))
$pathComparison = if ($IsWindows) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
$resultsPath = [IO.Path]::GetFullPath($ResultsDirectory, $repositoryRoot)
if (-not $resultsPath.StartsWith($workRoot + [IO.Path]::DirectorySeparatorChar, $pathComparison)) {
    throw 'ResultsDirectory must be a fresh directory beneath this repository''s .work directory.'
}
if (Test-Path -LiteralPath $resultsPath) {
    throw 'ResultsDirectory already exists. Use a fresh run directory to preserve prior evidence.'
}

# Reject existing directory aliases before creating any task evidence.
$ancestorPath = [IO.Path]::GetDirectoryName($resultsPath)
while ($ancestorPath -and -not $ancestorPath.Equals($repositoryRoot, $pathComparison)) {
    if (Test-Path -LiteralPath $ancestorPath) {
        $ancestor = Get-Item -LiteralPath $ancestorPath -Force
        if (-not $ancestor.PSIsContainer -or ($ancestor.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
            throw 'An existing results-directory ancestor is a file or reparse point.'
        }
    }
    $ancestorPath = [IO.Path]::GetDirectoryName($ancestorPath)
}
if (-not $ancestorPath) {
    throw 'The results directory does not resolve beneath the repository.'
}

$manifestFullPath = (Resolve-Path -LiteralPath $ManifestPath).ProviderPath
if (-not (Test-Path -LiteralPath $manifestFullPath -PathType Leaf)) {
    throw 'ManifestPath must identify an existing file.'
}
$testAssembly = Join-Path $repositoryRoot 'CreationsForge.UnitTests/bin/Release/net10.0/CreationsForge.UnitTests.dll'
if (-not (Test-Path -LiteralPath $testAssembly -PathType Leaf)) {
    throw 'The Release unit-test executable is missing. Build the unit-test project first.'
}
$dotnetPath = (Get-Command dotnet -CommandType Application | Select-Object -First 1).Source
[IO.Directory]::CreateDirectory($resultsPath) | Out-Null
$stdoutPath = Join-Path $resultsPath 'stdout.txt'
$stderrPath = Join-Path $resultsPath 'stderr.txt'
$trxPath = Join-Path $resultsPath 'results.trx'
$summaryPath = Join-Path $resultsPath 'process-result.json'
$startInfo = [Diagnostics.ProcessStartInfo]::new()
$startInfo.FileName = $dotnetPath
$startInfo.WorkingDirectory = $repositoryRoot
$startInfo.UseShellExecute = $false
$startInfo.CreateNoWindow = $true
$startInfo.RedirectStandardOutput = $true
$startInfo.RedirectStandardError = $true
foreach ($argument in @($testAssembly, '-noColor', '-filterVSTest', $TestFilter, '-result-trx', $trxPath)) {
    $startInfo.ArgumentList.Add($argument)
}
$startInfo.Environment['CREATIONSFORGE_EXTERNAL_SAMPLE_MANIFEST'] = $manifestFullPath
$child = [Diagnostics.Process]::new()
$child.StartInfo = $startInfo
$stdoutFile = $null
$stderrFile = $null
$stdoutCopy = $null
$stderrCopy = $null
$captureCompletion = $null
$captureCancellation = [Threading.CancellationTokenSource]::new()
$started = $false
$elapsed = [Diagnostics.Stopwatch]::new()
$startedUtc = [DateTime]::UtcNow
$peakPrivateBytes = 0L
$peakWorkingSetBytes = 0L
$memorySampleCount = 0
$stopReason = $null
$failureMessage = $null
$childExitCode = $null
$counters = $null
$verifiedTestSuccess = $false
try {
    $stdoutFile = [IO.File]::Open($stdoutPath, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::Read)
    $stderrFile = [IO.File]::Open($stderrPath, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::Read)
    $started = $child.Start()
    if (-not $started) { throw 'The external test process did not start.' }
    $elapsed.Start()
    $stdoutCopy = $child.StandardOutput.BaseStream.CopyToAsync($stdoutFile, $captureCancellation.Token)
    $stderrCopy = $child.StandardError.BaseStream.CopyToAsync($stderrFile, $captureCancellation.Token)
    $captureCompletion = [Threading.Tasks.Task]::WhenAll([Threading.Tasks.Task[]] @($stdoutCopy, $stderrCopy))
    Write-Output "Started external plugin-sample validation (PID $($child.Id))."
    while (-not $child.WaitForExit(500)) {
        $child.Refresh()
        if (-not $child.HasExited) {
            $peakPrivateBytes = [Math]::Max($peakPrivateBytes, $child.PrivateMemorySize64)
            $peakWorkingSetBytes = [Math]::Max($peakWorkingSetBytes, $child.WorkingSet64)
            $memorySampleCount++
        }
        if ($peakPrivateBytes -gt ($MaximumPrivateMemoryMiB * 1MB)) {
            $stopReason = 'PrivateMemoryLimitExceeded'
        } elseif ($elapsed.Elapsed.TotalSeconds -gt $MaximumDurationSeconds) {
            $stopReason = 'WallClockLimitExceeded'
        } elseif (($stdoutFile.Length + $stderrFile.Length) -gt ($MaximumCapturedOutputMiB * 1MB)) {
            $stopReason = 'CapturedOutputLimitExceeded'
        }
        if ($stopReason) {
            if (-not $child.HasExited) { $child.Kill($true) }
            break
        }
    }
    if (-not $child.WaitForExit(10000)) { throw 'The stopped test process did not exit within ten seconds.' }
    $childExitCode = $child.ExitCode
    # Descendants may retain inherited pipes after the tracked process exits.
    # Continue enforcing output/time limits, with at most ten extra seconds to drain.
    $drainStartedSeconds = $elapsed.Elapsed.TotalSeconds
    while (-not $stopReason -and -not $captureCompletion.IsCompleted) {
        if ($elapsed.Elapsed.TotalSeconds -gt $MaximumDurationSeconds) {
            $stopReason = 'WallClockLimitExceeded'
        } elseif (($stdoutFile.Length + $stderrFile.Length) -gt ($MaximumCapturedOutputMiB * 1MB)) {
            $stopReason = 'CapturedOutputLimitExceeded'
        } elseif (($elapsed.Elapsed.TotalSeconds - $drainStartedSeconds) -ge 10) {
            $stopReason = 'CapturedOutputDrainTimeout'
        }
        if (-not $stopReason) { Start-Sleep -Milliseconds 100 }
    }
    if (-not $captureCompletion.IsCompleted) {
        throw 'Captured output did not finish within the bounded drain; validation is incomplete.'
    }
    $null = $captureCompletion.GetAwaiter().GetResult()
    $stdoutFile.Flush()
    $stderrFile.Flush()
    if (-not $stopReason -and ($stdoutFile.Length + $stderrFile.Length) -gt ($MaximumCapturedOutputMiB * 1MB)) {
        $stopReason = 'CapturedOutputLimitExceeded'
    }
    if (-not $stopReason -and $childExitCode -eq 0) {
        if (-not (Test-Path -LiteralPath $trxPath -PathType Leaf)) {
            throw 'The test process exited successfully without writing its expected TRX evidence.'
        }
        [xml] $trx = Get-Content -LiteralPath $trxPath -Raw
        $trxCounters = $trx.SelectSingleNode("//*[local-name()='ResultSummary']/*[local-name()='Counters']")
        if ($null -eq $trxCounters) { throw 'The TRX has no result counters.' }
        $counters = [ordered]@{}
        foreach ($attribute in $trxCounters.Attributes) { $counters[$attribute.Name] = [long] $attribute.Value }
        $executed = [long] $trxCounters.GetAttribute('executed')
        $total = [long] $trxCounters.GetAttribute('total')
        $passed = [long] $trxCounters.GetAttribute('passed')
        $failed = [long] $trxCounters.GetAttribute('failed')
        if ($executed -le 0 -or $passed -le 0 -or $passed -ne $total -or $failed -ne 0) {
            throw 'Every selected external test must execute and pass. Empty or partly skipped runs are not acceptance.'
        }
        $verifiedTestSuccess = $true
    }
} catch {
    $failureMessage = $_.Exception.Message
} finally {
    if ($started -and -not $child.HasExited) {
        $child.Kill($true)
        $null = $child.WaitForExit(10000)
    }
    $captureCancellation.Cancel()
    if ($started) {
        $child.StandardOutput.Dispose()
        $child.StandardError.Dispose()
    }
    if ($null -ne $captureCompletion) {
        try {
            if (-not $captureCompletion.Wait(2000) -and -not $failureMessage) {
                $failureMessage = 'Output capture did not complete within the bounded cleanup interval.'
            }
        } catch {
            if (-not $failureMessage) { $failureMessage = $_.Exception.Message }
        }
    }
    if ($null -ne $stdoutFile) { $stdoutFile.Dispose() }
    if ($null -ne $stderrFile) { $stderrFile.Dispose() }
    $elapsed.Stop()
    $child.Dispose()
    $captureCancellation.Dispose()
}

$summary = [ordered]@{
    startedUtc = $startedUtc.ToString('O')
    testFilter = $TestFilter
    elapsedSeconds = [Math]::Round($elapsed.Elapsed.TotalSeconds, 3)
    childExitCode = $childExitCode
    peakObservedPrivateBytes = if ($memorySampleCount -gt 0) { $peakPrivateBytes } else { $null }
    peakObservedWorkingSetBytes = if ($memorySampleCount -gt 0) { $peakWorkingSetBytes } else { $null }
    memorySampleCount = $memorySampleCount
    maximumPrivateMemoryMiB = $MaximumPrivateMemoryMiB
    maximumDurationSeconds = $MaximumDurationSeconds
    maximumCapturedOutputMiB = $MaximumCapturedOutputMiB
    stopReason = $stopReason
    failureMessage = $failureMessage
    testCounters = $counters
    validationSucceeded = ($verifiedTestSuccess -and -not $failureMessage)
    resourceObservation = 'Private and working-set memory were sampled every 500 ms; these are observed peaks, not allocation guarantees.'
}
$summary | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $summaryPath -Encoding utf8NoBOM
$summary | ConvertTo-Json -Depth 6 | Write-Output
if (-not $summary.validationSucceeded) { exit 1 }
exit 0
