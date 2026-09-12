#requires -Version 7.2
<#
.SYNOPSIS
Runs one isolated Codex CLI process for native MCP acceptance evidence.
.DESCRIPTION
Starts the native Codex CLI with command-local configuration for only the
CreationsForge stdio MCP server. Retains JSONL events, stderr, the final agent
response, and bounded process evidence in a fresh directory beneath this
repository's .work directory. A successful process is not workflow acceptance;
the retained events and tool results still require independent inspection.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $CodexPath,

    [Parameter(Mandatory)]
    [string] $ConsoleAssemblyPath,

    [Parameter(Mandatory)]
    [string] $PromptPath,

    [Parameter(Mandatory)]
    [string] $TaskDirectory,

    [ValidateRange(1, 43200)]
    [int] $MaximumDurationSeconds = 1200,

    [ValidateRange(256, 131072)]
    [int] $MaximumPrivateMemoryMiB = 24576,

    [ValidateRange(1, 1024)]
    [int] $MaximumCapturedOutputMiB = 32,

    [switch] $ApproveDisposableSave
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-RegularFile {
    param(
        [Parameter(Mandatory)]
        [string] $Path,

        [Parameter(Mandatory)]
        [string] $Description
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "$Description must identify an existing file."
    }

    $item = Get-Item -LiteralPath $Path -Force
    if ($item.PSIsContainer -or ($item.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        throw "$Description must identify a regular, non-reparse-point file."
    }

    return [IO.Path]::GetFullPath($item.FullName)
}

function Assert-RegularDirectory {
    param(
        [Parameter(Mandatory)]
        [string] $Path,

        [Parameter(Mandatory)]
        [string] $Description
    )

    $item = Get-Item -LiteralPath $Path -Force
    if (-not $item.PSIsContainer -or ($item.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        throw "$Description must be a regular, non-reparse-point directory."
    }
}

function Assert-RegularDirectoryHierarchy {
    param(
        [Parameter(Mandatory)]
        [string] $Path,

        [Parameter(Mandatory)]
        [string] $BoundaryPath,

        [Parameter(Mandatory)]
        [StringComparison] $Comparison
    )

    $currentPath = $Path
    while ($currentPath) {
        Assert-RegularDirectory -Path $currentPath -Description 'A task-directory hierarchy entry'
        if ($currentPath.Equals($BoundaryPath, $Comparison)) {
            return
        }

        $currentPath = [IO.Path]::GetDirectoryName($currentPath)
    }

    throw 'The task-directory hierarchy does not terminate at the repository .work directory.'
}

function ConvertTo-TomlBasicString {
    param(
        [Parameter(Mandatory)]
        [string] $Value
    )

    $builder = [Text.StringBuilder]::new()
    :characterLoop foreach ($character in $Value.ToCharArray()) {
        switch ([int] $character) {
            8 { $null = $builder.Append('\b'); continue characterLoop }
            9 { $null = $builder.Append('\t'); continue characterLoop }
            10 { $null = $builder.Append('\n'); continue characterLoop }
            12 { $null = $builder.Append('\f'); continue characterLoop }
            13 { $null = $builder.Append('\r'); continue characterLoop }
            34 { $null = $builder.Append('\"'); continue characterLoop }
            92 { $null = $builder.Append('\\'); continue characterLoop }
        }

        if ([int] $character -lt 32) {
            throw 'A command path contains a control character that cannot be represented safely in TOML.'
        }

        $null = $builder.Append($character)
    }

    return '"' + $builder.ToString() + '"'
}

function Write-NewUtf8File {
    param(
        [Parameter(Mandatory)]
        [string] $Path,

        [Parameter(Mandatory)]
        [string] $Content
    )

    $encoding = [Text.UTF8Encoding]::new($false)
    $bytes = $encoding.GetBytes($Content)
    $stream = [IO.File]::Open($Path, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::Read)
    try {
        $stream.Write($bytes, 0, $bytes.Length)
        $stream.Flush($true)
    } finally {
        $stream.Dispose()
    }
}

function Get-EvidenceMetadata {
    param(
        [Parameter(Mandatory)]
        [string] $Path,

        [Parameter(Mandatory)]
        [string] $Description
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return $null
    }

    $item = Get-Item -LiteralPath $Path -Force
    if ($item.PSIsContainer -or ($item.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
        throw "$Description is not a regular evidence file."
    }

    return [ordered]@{
        path = $item.FullName
        lengthBytes = $item.Length
        sha256 = (Get-FileHash -LiteralPath $item.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    }
}

function Get-CapturedOutputLength {
    param(
        [Parameter(Mandatory)]
        [IO.FileStream] $EventsFile,

        [Parameter(Mandatory)]
        [IO.FileStream] $StderrFile,

        [Parameter(Mandatory)]
        [string] $FinalResponsePath
    )

    $length = $EventsFile.Length + $StderrFile.Length
    if (Test-Path -LiteralPath $FinalResponsePath -PathType Leaf) {
        $finalResponse = Get-Item -LiteralPath $FinalResponsePath -Force
        if ($finalResponse.PSIsContainer -or ($finalResponse.Attributes -band [IO.FileAttributes]::ReparsePoint)) {
            throw 'The final response path became a non-regular file while the Codex process was running.'
        }

        $length += $finalResponse.Length
    }

    return $length
}

$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$workRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot '.work'))
$pathComparison = if ($IsWindows) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
$taskPath = [IO.Path]::GetFullPath($TaskDirectory, $repositoryRoot)
if (-not $taskPath.StartsWith($workRoot + [IO.Path]::DirectorySeparatorChar, $pathComparison)) {
    throw 'TaskDirectory must be a fresh directory strictly beneath this repository''s .work directory.'
}
if (Test-Path -LiteralPath $taskPath) {
    throw 'TaskDirectory already exists. Use a fresh directory to preserve prior acceptance evidence.'
}

# Reject aliases in every existing ancestor between the requested task and .work.
$ancestorPath = [IO.Path]::GetDirectoryName($taskPath)
$foundWorkRoot = $false
while ($ancestorPath) {
    if (Test-Path -LiteralPath $ancestorPath) {
        Assert-RegularDirectory -Path $ancestorPath -Description 'An existing task-directory ancestor'
    }

    if ($ancestorPath.Equals($workRoot, $pathComparison)) {
        $foundWorkRoot = $true
        break
    }

    $ancestorPath = [IO.Path]::GetDirectoryName($ancestorPath)
}
if (-not $foundWorkRoot) {
    throw 'TaskDirectory does not resolve beneath the repository .work directory.'
}

$codexFullPath = Resolve-RegularFile -Path $CodexPath -Description 'CodexPath'
$consoleAssemblyFullPath = Resolve-RegularFile -Path $ConsoleAssemblyPath -Description 'ConsoleAssemblyPath'
$promptFullPath = Resolve-RegularFile -Path $PromptPath -Description 'PromptPath'
if ([IO.Path]::GetExtension($consoleAssemblyFullPath) -ne '.dll') {
    throw 'ConsoleAssemblyPath must identify the built CreationsForge console DLL.'
}
$codexSha256 = (Get-FileHash -LiteralPath $codexFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
$consoleAssemblySha256 = (Get-FileHash -LiteralPath $consoleAssemblyFullPath -Algorithm SHA256).Hash.ToLowerInvariant()
$promptSha256 = (Get-FileHash -LiteralPath $promptFullPath -Algorithm SHA256).Hash.ToLowerInvariant()

[IO.Directory]::CreateDirectory($taskPath) | Out-Null
Assert-RegularDirectoryHierarchy -Path $taskPath -BoundaryPath $workRoot -Comparison $pathComparison
$sessionPath = Join-Path $taskPath 'session'
[IO.Directory]::CreateDirectory($sessionPath) | Out-Null
Assert-RegularDirectory -Path $sessionPath -Description 'The Codex session directory'

$eventsPath = Join-Path $taskPath 'events.jsonl'
$stderrPath = Join-Path $taskPath 'stderr.txt'
$finalResponsePath = Join-Path $taskPath 'final-response.txt'
$resultPath = Join-Path $taskPath 'process-result.json'

$consoleAssemblyToml = ConvertTo-TomlBasicString -Value $consoleAssemblyFullPath
$arguments = @(
    'exec',
    '--ignore-user-config',
    '--ephemeral',
    '--json',
    '--sandbox',
    'workspace-write',
    '--skip-git-repo-check',
    '-C',
    $sessionPath,
    '-o',
    $finalResponsePath,
    '--disable',
    'apps',
    '--disable',
    'shell_tool',
    '--disable',
    'unified_exec',
    '-c',
    'mcp_servers.creationsforge.command="dotnet"',
    '-c',
    "mcp_servers.creationsforge.args=[$consoleAssemblyToml,`"mcp`"]",
    '-c',
    'mcp_servers.creationsforge.required=true',
    '-c',
    'mcp_servers.creationsforge.startup_timeout_sec=30',
    '-c',
    'mcp_servers.creationsforge.tool_timeout_sec=120'
)
if ($ApproveDisposableSave) {
    $arguments += @(
        '-c',
        'mcp_servers.creationsforge.tools.creationsforge_save.approval_mode="approve"'
    )
}
$arguments += '-'

$startInfo = [Diagnostics.ProcessStartInfo]::new()
$startInfo.FileName = $codexFullPath
$startInfo.WorkingDirectory = $sessionPath
$startInfo.UseShellExecute = $false
$startInfo.CreateNoWindow = $true
$startInfo.RedirectStandardInput = $true
$startInfo.RedirectStandardOutput = $true
$startInfo.RedirectStandardError = $true
foreach ($argument in $arguments) {
    $startInfo.ArgumentList.Add($argument)
}

$promptText = [IO.File]::ReadAllText($promptFullPath)
$child = [Diagnostics.Process]::new()
$child.StartInfo = $startInfo
$eventsFile = $null
$stderrFile = $null
$eventsCopy = $null
$stderrCopy = $null
$captureCompletion = $null
$promptWrite = $null
$captureCancellation = [Threading.CancellationTokenSource]::new()
$started = $false
$standardInputClosed = $false
$promptDeliveryCompleted = $false
$outputCaptureCompleted = $false
$elapsed = [Diagnostics.Stopwatch]::new()
$startedUtc = [DateTime]::UtcNow
$completedUtc = $null
$peakPrivateBytes = 0L
$peakWorkingSetBytes = 0L
$memorySampleCount = 0
$stopReason = $null
$failureMessage = $null
$childExitCode = $null

try {
    $eventsFile = [IO.File]::Open($eventsPath, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::Read)
    $stderrFile = [IO.File]::Open($stderrPath, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::Read)
    $started = $child.Start()
    if (-not $started) {
        throw 'The Codex process did not start.'
    }

    $elapsed.Start()
    $eventsCopy = $child.StandardOutput.BaseStream.CopyToAsync($eventsFile, $captureCancellation.Token)
    $stderrCopy = $child.StandardError.BaseStream.CopyToAsync($stderrFile, $captureCancellation.Token)
    $captureCompletion = [Threading.Tasks.Task]::WhenAll([Threading.Tasks.Task[]] @($eventsCopy, $stderrCopy))
    $child.StandardInput.AutoFlush = $true
    $promptWrite = $child.StandardInput.WriteAsync($promptText)
    Write-Output "Started isolated Codex native-acceptance process (PID $($child.Id))."

    while (-not $child.WaitForExit(100)) {
        if (-not $standardInputClosed -and $promptWrite.IsCompleted) {
            $null = $promptWrite.GetAwaiter().GetResult()
            $child.StandardInput.Close()
            $standardInputClosed = $true
            $promptDeliveryCompleted = $true
        }

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
        } elseif ((Get-CapturedOutputLength -EventsFile $eventsFile -StderrFile $stderrFile -FinalResponsePath $finalResponsePath) -gt ($MaximumCapturedOutputMiB * 1MB)) {
            $stopReason = 'CapturedOutputLimitExceeded'
        }

        if ($stopReason) {
            if (-not $child.HasExited) {
                $child.Kill($true)
            }
            break
        }
    }

    if (-not $child.WaitForExit(10000)) {
        throw 'The stopped Codex process did not exit within ten seconds.'
    }
    $childExitCode = $child.ExitCode
    if (-not $stopReason -and $elapsed.Elapsed.TotalSeconds -gt $MaximumDurationSeconds) {
        $stopReason = 'WallClockLimitExceeded'
    } elseif (-not $stopReason -and (Get-CapturedOutputLength -EventsFile $eventsFile -StderrFile $stderrFile -FinalResponsePath $finalResponsePath) -gt ($MaximumCapturedOutputMiB * 1MB)) {
        $stopReason = 'CapturedOutputLimitExceeded'
    }
    if (-not $standardInputClosed -and $null -ne $promptWrite -and $promptWrite.IsCompleted) {
        $null = $promptWrite.GetAwaiter().GetResult()
        $child.StandardInput.Close()
        $standardInputClosed = $true
        $promptDeliveryCompleted = $true
    }

    # Descendants can retain inherited pipes after the tracked process exits.
    # Drain for at most ten more seconds while retaining the original stop reason.
    $drainStartedSeconds = $elapsed.Elapsed.TotalSeconds
    while (-not $captureCompletion.IsCompleted) {
        if (-not $stopReason -and $elapsed.Elapsed.TotalSeconds -gt $MaximumDurationSeconds) {
            $stopReason = 'WallClockLimitExceeded'
        } elseif (-not $stopReason -and (Get-CapturedOutputLength -EventsFile $eventsFile -StderrFile $stderrFile -FinalResponsePath $finalResponsePath) -gt ($MaximumCapturedOutputMiB * 1MB)) {
            $stopReason = 'CapturedOutputLimitExceeded'
        } elseif (($elapsed.Elapsed.TotalSeconds - $drainStartedSeconds) -ge 10) {
            if (-not $stopReason) {
                $stopReason = 'CapturedOutputDrainTimeout'
            }
            break
        }

        Start-Sleep -Milliseconds 100
    }

    if (-not $captureCompletion.IsCompleted) {
        $captureCancellation.Cancel()
        throw 'Captured output did not finish within the bounded drain; process evidence is incomplete.'
    }

    $null = $captureCompletion.GetAwaiter().GetResult()
    $outputCaptureCompleted = $true
    $eventsFile.Flush($true)
    $stderrFile.Flush($true)
    if (-not $stopReason -and (Get-CapturedOutputLength -EventsFile $eventsFile -StderrFile $stderrFile -FinalResponsePath $finalResponsePath) -gt ($MaximumCapturedOutputMiB * 1MB)) {
        $stopReason = 'CapturedOutputLimitExceeded'
    }
    if (-not $promptDeliveryCompleted) {
        throw 'The Codex process exited before the complete prompt was delivered through stdin.'
    }
    if (-not $stopReason -and $childExitCode -eq 0 -and -not (Test-Path -LiteralPath $finalResponsePath -PathType Leaf)) {
        throw 'The Codex process exited successfully without writing its final-response evidence.'
    }
} catch {
    $failureMessage = $_.Exception.Message
} finally {
    if ($started -and -not $child.HasExited) {
        $child.Kill($true)
        $null = $child.WaitForExit(10000)
    }
    if ($started -and $child.HasExited -and $null -eq $childExitCode) {
        $childExitCode = $child.ExitCode
    }
    if ($started -and -not $standardInputClosed) {
        try {
            $child.StandardInput.Close()
        } catch {
            if (-not $failureMessage) {
                $failureMessage = $_.Exception.Message
            }
        }
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
            if (-not $failureMessage) {
                $failureMessage = $_.Exception.Message
            }
        }
    }
    if ($null -ne $eventsFile) {
        $eventsFile.Dispose()
    }
    if ($null -ne $stderrFile) {
        $stderrFile.Dispose()
    }
    $elapsed.Stop()
    $completedUtc = [DateTime]::UtcNow
    $child.Dispose()
    $captureCancellation.Dispose()
}

try {
    Assert-RegularDirectoryHierarchy -Path $taskPath -BoundaryPath $workRoot -Comparison $pathComparison
    Assert-RegularDirectory -Path $sessionPath -Description 'The Codex session directory'
    $eventsEvidence = Get-EvidenceMetadata -Path $eventsPath -Description 'The JSONL event capture'
    $stderrEvidence = Get-EvidenceMetadata -Path $stderrPath -Description 'The stderr capture'
    $finalResponseEvidence = Get-EvidenceMetadata -Path $finalResponsePath -Description 'The final response'
    $capturedOutputBytes = $eventsEvidence.lengthBytes + $stderrEvidence.lengthBytes
    if ($null -ne $finalResponseEvidence) {
        $capturedOutputBytes += $finalResponseEvidence.lengthBytes
    }
    if (-not $stopReason -and $capturedOutputBytes -gt ($MaximumCapturedOutputMiB * 1MB)) {
        $stopReason = 'CapturedOutputLimitExceeded'
    }
} catch {
    if (-not $failureMessage) {
        $failureMessage = $_.Exception.Message
    }
    $eventsEvidence = $null
    $stderrEvidence = $null
    $finalResponseEvidence = $null
    $capturedOutputBytes = $null
}

$processEvidenceComplete = (
    $started -and
    $childExitCode -eq 0 -and
    $promptDeliveryCompleted -and
    $outputCaptureCompleted -and
    $null -ne $finalResponseEvidence -and
    -not $stopReason -and
    -not $failureMessage)
$summary = [ordered]@{
    evidenceKind = 'CodexProcessEvidence'
    workflowAcceptanceEvaluated = $false
    approveDisposableSave = $ApproveDisposableSave.IsPresent
    startedUtc = $startedUtc.ToString('O')
    completedUtc = $completedUtc.ToString('O')
    elapsedSeconds = [Math]::Round($elapsed.Elapsed.TotalSeconds, 3)
    codexPath = $codexFullPath
    codexSha256 = $codexSha256
    consoleAssemblyPath = $consoleAssemblyFullPath
    consoleAssemblySha256 = $consoleAssemblySha256
    promptPath = $promptFullPath
    promptSha256 = $promptSha256
    sessionDirectory = $sessionPath
    arguments = $arguments
    childExitCode = $childExitCode
    promptDeliveryCompleted = $promptDeliveryCompleted
    outputCaptureCompleted = $outputCaptureCompleted
    peakObservedPrivateBytes = if ($memorySampleCount -gt 0) { $peakPrivateBytes } else { $null }
    peakObservedWorkingSetBytes = if ($memorySampleCount -gt 0) { $peakWorkingSetBytes } else { $null }
    memorySampleCount = $memorySampleCount
    maximumPrivateMemoryMiB = $MaximumPrivateMemoryMiB
    maximumDurationSeconds = $MaximumDurationSeconds
    maximumCapturedOutputMiB = $MaximumCapturedOutputMiB
    capturedOutputBytes = $capturedOutputBytes
    stopReason = $stopReason
    failureMessage = $failureMessage
    events = $eventsEvidence
    stderr = $stderrEvidence
    finalResponse = $finalResponseEvidence
    processEvidenceComplete = $processEvidenceComplete
    acceptanceNotice = 'Process completion alone is not workflow acceptance. Inspect the raw JSONL tool inventory, tool events, MCP results, stderr, and final response.'
    resourceObservation = 'Private and working-set memory were sampled from the tracked Codex process every 100 ms; these are observed peaks, not allocation or descendant-process guarantees.'
}
$summaryJson = $summary | ConvertTo-Json -Depth 8
Write-NewUtf8File -Path $resultPath -Content $summaryJson
$summaryJson | Write-Output
if (-not $processEvidenceComplete) {
    exit 1
}
exit 0
