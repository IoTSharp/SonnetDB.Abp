[CmdletBinding()]
param(
    [ValidateSet('Sqlite', 'SonnetDB')][ValidateCount(1, 2)][string[]]$Providers = @('Sqlite', 'SonnetDB'),
    [ValidateSet('crud', 'join-paging', 'contains-percent', 'contains-underscore', 'contains-backslash',
        'identity-includes', 'binary-key', 'composite-key', 'unique-null', 'foreign-key', 'named-foreign-key',
        'history', 'requires-new', 'passkey-any', 'collection-filter', 'role-union', 'idempotent-script', 'datetimeoffset')]
    [ValidateCount(1, 18)][string[]]$Cases = @('crud'),
    [ValidatePattern('^[a-z0-9-]+$')][string]$LabelPrefix = ('a06-contracts-' + [Guid]::NewGuid().ToString('N')),
    [ValidateRange(40, 900)][int]$TimeoutSeconds = 300
)

$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'PowerShell 7 or later is required.' }
$taskRoot = [IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$taskClock = [Diagnostics.Stopwatch]::StartNew()
$taskResults = [Collections.Generic.List[object]]::new()
$taskCount = $Providers.Count * $Cases.Count
if ($taskCount -gt 36) { throw 'Matrix limit exceeded.' }
# 每个唯一项目只执行一次；总项目数 <=36、总墙钟有限，子命令另有40秒上限。
foreach ($taskProvider in $Providers) {
    foreach ($taskCase in $Cases) {
        if ($taskClock.Elapsed.TotalSeconds -ge ($TimeoutSeconds - 40)) { throw 'Matrix deadline reached before starting another command.' }
        $taskLabel = $LabelPrefix + '-' + $taskProvider.ToLowerInvariant() + '-' + $taskCase
        $taskFailure = $null
        Write-Host "A06 contract $($taskResults.Count + 1)/${taskCount}: $taskProvider $taskCase"
        try {
            & (Join-Path $PSScriptRoot 'Invoke-CheckedCommand.ps1') -Executable dotnet -Arguments @(
                'run', '--project', 'samples/SonnetDB.Abp.A06ContractProbe', '-c', 'Release',
                '--no-build', '--no-restore', '--', $taskProvider, $taskCase
            ) -Label $taskLabel -TimeoutSeconds 40 -WorkingDirectory $taskRoot
        }
        catch { $taskFailure = $_.Exception.Message }
        $taskReceiptPath = Join-Path $taskRoot ('artifacts/checks/' + $taskLabel + '.json')
        $taskReceipt = if (Test-Path -LiteralPath $taskReceiptPath) { Get-Content -LiteralPath $taskReceiptPath -Raw | ConvertFrom-Json } else { $null }
        $taskResults.Add([pscustomobject][ordered]@{
            provider = $taskProvider; case = $taskCase; exitCode = $taskReceipt.exitCode
            failure = $taskFailure; receipt = $taskReceiptPath; elapsedSeconds = $taskReceipt.elapsedSeconds
        })
        if ($taskReceipt.timedOut -or $taskReceipt.remainingRecordedProcesses.Count -gt 0) { throw 'Stop matrix after timeout or incomplete process cleanup.' }
    }
}
$taskSummaryPath = Join-Path $taskRoot ('artifacts/checks/' + $LabelPrefix + '-summary.json')
$taskResults | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $taskSummaryPath -Encoding utf8
$taskResults | Format-Table provider, case, exitCode
Write-Host "SUMMARY $taskSummaryPath"
if (@($taskResults | Where-Object { $_.failure }).Count -gt 0) { exit 1 }
