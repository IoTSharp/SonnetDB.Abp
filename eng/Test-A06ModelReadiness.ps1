[CmdletBinding()]
param(
    [ValidateRange(90, 240)][int]$TimeoutSeconds = 240,
    [ValidateRange(5, 40)][int]$CommandTimeoutSeconds = 30,
    [string]$WorkingDirectory = (Split-Path -Parent $PSScriptRoot),
    [switch]$Micro,
    [switch]$PlanOnly
)

$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'PowerShell 7 or later is required.' }
$a06Clock = [Diagnostics.Stopwatch]::StartNew()
$a06StartedAt = [DateTime]::UtcNow
$a06Root = [IO.Path]::GetFullPath($WorkingDirectory)
$a06Helper = Join-Path $a06Root 'eng/Invoke-CheckedCommand.ps1'
$a06Repro = 'samples/SonnetDB.Abp.JsonColumnRepro'
$a06Migration = 'samples/BookStore/src/Acme.BookStore.SonnetDB.Migrations'
$a06Migrator = 'samples/BookStore/src/Acme.BookStore.DbMigrator'
$a06RunId = [Guid]::NewGuid().ToString('N')
$a06Connection = 'Data Source=' + [IO.Path]::GetFullPath(
    (Join-Path $a06Root ('artifacts/a06-model-readiness-' + $a06RunId)))

# 前置条件：最终根解决方案已 restore 并以 Release 构建，工具已有 dotnet-ef。
# 所有正式命令使用 --no-build；不执行迁移、不创建数据库或源码迁移文件。
$a06Cases = @(
    [pscustomobject]@{ Name = 'sqlite-complex'; Arguments = @('run', '--project', $a06Repro, '-c', 'Release', '--no-build', '--no-restore', '--', 'Sqlite', 'Complex') }
    [pscustomobject]@{ Name = 'sonnetdb-complex'; Arguments = @('run', '--project', $a06Repro, '-c', 'Release', '--no-build', '--no-restore', '--', 'SonnetDB', 'Complex') }
    [pscustomobject]@{ Name = 'sqlite-owned'; Arguments = @('run', '--project', $a06Repro, '-c', 'Release', '--no-build', '--no-restore', '--', 'Sqlite', 'Owned') }
    [pscustomobject]@{ Name = 'sonnetdb-owned'; Arguments = @('run', '--project', $a06Repro, '-c', 'Release', '--no-build', '--no-restore', '--', 'SonnetDB', 'Owned') }
    [pscustomobject]@{ Name = 'bookstore-sonnetdb'; Arguments = @('ef', 'dbcontext', 'script', '--project', $a06Migration, '--startup-project', $a06Migrator, '--context', 'BookStoreDbContext', '--configuration', 'Release', '--no-build', '--', '--Database:Provider', 'SonnetDB', '--ConnectionStrings:Default', $a06Connection) }
)
if ($a06Cases.Count -ne 5) { throw 'A06 readiness requires exactly five bounded checks.' }
$a06CaseCount = if ($Micro) { 1 } else { 5 }
if (-not [IO.File]::Exists($a06Helper)) { throw "Checked command helper is missing: $a06Helper" }

Write-Host 'A06: design-time model readiness and SQL script generation only.'
Write-Host 'Success does not establish database migrations, SQL execution or the official application journey.'
if ($PlanOnly) {
    # 极小输入检查只展示五项计划，不启动 dotnet、不写验收结果。
    Write-Host 'PlanOnly: no checks executed; restore and Release build must precede a real run.'
    $a06Cases | Select-Object -First $a06CaseCount | ConvertTo-Json -Depth 4
    return
}

$a06RequiredAssemblies = @(
    'samples/SonnetDB.Abp.JsonColumnRepro/bin/Release/net10.0/SonnetDB.Abp.JsonColumnRepro.dll'
    'samples/BookStore/src/Acme.BookStore.DbMigrator/bin/Release/net10.0/Acme.BookStore.DbMigrator.dll'
)
foreach ($a06Assembly in $a06RequiredAssemblies) {
    if (-not [IO.File]::Exists((Join-Path $a06Root $a06Assembly))) {
        throw "Restore and build SonnetDB.Abp.slnx in Release before readiness checks: $a06Assembly"
    }
}

$a06Results = [Collections.Generic.List[object]]::new()
$a06SummaryPath = Join-Path $a06Root ('artifacts/checks/a06-model-readiness-' + $a06RunId + '.json')
if ([IO.File]::Exists($a06SummaryPath)) { throw 'Refusing to overwrite an existing readiness summary.' }
# 为现有 helper 的身份核验、取消、清理及最终报告预留 75 秒。
# 在剩余预算不足时不再启动下一条命令；正式最多五次，Micro 仅首项 SQLite Complex。
# 主调用方仍须用 checked helper 启动 pwsh（外层 TimeoutSeconds <= 300），作为最终硬停止。
$a06CleanupReserveSeconds = 75
for ($a06Index = 0; $a06Index -lt $a06CaseCount; $a06Index++) {
    $a06Case = $a06Cases[$a06Index]
    $a06Available = [int][Math]::Floor($TimeoutSeconds - $a06Clock.Elapsed.TotalSeconds - $a06CleanupReserveSeconds)
    $a06Result = [ordered]@{
        name = $a06Case.Name
        status = 'NotExecutedBudget'
        arguments = $a06Case.Arguments
        label = $null
        timeoutSeconds = $null
        exitCode = $null
        failure = 'Overall deadline leaves insufficient command and cleanup budget.'
        receipt = $null
        stdout = $null
        stderr = $null
    }
    if ($a06Available -ge 5) {
        $a06Budget = [Math]::Min($CommandTimeoutSeconds, $a06Available)
        $a06Label = 'a06-model-' + [Guid]::NewGuid().ToString('N') + '-' + $a06Case.Name
        $a06Prefix = Join-Path $a06Root ('artifacts/checks/' + $a06Label)
        $a06Result.label = $a06Label
        $a06Result.timeoutSeconds = $a06Budget
        $a06Result.receipt = $a06Prefix + '.json'
        $a06Result.stdout = $a06Prefix + '.stdout.log'
        $a06Result.stderr = $a06Prefix + '.stderr.log'
        Write-Host "A06 $($a06Index + 1)/${a06CaseCount}: $($a06Case.Name), command budget ${a06Budget}s"
        try {
            # helper 记录 PID、创建时间、命令行和父链，并清理其核验归属的子进程。
            & $a06Helper -Executable dotnet -Arguments $a06Case.Arguments -Label $a06Label -TimeoutSeconds $a06Budget -WorkingDirectory $a06Root | ForEach-Object { Write-Host $_ }
            $a06Result.status = 'Succeeded'
            $a06Result.failure = $null
        }
        catch {
            $a06Result.status = 'Failed'
            $a06Result.failure = $_.Exception.Message
            Write-Host "A06 $($a06Case.Name) failed; retaining evidence and continuing independent checks."
        }
        try {
            $a06Receipt = Get-Content -LiteralPath $a06Result.receipt -Raw | ConvertFrom-Json
            $a06Result.exitCode = $a06Receipt.exitCode
            if ($a06Receipt.exitCode -ne 0 -or $a06Receipt.timedOut -or $a06Receipt.failure -or $a06Receipt.remainingRecordedProcesses.Count -gt 0) {
                $a06Result.status = 'Failed'
                if (-not $a06Result.failure) { $a06Result.failure = 'The checked command receipt did not confirm successful execution and cleanup.' }
            }
        }
        catch {
            $a06Result.status = 'Failed'
            $a06Result.failure = 'Unable to verify the checked command receipt: ' + $_.Exception.Message
        }
    }
    $a06Results.Add([pscustomobject]$a06Result)
}

$a06Succeeded = @($a06Results | Where-Object status -eq 'Succeeded').Count
$a06Failed = @($a06Results | Where-Object status -eq 'Failed').Count
$a06NotExecuted = @($a06Results | Where-Object status -eq 'NotExecutedBudget').Count
$a06WithinDeadline = $a06Clock.Elapsed.TotalSeconds -le $TimeoutSeconds
$a06AllModelsReady = -not $Micro -and $a06Succeeded -eq 5 -and $a06WithinDeadline
$a06MicroSucceeded = $Micro -and $a06Succeeded -eq 1 -and $a06WithinDeadline
$a06Summary = [ordered]@{
    task = 'A06'
    scope = if ($Micro) { 'Micro' } else { 'FullModelReadiness' }
    acceptance = 'Design-time JSON models and official model SQL script generation only; no database or migration execution requested.'
    startedAt = $a06StartedAt
    completedAt = [DateTime]::UtcNow
    timeoutSeconds = $TimeoutSeconds
    elapsedSeconds = $a06Clock.Elapsed.TotalSeconds
    allModelsReady = $a06AllModelsReady
    microSucceeded = $a06MicroSucceeded
    planned = $a06CaseCount
    remaining = 5 - $a06CaseCount
    succeeded = $a06Succeeded
    failed = $a06Failed
    notExecuted = $a06NotExecuted
    results = $a06Results.ToArray()
}
New-Item -ItemType Directory -Path (Split-Path -Parent $a06SummaryPath) -Force | Out-Null
[IO.File]::WriteAllText($a06SummaryPath, ($a06Summary | ConvertTo-Json -Depth 8))
Write-Host "A06 summary: $a06SummaryPath"
Write-Host "A06 $($a06Summary.scope): succeeded=$a06Succeeded, failed=$a06Failed, not-executed=$a06NotExecuted."
if ($Micro) {
    Write-Host 'Micro checks only SQLite Complex; full A06 model readiness remains unverified.'
    if (-not $a06MicroSucceeded) { throw 'A06 micro check failed or exceeded its deadline. Review the retained receipt.' }
}
elseif (-not $a06AllModelsReady) { throw 'A06 model readiness failed or was incomplete. Review the retained receipts; expected failures remain failures.' }
