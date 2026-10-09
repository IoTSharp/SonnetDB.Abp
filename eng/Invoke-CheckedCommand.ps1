[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidateSet('dotnet', 'gh', 'git', 'pwsh')][string]$Executable,
    [Parameter(Mandatory)][ValidateCount(1, 32)][string[]]$Arguments,
    [Parameter(Mandatory)][ValidatePattern('^[a-z0-9-]+$')][string]$Label,
    [ValidateRange(5, 300)][int]$TimeoutSeconds = 120,
    [string]$WorkingDirectory = (Split-Path -Parent $PSScriptRoot),
    [switch]$Proxy
)

$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'PowerShell 7 or later is required.' }
$taskDirectory = [IO.Path]::GetFullPath($WorkingDirectory)
$taskLogDirectory = Join-Path $taskDirectory 'artifacts/checks'
New-Item -ItemType Directory -Path $taskLogDirectory -Force | Out-Null
$taskPrefix = Join-Path $taskLogDirectory $Label
if (Test-Path -LiteralPath ($taskPrefix + '.json')) { throw 'Use a new label for each attempt.' }
$taskStartInfo = [Diagnostics.ProcessStartInfo]::new()
$taskStartInfo.FileName = (Get-Command $Executable -CommandType Application | Select-Object -First 1).Source
$taskStartInfo.WorkingDirectory = $taskDirectory
$taskStartInfo.UseShellExecute = $false
$taskStartInfo.RedirectStandardOutput = $true
$taskStartInfo.RedirectStandardError = $true
foreach ($taskArgument in $Arguments) { $taskStartInfo.ArgumentList.Add($taskArgument) }
$taskStartInfo.Environment['MSBUILDDISABLENODEREUSE'] = '1'
$taskStartInfo.Environment['DOTNET_CLI_USE_MSBUILD_SERVER'] = '0'
$taskStartInfo.Environment['DOTNET_CLI_TELEMETRY_OPTOUT'] = '1'
if ($Proxy) {
    $taskStartInfo.Environment['HTTP_PROXY'] = 'http://127.0.0.1:7890'
    $taskStartInfo.Environment['HTTPS_PROXY'] = 'http://127.0.0.1:7890'
}
$taskProcess = [Diagnostics.Process]::new()
$taskProcess.StartInfo = $taskStartInfo
$taskClock = [Diagnostics.Stopwatch]::StartNew()
$taskIdentities = [Collections.Generic.Dictionary[int, object]]::new()
$taskTimedOut = $false
$taskFailure = $null
$taskExitCode = $null
$taskRootIdentity = $null
$taskRemaining = @()
$taskLaunchedPid = $null
$taskLaunchedAt = $null

function Save-TaskDescendants {
    param([int[]]$ParentIds)
    $taskParents = $ParentIds
    for ($taskDepth = 0; $taskDepth -lt 8 -and $taskParents.Count -gt 0; $taskDepth++) {
        $taskNext = [Collections.Generic.List[int]]::new()
        foreach ($taskParentId in $taskParents) {
            if ($taskClock.Elapsed.TotalSeconds -ge $TimeoutSeconds) { throw 'Process discovery deadline exceeded.' }
            if ($taskIdentities.Count -ge 128) { throw 'Process ledger limit exceeded.' }
            $taskChildren = @(Get-CimInstance Win32_Process -Filter "ParentProcessId = $taskParentId" -OperationTimeoutSec 3)
            foreach ($taskChild in $taskChildren) {
                if ($taskIdentities.Count -ge 128) { throw 'Process ledger limit exceeded.' }
                if (-not $taskChild.CommandLine -or -not $taskChild.CreationDate) { throw 'Incomplete owned child identity.' }
                $taskIdentities[[int]$taskChild.ProcessId] = $taskChild
                $taskNext.Add([int]$taskChild.ProcessId)
            }
        }
        $taskParents = $taskNext.ToArray()
    }
    if ($taskParents.Count -gt 0) { throw 'Process tree depth limit exceeded.' }
}

try {
    if (-not $taskProcess.Start()) { throw 'Process failed to start.' }
    $taskLaunchedPid = $taskProcess.Id
    $taskLaunchedAt = $taskProcess.StartTime.ToUniversalTime()
    $taskStdout = $taskProcess.StandardOutput.ReadToEndAsync()
    $taskStderr = $taskProcess.StandardError.ReadToEndAsync()
    $taskRootIdentity = Get-CimInstance Win32_Process -Filter "ProcessId = $($taskProcess.Id)" -OperationTimeoutSec 3
    if ($taskRootIdentity) {
        if (-not $taskRootIdentity.CommandLine -or $taskRootIdentity.ParentProcessId -ne $PID) { throw 'Root identity could not be verified.' }
        $taskIdentities[[int]$taskProcess.Id] = $taskRootIdentity
    }
    elseif (-not $taskProcess.HasExited) { throw 'Live root identity could not be verified.' }
    $taskMaximumPolls = $TimeoutSeconds * 4
    for ($taskPoll = 0; $taskPoll -lt $taskMaximumPolls -and $taskClock.Elapsed.TotalSeconds -lt $TimeoutSeconds; $taskPoll++) {
        if ($taskProcess.WaitForExit(250)) { break }
        if ($taskPoll % 40 -eq 0) {
            Save-TaskDescendants -ParentIds @($taskProcess.Id)
            Write-Host "$Label running: $([int]$taskClock.Elapsed.TotalSeconds)s"
        }
    }
    if (-not $taskProcess.HasExited) { $taskTimedOut = $true; throw 'Command deadline exceeded.' }
    $taskExitCode = $taskProcess.ExitCode
}
catch { $taskFailure = $_.Exception.Message }
finally {
    $taskCleanupClock = [Diagnostics.Stopwatch]::StartNew()
    if ($taskRootIdentity -and -not $taskProcess.HasExited) {
        $taskLiveRoot = Get-CimInstance Win32_Process -Filter "ProcessId = $($taskRootIdentity.ProcessId)" -OperationTimeoutSec 3
        if ($taskLiveRoot -and $taskLiveRoot.CreationDate -eq $taskRootIdentity.CreationDate -and $taskLiveRoot.CommandLine -ceq $taskRootIdentity.CommandLine -and $taskLiveRoot.ParentProcessId -eq $taskRootIdentity.ParentProcessId) {
            $taskProcess.Kill($true)
            if (-not $taskProcess.WaitForExit(5000)) { $taskFailure = 'Owned root did not exit after cancellation.' }
        }
    }
    foreach ($taskIdentity in @($taskIdentities.Values)) {
        if ($taskCleanupClock.Elapsed.TotalSeconds -ge 30) { $taskFailure = 'Recorded process cleanup deadline exceeded.'; break }
        if ($taskIdentity.ProcessId -eq $taskRootIdentity.ProcessId) { continue }
        $taskLive = Get-CimInstance Win32_Process -Filter "ProcessId = $($taskIdentity.ProcessId)" -OperationTimeoutSec 3
        if ($taskLive -and $taskLive.CreationDate -eq $taskIdentity.CreationDate -and $taskLive.CommandLine -ceq $taskIdentity.CommandLine -and $taskLive.ParentProcessId -eq $taskIdentity.ParentProcessId) {
            $taskOwnedProcess = [Diagnostics.Process]::GetProcessById([int]$taskIdentity.ProcessId)
            try { $taskOwnedProcess.Kill($true); $null = $taskOwnedProcess.WaitForExit(5000) }
            finally { $taskOwnedProcess.Dispose() }
        }
    }
    if ($taskStdout -and $taskStdout.IsCompleted) { [IO.File]::WriteAllText($taskPrefix + '.stdout.log', $taskStdout.GetAwaiter().GetResult()) }
    if ($taskStderr -and $taskStderr.IsCompleted) { [IO.File]::WriteAllText($taskPrefix + '.stderr.log', $taskStderr.GetAwaiter().GetResult()) }
    foreach ($taskIdentity in @($taskIdentities.Values)) {
        if ($taskCleanupClock.Elapsed.TotalSeconds -ge 45) { $taskFailure = 'Recorded process audit deadline exceeded.'; break }
        $taskLive = Get-CimInstance Win32_Process -Filter "ProcessId = $($taskIdentity.ProcessId)" -OperationTimeoutSec 3
        if ($taskLive -and $taskLive.CreationDate -eq $taskIdentity.CreationDate) { $taskRemaining += [int]$taskIdentity.ProcessId }
    }
    $taskReceipt = [ordered]@{
        label = $Label; executable = $taskStartInfo.FileName; arguments = $Arguments
        launchedPid = $taskLaunchedPid; launchedAt = $taskLaunchedAt; launchingParentPid = $PID
        started = $taskRootIdentity.CreationDate; pid = $taskRootIdentity.ProcessId
        parentPid = $taskRootIdentity.ParentProcessId; commandLine = $taskRootIdentity.CommandLine
        elapsedSeconds = $taskClock.Elapsed.TotalSeconds; exitCode = $taskExitCode
        timedOut = $taskTimedOut; failure = $taskFailure; remainingRecordedProcesses = $taskRemaining
        recordedIdentities = @($taskIdentities.Values | Select-Object ProcessId, ParentProcessId, CreationDate, CommandLine)
        unobservedDescendantsProvenAbsent = $false
    }
    [IO.File]::WriteAllText($taskPrefix + '.json', ($taskReceipt | ConvertTo-Json -Depth 6))
    $taskProcess.Dispose()
}
Get-Content -LiteralPath ($taskPrefix + '.stdout.log') -ErrorAction SilentlyContinue | Select-Object -Last 25
Get-Content -LiteralPath ($taskPrefix + '.stderr.log') -ErrorAction SilentlyContinue | Select-Object -Last 15
if ($taskFailure -or $taskRemaining.Count -gt 0) { throw "$Label failed: $taskFailure; remaining=$($taskRemaining.Count)" }
if ($taskExitCode -ne 0) { throw "$Label exited with $taskExitCode" }
